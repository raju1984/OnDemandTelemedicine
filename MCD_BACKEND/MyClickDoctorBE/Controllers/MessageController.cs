using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyClickDoctorBE.App_Resources;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyClickDoctorBE.Models.MessageHub;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class MessageController : ControllerBase
    {
        private readonly MyClickDoctorV4 _db;
        public MessageController(MyClickDoctorV4 context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task<IActionResult> MakeChat(Message msg)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                Chat ch = new Chat()
                {
                    SenderId = msg.SenderId,
                    RecieverId = msg.RecieverId,
                    SenderUserType = msg.SenderUserType,
                    RecieverUserType = msg.RecieverUserType,
                    ConnectionId = msg.ConnectionId,
                    Message = msg.Messages,
                    CreatedAt = DateTime.UtcNow,
                    Isseen = false,
                    Userseen = false,
                    RequestType = msg.RequestType,
                    Chattype = msg.Chattype,
                    Chatwith = msg.Chatwith,
                };
                _db.Chat.Add(ch);
                int i = await _db.SaveChangesAsync();
                if (i > 0)
                {
                    if (msg.Chatwith == "group")
                    {
                        var list = _db.Groupchat.Where(a => a.ConnectionId == msg.ConnectionId).ToList();
                        foreach (var item in list)
                        {
                            GroupMsgSeen msgSeen = new GroupMsgSeen()
                            {
                                MsgId = msg.ConnectionId,
                                ParticipateId = item.ParticipateId
                            };
                            _db.GroupMsgSeen.Add(msgSeen);
                            _db.SaveChanges();
                        }
                    }
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult GetUserChatHistory(int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            msglist data = new msglist();
            List<Messge> list = new List<Messge>();
            try
            {
                data.data = (from r in _db.Chat
                             where r.SenderId == UserId && r.Chattype == null && r.SenderUserType != 1 || r.RecieverId == UserId && r.SenderUserType == 1
                             select new Messge
                             {
                                 MyId = r.SenderId,
                                 YourId = r.RecieverId,
                                 Messages = r.Message,
                                 ChatUserName = AppDbCommonLogic.GetUserChatName(r.SenderId, r.SenderUserType),
                                 CreatedAt = r.CreatedAt,
                                 RequestType = r.RequestType,
                                 //SenderUserType=r.SenderUserType,
                                 //RecieverUserType=r.RecieverUserType
                             }).ToList();
                //list = (from r in _db.Chat
                //             where r.RecieverId == UserId && r.SenderUserType == 1
                //             select new Messge
                //             {
                //                 MyId = r.RecieverId,
                //                 YourId = r.SenderId,
                //                 Messages = r.Message,
                //                 ChatUserName = AppDbCommonLogic.GetUserChatName(r.SenderId, r.SenderUserType),
                //                 CreatedAt = r.CreatedAt,
                //                 RequestType = r.RequestType
                //             }).ToList();
                //if (list.Count > 0)
                //{
                //    data.data.AddRange(list);
                //}
                if (data.data.Count > 0)
                {
                    data.Count = AppDbCommonLogic.GetUserCount(UserId);
                    data.TotalCount = AppDbCommonLogic.GetUserTotalCount(UserId);
                    data.AdminCount = AppDbCommonLogic.GetUserTotalAdminCount(UserId);
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult GetUserChatList()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            msglist data = new msglist();
            List<Messge> list = new List<Messge>();
            try
            {
                data.data = (from r in _db.Chat
                             where r.RequestType == "user" && r.Chattype == null
                             select new Messge
                             {
                                 SenderId = r.Id,
                                 MyId = r.SenderId,
                                 SenderUserType = r.SenderUserType,
                                 YourId = r.RecieverId,
                                 Messages = r.Message,
                                 ChatUserName = AppDbCommonLogic.GetUserChatName(r.SenderId, r.SenderUserType),
                                 CreatedAt = r.CreatedAt,
                                 SeenCount = AppDbCommonLogic.GetAdminCount((int)r.SenderId)
                             }).ToList();
                if (data.data.Count > 0)
                {
                    data.data = data.data.OrderByDescending(a => a.SenderId).GroupBy(x => x.MyId).Select(x => x.First()).ToList();
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> SeenAdminUserMessage(UpdateStatus data)
        {
            UpdateStatus Resp = new UpdateStatus();
            try
            {
                var getpro = _db.Chat.Where(a => a.SenderId == data.UserId || a.RecieverId == data.UserId).ToList();
                if (getpro.Count > 0)
                {
                    foreach (var item in getpro)
                    {
                        item.Isseen = true;
                    }
                    int j = await _db.SaveChangesAsync();
                    if (j > 0)
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_update;
                        Resp.BussinessStatus = data.BussinessStatus;
                        Resp.UserId = data.UserId;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_update;
                        return Ok(Resp);
                    }
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    Resp.msg = AppResource.invalid_model;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SeenUserAdminMessage(UpdateStatus data)
        {
            UpdateStatus Resp = new UpdateStatus();
            try
            {
                var getpro = _db.Chat.Where(a => a.SenderId == data.UserId || a.RecieverId == data.UserId).ToList();
                if (getpro.Count > 0)
                {
                    foreach (var item in getpro)
                    {
                        item.Userseen = true;
                    }
                    int j = await _db.SaveChangesAsync();
                    if (j > 0)
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_update;
                        return Ok(Resp);
                    }
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    Resp.msg = AppResource.invalid_model;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public ActionResult GetSupportStatus()
        {
            myversion resp = new myversion();
            resp.code = AppDbCommonLogic.checkStatusofSupport();
            if (resp.code > 0)
            {
                resp.code = (int)ApiCustomResponseCode.Ok;
            }
            else
            {
                resp.code = (int)ApiCustomResponseCode.Error;
            }

            return Ok(resp);
        }
        [HttpGet]
        public ActionResult GetDoctorChatList(int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            msglist data = new msglist();
            List<Messge> list = new List<Messge>();
            List<Messge> patientlist = new List<Messge>();
            try
            {
                data.data = (from r in _db.Chat
                             where (r.SenderId == UserId || r.RecieverId == UserId) && (r.Chattype == "chat") && r.Message != null && r.Message != "" && r.Chatwith == "doctor"
                             select new Messge
                             {
                                 SenderId = r.Id,
                                 MyId = AppDbCommonLogic.GetsenderId(r.SenderId, UserId, r.RecieverId),
                                 SenderUserType = r.SenderUserType,
                                 RecieverUserType = r.RecieverUserType,
                                 YourId = AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId),
                                 Messages = r.Message,
                                 ChatUserName = AppDbCommonLogic.GetDoctorChatName(r.SenderId, r.RecieverId, UserId),
                                 CreatedAt = r.CreatedAt,
                                 // SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)r.SenderId),
                                 ConnectionId = r.ConnectionId,
                                 DocCategory = _db.Doctors.Where(a => a.UserId == (AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId))).FirstOrDefault().DocCategory,
                                 DocId = _db.Doctors.Where(a => a.UserId == (AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId))).FirstOrDefault().Id,
                             }).ToList();
                patientlist = (from r in _db.Chat
                               where (r.SenderId == UserId || r.RecieverId == UserId) && (r.Chattype == "chat") && r.Message != null && r.Message != "" && r.Chatwith == "patient"
                               select new Messge
                               {
                                   SenderId = r.Id,
                                   MyId = AppDbCommonLogic.GetsenderId(r.SenderId, UserId, r.RecieverId),
                                   SenderUserType = r.SenderUserType,
                                   RecieverUserType = r.RecieverUserType,
                                   YourId = AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId),
                                   Messages = r.Message,
                                   ChatUserName = AppDbCommonLogic.GetPatientChatName(r.SenderId, r.RecieverId, UserId),
                                   CreatedAt = r.CreatedAt,
                                   // SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)r.SenderId),
                                   ConnectionId = r.ConnectionId,
                                   DocCategory = "patient",
                                   DocId = AppDbCommonLogic.GetPatientId(r.SenderId, r.RecieverId, UserId),
                               }).ToList();
                data.data.AddRange(patientlist);
                if (data.data.Count > 0)
                {
                    data.data = data.data.OrderByDescending(a => a.CreatedAt).GroupBy(x => x.YourId).Select(x => x.First()).ToList();
                    //  data.data.ForEach(a => a.SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)a.YourId),a.);
                    var correction = (from abc in data.data
                                      select new Messge
                                      {
                                          SenderId = abc.SenderId,
                                          MyId = abc.MyId,
                                          SenderUserType = abc.SenderUserType,
                                          RecieverUserType = abc.RecieverUserType,
                                          YourId = abc.YourId,
                                          Messages = abc.Messages,
                                          ChatUserName = abc.ChatUserName,
                                          CreatedAt = abc.CreatedAt,
                                          ConnectionId = abc.ConnectionId,
                                          DocCategory = abc.DocCategory,
                                          DocId = abc.DocId,
                                          SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)abc.YourId),
                                          NotificationStatus = AppDbCommonLogic.GetDoctorNotificationstatus((int)abc.YourId)
                                      }).ToList();
                    data.data = null;
                    data.data = correction;
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult GetDoctorChatHistory(string ConnectionId, int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            msglist data = new msglist();
            List<Messge> list = new List<Messge>();
            try
            {
                data.data = (from r in _db.Chat
                             where r.ConnectionId == ConnectionId && r.Chattype == "chat"
                             select new Messge
                             {
                                 MyId = r.SenderId,
                                 YourId = r.RecieverId,
                                 Messages = r.Message,
                                 ChatUserName = AppDbCommonLogic.GetUserChatName(r.SenderId, r.SenderUserType),
                                 CreatedAt = r.CreatedAt,
                                 RequestType = AppDbCommonLogic.GetRequesttype(r.SenderId, UserId),
                             }).ToList();
                if (data.data.Count > 0)
                {
                    data.TotalCount = AppDbCommonLogic.GetDoctorChatHistoryCount(ConnectionId);
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> SeenDoctorMessage(updateseen data)
        {
            UpdateStatus Resp = new UpdateStatus();
            try
            {
                var getpro = _db.Chat.Where(a => a.ConnectionId == data.ConnectionId && a.RecieverId == data.UserId).ToList();
                if (getpro.Count > 0)
                {
                    foreach (var item in getpro)
                    {
                        item.Isseen = true;
                    }
                    int j = await _db.SaveChangesAsync();
                    if (j > 0)
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_update;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_update;
                        return Ok(Resp);
                    }
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    Resp.msg = AppResource.invalid_model;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Createnewchat(Message msg)
        {
            newchat resp = new newchat();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Ok;
            try
            {
                string MyId = msg.SenderId.ToString() + "" + msg.RecieverId.ToString();
                msg.ConnectionId = MyId;

                if (msg.RecieverUserType == 2)
                {
                    resp.ChatName = AppDbCommonLogic.GetDoctorchatName(msg.RecieverId);
                    var doctors = _db.Doctors.Where(a => a.UserId == msg.RecieverId).FirstOrDefault();
                    if (doctors != null)
                    {
                        resp.DocCategory = doctors.DocCategory;
                        resp.DocId = doctors.Id;
                    }
                }
                else
                {
                    var patient = _db.Patient.Where(a => a.UserId == msg.RecieverId).FirstOrDefault();
                    if (patient != null)
                    {
                        resp.DocCategory = "patient";
                        resp.DocId = patient.Id;
                        resp.ChatName = patient.SecondName + " " + patient.FirstName;
                    }
                }

                var checkrecord = _db.Chat.Where(a => a.ConnectionId == MyId).FirstOrDefault();
                if (checkrecord != null)
                {
                    resp.ConnectionId = MyId;
                }
                else
                {
                    string yourId = msg.RecieverId.ToString() + "" + msg.SenderId.ToString();
                    var checkrecords = _db.Chat.Where(a => a.ConnectionId == yourId).FirstOrDefault();
                    if (checkrecords != null)
                    {
                        resp.ConnectionId = yourId;
                    }
                    else
                    {
                        Chat ch = new Chat()
                        {
                            SenderId = msg.SenderId,
                            RecieverId = msg.RecieverId,
                            SenderUserType = msg.SenderUserType,
                            RecieverUserType = msg.RecieverUserType,
                            ConnectionId = msg.ConnectionId,
                            Message = "",
                            CreatedAt = DateTime.UtcNow,
                            Isseen = false,
                            Userseen = false,
                            Chattype = "chat",
                            Chatwith = resp.DocCategory == "patient" ? resp.DocCategory : "doctor",
                        };
                        _db.Chat.Add(ch);
                        int i = await _db.SaveChangesAsync();
                        if (i > 0)
                        {
                            resp.ConnectionId = msg.ConnectionId;
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            return Ok(resp);
                        }
                        else
                        {
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult GetPatientChatList(int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            msglist data = new msglist();
            List<Messge> list = new List<Messge>();
            List<Messge> patientlist = new List<Messge>();
            try
            {
                data.data = (from r in _db.Chat
                             where (r.SenderId == UserId || r.RecieverId == UserId) && (r.Chattype == "chat") && r.Message != null && r.Message != "" && r.Chatwith == "patient"
                             select new Messge
                             {
                                 SenderId = r.Id,
                                 MyId = AppDbCommonLogic.GetsenderId(r.SenderId, UserId, r.RecieverId),
                                 SenderUserType = r.SenderUserType,
                                 RecieverUserType = r.RecieverUserType,
                                 YourId = AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId),
                                 Messages = r.Message,
                                 ChatUserName = AppDbCommonLogic.GetDoctorChatName(r.SenderId, r.RecieverId, UserId),
                                 CreatedAt = r.CreatedAt,
                                 // SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)r.SenderId),
                                 ConnectionId = r.ConnectionId,
                                 DocCategory = _db.Doctors.Where(a => a.UserId == (AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId))).FirstOrDefault().DocCategory,
                                 DocId = _db.Doctors.Where(a => a.UserId == (AppDbCommonLogic.GetRecieverId(r.RecieverId, UserId, r.SenderId))).FirstOrDefault().Id,
                             }).ToList();
                if (data.data.Count > 0)
                {
                    data.data = data.data.OrderByDescending(a => a.CreatedAt).GroupBy(x => x.YourId).Select(x => x.First()).ToList();
                    //  data.data.ForEach(a => a.SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)a.YourId),a.);
                    var correction = (from abc in data.data
                                      select new Messge
                                      {
                                          SenderId = abc.SenderId,
                                          MyId = abc.MyId,
                                          SenderUserType = abc.SenderUserType,
                                          RecieverUserType = abc.RecieverUserType,
                                          YourId = abc.YourId,
                                          Messages = abc.Messages,
                                          ChatUserName = abc.ChatUserName,
                                          CreatedAt = abc.CreatedAt,
                                          ConnectionId = abc.ConnectionId,
                                          DocCategory = abc.DocCategory,
                                          DocId = abc.DocId,
                                          SeenCount = AppDbCommonLogic.GetDoctorSeenCount((int)abc.YourId),
                                          NotificationStatus = AppDbCommonLogic.GetDoctorNotificationstatus((int)abc.YourId)
                                      }).ToList();
                    data.data = null;
                    data.data = correction;
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> Createnewchatforpatient(Message msg)
        {
            newchat resp = new newchat();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Ok;
            try
            {
                string MyId = msg.SenderId.ToString() + "" + msg.RecieverId.ToString();
                msg.ConnectionId = MyId;
                resp.ChatName = AppDbCommonLogic.GetDoctorchatName(msg.RecieverId);
                var doctors = _db.Doctors.Where(a => a.UserId == msg.RecieverId).FirstOrDefault();
                if (doctors != null)
                {
                    resp.DocCategory = doctors.DocCategory;
                    resp.DocId = doctors.Id;
                }
                var checkrecord = _db.Chat.Where(a => a.ConnectionId == MyId).FirstOrDefault();
                if (checkrecord != null)
                {
                    resp.ConnectionId = MyId;
                }
                else
                {
                    string yourId = msg.RecieverId.ToString() + "" + msg.SenderId.ToString();
                    var checkrecords = _db.Chat.Where(a => a.ConnectionId == yourId).FirstOrDefault();
                    if (checkrecords != null)
                    {
                        resp.ConnectionId = yourId;
                    }
                    else
                    {
                        Chat ch = new Chat()
                        {
                            SenderId = msg.SenderId,
                            RecieverId = msg.RecieverId,
                            SenderUserType = msg.SenderUserType,
                            RecieverUserType = msg.RecieverUserType,
                            ConnectionId = msg.ConnectionId,
                            Message = "",
                            CreatedAt = DateTime.UtcNow,
                            Isseen = false,
                            Userseen = false,
                            Chattype = "chat",
                            Chatwith = "patient",
                        };
                        _db.Chat.Add(ch);
                        int i = await _db.SaveChangesAsync();
                        if (i > 0)
                        {
                            resp.ConnectionId = msg.ConnectionId;
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            return Ok(resp);
                        }
                        else
                        {
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewchatGroup(GroupchatViewModel model)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                Groupchat grpcrt = new Groupchat()
                {
                    GroupName = model.GroupName,
                    ParticipateId = model.ParticipateId,
                    ConnectionId = model.ConnectionId,
                    CreatedBy = model.CreatedBy,
                    Createddate = DateTime.UtcNow,
                };
                _db.Groupchat.Add(grpcrt);
                int i = await _db.SaveChangesAsync();
                if (i > 0)
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> LeaveChat(int Id, string ConnectionId)
        {
            ApiResponse resp = new ApiResponse();
            var leave = _db.Groupchat.Where(a => a.ParticipateId == Id && a.ConnectionId == ConnectionId).FirstOrDefault();
            if (leave != null)
            {
                _db.Groupchat.Remove(leave);
                int i = await _db.SaveChangesAsync();
                if (i > 0)
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = AppResource.user_delete;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.user_Failed;
                    return Ok(resp);
                }
            }
            else
            {
                resp.code = (int)ApiCustomResponseCode.Error;
                resp.msg = AppResource.invalid_model;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetGroupList(int Id)
        {
            Listofgroup data = new Listofgroup();
            try
            {
                data.list = (from item in _db.Groupchat.OrderByDescending(a => a.Createddate)
                             where item.ParticipateId == Id
                             select new GroupchatViewModel
                             {
                                 GroupName = item.GroupName,
                                 ParticipateId = Id,
                                 ConnectionId = item.ConnectionId,
                                 CreatedBy = item.CreatedBy,
                                 Count = _db.GroupMsgSeen.Where(a => a.ParticipateId == Id && a.MsgId == item.ConnectionId).Count()
                             }).ToList();

                if (data.list.Count > 0)
                {
                    data.list = data.list.GroupBy(x => x.ConnectionId).Select(x => x.First()).ToList();
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
                }
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                data.msg = AppResource.user_listE;
                data.code = (int)ApiCustomResponseCode.Error;
                return Ok(data);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> SeenGroupChat(int Id, string ConnectionId)
        {
            ApiResponse resp = new ApiResponse();
            try
            {
                var leave = _db.GroupMsgSeen.Where(a => a.ParticipateId == Id && a.MsgId == ConnectionId).ToList();
                if (leave != null)
                {
                    foreach (var item in leave)
                    {
                        _db.GroupMsgSeen.Remove(item);
                    }
                    await _db.SaveChangesAsync();
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = AppResource.user_delete;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.invalid_model;
                    return Ok(resp);
                }
            }
            catch(Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.Error;
                resp.msg = AppResource.invalid_model;
                return Ok(resp);
            }
           
        }
    }
}
