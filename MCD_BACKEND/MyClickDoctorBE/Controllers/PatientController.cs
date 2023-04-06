using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyClickDoctorBE.App_Resources;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using static MyClickDoctorBE.Models.AppCommonLogic;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class PatientController : ControllerBase
    {
        private readonly MyClickDoctorV4 _db;
        private readonly IHostingEnvironment _appEnvironment;
        public PatientController(MyClickDoctorV4 context, IHostingEnvironment appEnvironment)
        {
            _db = context;
            _appEnvironment = appEnvironment;
        }
        [HttpGet]
        public ActionResult GetPatientDashboard(int UserId)
        {
            GetPatDash data = new GetPatDash();
            try
            {
                data.GetData = (from pat in _db.Patient
                                where pat.UserId == UserId
                                select new PatientDashBoard
                                {
                                    Name = pat.SecondName + " " + pat.FirstName,
                                    WizardStatus = AppDbCommonLogic.GetwizardStatus(pat.Id),
                                    AllAppointments = _db.Appoinments.Where(a => a.PatientId == pat.Id && a.IsActive != 0).Count(),
                                    UpcomingAppointments = _db.Appoinments.Where(a => a.PatientId == pat.Id && a.IsActive != 0 && a.EndDate >= DateTime.UtcNow).Count(),
                                    MedicalReport = _db.Payments.Where(a => a.UserId == UserId && a.Status == 1 && a.InvoiceStatus == 1).Count(),
                                    TodayAppList = (from ta in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                    where ta.StartDate.Date == DateTime.UtcNow.Date && ta.PatientId == pat.Id && ta.IsActive != 0
                                                    select new BookPharmaApp
                                                    {
                                                        Id = ta.Id,
                                                        StartDate = ta.StartDate,
                                                        EndDate = ta.EndDate,
                                                        BookingDate = ta.BookingDate,
                                                        AppointmentTitle = ta.AppointmentTitle,
                                                        AppointmentId = ta.Appoinmentcode,
                                                        Duration = (int)ta.Duration,
                                                        Status = (int)ta.IsActive,
                                                        AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(ta.Id, ta.UserRole),
                                                        AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(ta.Id, ta.UserRole),
                                                        AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(ta.Id, ta.UserRole),
                                                        Role = (int)ta.UserRole,
                                                        MyRole = (int)ta.MyRole,
                                                        UserRole = (int)ta.UserRole,
                                                        CancelStatus = AppDbCommonLogic.GetCancelStatus(ta.Id, ta.EndDate),
                                                        UploadStatus = AppDbCommonLogic.getuploaddocumentstatus(ta.EndDate),
                                                        Expert = (from sp in _db.Doctorspecilization
                                                                  where sp.DocId == ta.DoctorId
                                                                  select new Specializion
                                                                  {
                                                                      Specialization = sp.Name
                                                                  }).ToList(),
                                                        Appointmenttype= AppDbCommonLogic.GetAppointmenttype(ta.DocSlotId)
                                                    }).ToList(),
                                    WeekAppList = (from week in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                   where week.EndDate >= DateTime.UtcNow && week.PatientId == pat.Id && week.IsActive != 0
                                                   select new BookPharmaApp
                                                   {
                                                       Id = week.Id,
                                                       StartDate = week.StartDate,
                                                       EndDate = week.EndDate,
                                                       BookingDate = week.BookingDate,
                                                       AppointmentTitle = week.AppointmentTitle,
                                                       AppointmentId = week.Appoinmentcode,
                                                       Duration = (int)week.Duration,
                                                       //DoctorId = week.Doctor.UserId,
                                                       //DoctorName = week.Doctor.FirstName + " " + week.Doctor.SecondName,
                                                       //DocImage = week.Doctor.PhotoUrl,
                                                       AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(week.Id, week.UserRole),
                                                       AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(week.Id, week.UserRole),
                                                       AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(week.Id, week.UserRole),
                                                       Role = (int)week.UserRole,
                                                       Status = (int)week.IsActive,
                                                       MyRole = (int)week.MyRole,
                                                       UserRole = (int)week.UserRole,
                                                       CancelStatus = AppDbCommonLogic.GetCancelStatus(week.Id, week.EndDate),
                                                       UploadStatus = AppDbCommonLogic.getuploaddocumentstatus(week.EndDate),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == week.DoctorId
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = sp.Name
                                                                 }).ToList(),
                                                       Appointmenttype = AppDbCommonLogic.GetAppointmenttype(week.DocSlotId)
                                                   }).ToList()
                                }).FirstOrDefault();

                if (data.GetData != null)
                {
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
                data.code = (int)ApiCustomResponseCode.BadRequest;
                data.msg = ex.Message;
                return Ok(data);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePatientProfile(PatientReg data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                var UserDetail = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (UserDetail != null)
                {
                    UserDetail.Email = data.Email;
                    UserDetail.PhoneNo = data.PhoneNo;
                }
                var PatientDetail = _db.Patient.Where(a => a.UserId == data.UserId).FirstOrDefault();
                if (PatientDetail != null)
                {
                    PatientDetail.FirstName = data.FirstName;
                    PatientDetail.SecondName = data.SecondName;
                    PatientDetail.ShortIntroduction = data.ShortIntroduction;
                    PatientDetail.Dob = data.Dob;
                    PatientDetail.Zipcode = data.Zipcode;
                    PatientDetail.City = data.City;
                    PatientDetail.StreetNumber = data.StreetNumber;
                    PatientDetail.AddressLine = data.AddressLine;
                    PatientDetail.Gender = data.Gender;
                    PatientDetail.SocialSecurityNumber = data.SocialSecurityNumber;
                    PatientDetail.CreatedAt = DateTime.UtcNow;
                    PatientDetail.Country = data.Country;
                    PatientDetail.State = data.State;
                    PatientDetail.CountryCode = data.CountryCode;
                }
                int j = await _db.SaveChangesAsync();
                AppCommonLogic.SendMailOnProfileUpdate(UserDetail.Email);
                if (j > 0)
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = AppResource.user_SuccessUpdate;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.user_Failed;
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
        [HttpPost]
        public async Task<IActionResult> UploadPatientProfilePic()
        {
            Upload Resp = new Upload();
            Resp.code = (int)ApiCustomResponseCode.BadRequest;
            Resp.msg = AppResource.invalid_model;
            var files = HttpContext.Request.Form.Files;
            int UserId = Convert.ToInt32(HttpContext.Request.Form["UserId"]);
            try
            {
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        //There is an error here
                        var uploads = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                        var Extetion = Path.GetExtension(files[0].FileName);
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                Resp.Path = "/Images/" + fileName;
                                var user = _db.Patient.Where(a => a.UserId == UserId).FirstOrDefault();
                                user.ProfileUrl = Resp.Path;
                                _db.SaveChanges();
                                Resp.code = (int)ApiCustomResponseCode.Ok;
                                Resp.msg = AppResource.user_SuccessUpdate;
                                return Ok(Resp);
                            }
                        }
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        Resp.msg = AppResource.user_Img;
                        return Ok(Resp);
                    }
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
            return Ok(Resp);
        }
        [HttpGet]
        public ActionResult GetPatientProfile(int UserId)
        {
            PatientProfile data = new PatientProfile();
            try
            {
                data.detail = (from patdet in _db.Patient
                               where patdet.UserId == UserId
                               select new PatientReg
                               {
                                   Id = patdet.Id,
                                   UserId = patdet.UserId,
                                   FirstName = patdet.FirstName,
                                   SecondName = patdet.SecondName,
                                   Gender = patdet.Gender,
                                   ProfileUrl = patdet.ProfileUrl,
                                   SocialSecurityNumber = patdet.SocialSecurityNumber,
                                   Dob = patdet.Dob,
                                   Email = _db.Users.Where(a => a.Id == patdet.UserId).FirstOrDefault().Email,
                                   PhoneNo = _db.Users.Where(a => a.Id == patdet.UserId).FirstOrDefault().PhoneNo,
                                   EmailAuth = _db.Users.Where(a => a.Id == patdet.UserId).FirstOrDefault().EmailAuth,
                                   SMSAuth = _db.Users.Where(a => a.Id == patdet.UserId).FirstOrDefault().SMSAuth,
                                   AddressLine = patdet.AddressLine,
                                   City = patdet.City,
                                   State = patdet.State,
                                   Zipcode = patdet.Zipcode,
                                   ShortIntroduction = patdet.ShortIntroduction,
                                   StreetNumber = patdet.StreetNumber,
                                   Status = patdet.Status,
                                   Country = patdet.Country,
                                   GDPR = (bool)patdet.GDPR,
                                   CountryCode = patdet.CountryCode
                               }).FirstOrDefault();
                if (data.detail != null)
                {
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
                data.code = (int)ApiCustomResponseCode.BadRequest;
                data.msg = ex.Message;
                return Ok(data);
            }
        }
        [HttpPost]
        public ActionResult BookPatientAppointment(BookPharmaApp data)
        {
            ptapp resp = new ptapp();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            string AppCode = "";
            var cnt = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault();
            int count = 0;
            if (cnt != null)
            {
                count = cnt.Id;
            }
            else
            {
                count = 1;
            }

            Random RN = new Random();
            AppCode = "DP" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow;
            DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
            int row = 0;
            int UserId = _db.Patient.Where(a => a.Id == data.PatientId).FirstOrDefault().UserId;
            try
            {
                if (ModelState.IsValid)
                {
                    var Docslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault();
                    var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.PatientId == data.PatientId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    var checkdoctorSlotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == Docslot.DocId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    if (GetUserTime > data.StartDate)
                    {
                        resp.msg = AppResource.appointment_booked;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
                    }
                    else
                    {
                        if (checkdoctorSlotbook == null)
                        {
                            if (Slotbook == null)
                            {
                                Appoinments obj = new Appoinments()
                                {
                                    StartDate = data.StartDate,
                                    EndDate = data.StartDate.AddMinutes(data.Duration),
                                    BookingDate = DateTime.UtcNow,
                                    AppointmentTitle = data.AppointmentTitle,
                                    Duration = data.Duration,
                                    DoctorId = Docslot.DocId,
                                    PatientId = data.PatientId,
                                    Appoinmentcode = AppCode,
                                    // IsActive = 1,
                                    IsActive = 0,
                                    DocSlotId = data.SlotId,
                                    UserRole = 2,
                                    MyRole = 4
                                };
                                _db.Appoinments.Add(obj);
                                row = _db.SaveChanges();
                                int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                AppDbCommonLogic.InsertPatientDoctorActivity(AppId, data.PatientId, Docslot.DocId);
                               // AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                if (row > 0)
                                {
                                    //var fillslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault();
                                    //var GetTime = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DocSlotId == data.SlotId).FirstOrDefault();
                                    //TimeSpan span = fillslot.FromTime.TimeOfDay - GetTime.EndDate.TimeOfDay;
                                    //int Duration = Math.Abs((int)span.TotalMinutes);
                                    //if (Duration < 15)
                                    //{
                                    //    fillslot.Status = 1;
                                    //    _db.SaveChanges();
                                    //}
                                    resp.msg = AppResource.appointment_success;
                                    resp.code = (int)ApiCustomResponseCode.Ok;
                                    resp.AppointmentId = AppId;
                                    return Ok(resp);
                                }
                                else
                                {
                                    resp.msg = AppResource.appointment_booked;
                                    resp.code = (int)ApiCustomResponseCode.Error;
                                    return Ok(resp);
                                }
                            }
                            else
                            {
                                resp.msg = AppResource.pharma_slot;
                                resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(resp);
                            }

                        }
                        else
                        {
                            resp.msg = AppResource.appointment_booked;
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                    }
                }
                else
                {
                    resp.msg = AppResource.invalid_model;
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
        [HttpGet]
        public ActionResult GetPateintAppointment(int UserId)
        {
            PharmaApp data = new PharmaApp();
            try
            {
                data.Dashboard = (from pt in _db.Patient
                                  where pt.UserId == UserId
                                  select new pharma
                                  {
                                      FirstName = pt.FirstName,
                                      SecondName = pt.SecondName,
                                      TodayAppList = (from today in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                      where today.StartDate.Date < DateTime.Now.Date && today.PatientId == pt.Id && today.IsActive != 0
                                                      select new BookPharmaApp
                                                      {
                                                          Id = today.Id,
                                                          StartDate = today.StartDate,
                                                          EndDate = today.EndDate,
                                                          BookingDate = today.BookingDate,
                                                          AppointmentTitle = today.AppointmentTitle,
                                                          AppointmentId = today.Appoinmentcode,
                                                          Duration = (int)today.Duration,
                                                          //DoctorId = today.Doctor.UserId,
                                                          //DoctorName = today.Doctor.FirstName + " " + today.Doctor.SecondName,
                                                          //DocImage = today.Doctor.PhotoUrl,
                                                          AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(today.Id, today.UserRole),
                                                          AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(today.Id, today.UserRole),
                                                          AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(today.Id, today.UserRole),
                                                          Role = (int)today.UserRole,
                                                          Status = (int)today.IsActive,
                                                          CancelStatus = AppDbCommonLogic.GetCancelStatus(today.Id, today.EndDate),
                                                          Expert = (from sp in _db.Doctorspecilization
                                                                    where sp.DocId == today.DoctorId
                                                                    select new Specializion
                                                                    {
                                                                        Specialization = sp.Name
                                                                    }).ToList()
                                                      }).ToList(),
                                      WeekAppList = (from week in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                     where week.StartDate.Date >= DateTime.UtcNow.Date && week.StartDate <= DateTime.UtcNow.Date.AddDays(7) && week.PatientId == pt.Id && week.IsActive != 0
                                                     select new BookPharmaApp
                                                     {
                                                         Id = week.Id,
                                                         StartDate = week.StartDate,
                                                         EndDate = week.EndDate,
                                                         BookingDate = week.BookingDate,
                                                         AppointmentTitle = week.AppointmentTitle,
                                                         AppointmentId = week.Appoinmentcode,
                                                         Duration = (int)week.Duration,
                                                         //DoctorId = week.Doctor.UserId,
                                                         //DoctorName = week.Doctor.FirstName + " " + week.Doctor.SecondName,
                                                         //DocImage = week.Doctor.PhotoUrl,
                                                         AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(week.Id, week.UserRole),
                                                         AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(week.Id, week.UserRole),
                                                         AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(week.Id, week.UserRole),
                                                         Role = (int)week.UserRole,
                                                         Status = (int)week.IsActive,
                                                         CancelStatus = AppDbCommonLogic.GetCancelStatus(week.Id, week.EndDate),
                                                         Expert = (from sp in _db.Doctorspecilization
                                                                   where sp.DocId == week.DoctorId
                                                                   select new Specializion
                                                                   {
                                                                       Specialization = sp.Name
                                                                   }).ToList()
                                                     }).ToList()
                                  }).FirstOrDefault();
                if (data.Dashboard != null)
                {
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
                data.msg = ex.Message;
                data.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(data);
            }
        }
        [HttpGet]
        public ActionResult GetPatientBookAppointment(int PatientId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            var Patientdata = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments.OrderBy(x => x.StartDate)
                                where appbk.PatientId == PatientId && appbk.IsActive != 0 && appbk.StartDate.Month >= DateTime.UtcNow.Month
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    //DoctorName = appbk.Doctor.FirstName,
                                    //DoctorId = appbk.Doctor.UserId,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.UserRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.UserRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.UserRole),
                                    Role = (int)appbk.UserRole,
                                    AppointmentId = appbk.Appoinmentcode,
                                   // Appointmenttype = "Doctor Appointment",
                                    Status = (int)appbk.IsActive,
                                    // DocImage = appbk.Doctor.PhotoUrl,
                                    UserId = AppCommonLogic.ToHexString(Patientdata.UserId.ToString()),
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate),
                                    Appointmenttype = AppDbCommonLogic.GetAppointmenttype(appbk.DocSlotId)
                                }).ToList();
                if (data.Appdata.Count != 0)
                {
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
                resp.msg = AppResource.invalid_model;
                resp.code = (int)ApiCustomResponseCode.Error;
                return Ok(resp);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddDoctorReview(Feedback data)
        {
            ApiResponse Resp = new ApiResponse();
            int row = 0;
            try
            {
                var app = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).FirstOrDefault();
                if (data.ParentId != 0)
                {
                    DoctorReview obj = new DoctorReview()
                    {
                        DocId = data.DocId,
                        FromUserId = data.FromUserId,
                        Type = data.Type,
                        Rating = 0,
                        Review = data.Review,
                        Created = DateTime.UtcNow,
                        ParentId = data.ParentId,
                        AppointmentId = app.Id
                    };
                    _db.DoctorReview.Add(obj);
                }
                else
                {
                    DoctorReview obj = new DoctorReview()
                    {
                        DocId = data.DocId,
                        FromUserId = data.FromUserId,
                        Type = data.Type,
                        Rating = data.Rating,
                        Review = data.Review,
                        Created = DateTime.UtcNow,
                        ParentId = 0,
                        AppointmentId = app.Id
                    };
                    _db.DoctorReview.Add(obj);
                }
                row = await _db.SaveChangesAsync();
                if (row > 0)
                {
                    Resp.code = (int)ApiCustomResponseCode.Ok;
                    Resp.msg = AppResource.user_feedback;
                    return Ok(Resp);
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    Resp.msg = AppResource.user_Failed;
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
        public ActionResult GetDoctorReview(int PatientId)
        {
            getreview data = new getreview();
            try
            {
                data.getlist = (from feed in _db.DoctorReview
                                where feed.FromUserId == PatientId && feed.Type == (int)UserType.Patient && feed.ParentId == 0
                                select new Feedback
                                {
                                    Id = feed.Id,
                                    DocId = feed.DocId,
                                    DocName = feed.Doc.SecondName + " " + feed.Doc.FirstName,
                                    DocImage = feed.Doc.PhotoUrl,
                                    Type = feed.Type,
                                    Rating = feed.Rating,
                                    Review = feed.Review,
                                    Created = feed.Created,
                                    //reply = (from rep in _db.DoctorReview
                                    //         where rep.ParentId == feed.Id
                                    //         select new Feedback
                                    //         {
                                    //             Id = rep.Id,
                                    //             Review = rep.Review
                                    //         }).ToList()
                                }).ToList();
                if (data.getlist.Count > 0)
                {
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
                data.code = (int)ApiCustomResponseCode.BadRequest;
                data.msg = ex.Message;
                return Ok(data);
            }
        }
        //[HttpGet]
        //public ActionResult GetPatientMorbidity(int PatientId)
        //{
        //    ApiResponse resp = new ApiResponse();
        //    resp.msg = AppResource.invalid_model;
        //    resp.code = (int)ApiCustomResponseCode.BadRequest;
        //    getpatmobidy data = new getpatmobidy();
        //    try
        //    {
        //        data.getlist = (from mrb in _db.Morbidity.OrderByDescending(x => x.Id)
        //                        where mrb.UserId == PatientId
        //                        select new PatMobidy
        //                        {
        //                            DiseaseName = mrb.DiseaseName,
        //                            Title = mrb.Title,
        //                            UserId = mrb.UserId,
        //                            Duration = mrb.Duration,
        //                            Symptoms = mrb.Symptoms,
        //                            Medicine = mrb.Medicine,
        //                            Created = (DateTime)mrb.Created
        //                        }).ToList();
        //        if (data.getlist.Count != 0)
        //        {
        //            data.msg = AppResource.user_listS;
        //            data.code = (int)ApiCustomResponseCode.Ok;
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            data.msg = AppResource.user_listE;
        //            data.code = (int)ApiCustomResponseCode.Error;
        //            return Ok(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.msg = ex.Message;
        //        resp.code = (int)ApiCustomResponseCode.Error;
        //        return Ok(resp);
        //    }
        //}
        [HttpPost]
        public ActionResult PaymentSuccessFailedStatus(paystatus data)
        {
            ApiResponse resp = new ApiResponse();
            int row = 0;
            try
            {
                if (data.Status == 0)
                {
                    row = AppDbCommonLogic.InsertPaymentWithFailedStatus(data.TransactionId);
                }
                else
                {
                    row = AppDbCommonLogic.InsertPaymentWithSuccessStatus(data.TransactionId);
                }
                if (row > 0)
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = AppResource.user_Added;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.user_Failed;
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
        [HttpGet]
        public ActionResult GetPatientDoctorsList(int PatientId)
        {
            GetSpeCat data = new GetSpeCat();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                data.SpecializationCategory = (from r in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                               where r.PatientId == PatientId && r.UserRole == 2 && r.MyRole == 4
                                               select new doctor
                                               {
                                                   Id = r.Doctor.Id,
                                                   UserId = r.Doctor.UserId,
                                                   FirstName = r.Doctor.SecondName + " " + r.Doctor.FirstName,
                                                   YearsOfExperiecne = r.Doctor.YearsOfExperiecne,
                                                   MedicalRegistrationNo = r.Doctor.MedicalRegistrationNo,
                                                   City = r.Doctor.City,
                                                   StreetNumber = r.Doctor.StreetNumber,
                                                   Zipcode = r.Doctor.Zipcode,
                                                   ContractualDoctorsCompany = r.Doctor.ContractualDoctorsCompany,
                                                   ContractStartDate = r.Doctor.ContractStartDate,
                                                   ContarctEndDate = r.Doctor.ContarctEndDate,
                                                   ShortIntroduction = r.Doctor.ShortIntroduction,
                                                   PhotoUrl = r.Doctor.PhotoUrl,
                                                   Status = r.Doctor.Status,
                                                   BussinessStatus = AppDbCommonLogic.GetBussinessStatus(r.Doctor.UserId),
                                                   DoctorType = (int)r.Doctor.DoctorType,
                                                   Expert = AppDbCommonLogic.GetDoctorSpeciality(r.Doctor.Id, Lang),
                                                   AppointmentTitle = r.AppointmentTitle,
                                                   LastVisitDate = r.StartDate,
                                                   Awailability = AppDbCommonLogic.CheckTimeawailabilityforpatient(r.Doctor.Id),
                                                   DocClinic = (from dc in _db.DoctorClinicMap
                                                                where dc.UserId == r.Doctor.UserId
                                                                select new DocCliMap
                                                                {
                                                                    ClinicName = dc.ClinicName,
                                                                    ClinicAddress = dc.ClinicAddress
                                                                }).ToList()
                                               }).ToList();
                data.SpecializationCategory = data.SpecializationCategory.Where(a => a.Status == 1).GroupBy(x => x.UserId).Select(x => x.First()).ToList();
              //  data.WizardStatus = AppDbCommonLogic.GetwizardStatusformydoctor(PatientId);
                if (data.SpecializationCategory.Count != 0)
                {
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
        [HttpPost]
        public async Task<ActionResult> AddMorbidity(PatMobidy data)
        {
            ApiResponse resp = new ApiResponse();
            var getapp = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).FirstOrDefault();
            var patient = _db.Patient.Where(a => a.Id == getapp.PatientId).FirstOrDefault();
            var user = _db.Users.Where(a => a.Id == patient.UserId).FirstOrDefault();
            int row = 0;
            try
            {
                Morbidity obj = new Morbidity()
                {
                    Anamnesis = new AES_ALGORITHM().Encrypt(data.anamnesis),
                    Diagnosis = new AES_ALGORITHM().Encrypt(data.diagnosis),
                    Therapy = new AES_ALGORITHM().Encrypt(data.therapy),
                    AppointmentId = getapp.Id,
                    Created = DateTime.UtcNow
                };
                _db.Morbidity.Add(obj);
                row = await _db.SaveChangesAsync();
                if (row > 0)
                {
                    AppCommonLogic.sendmorbiditytopatient(user.Email, patient.SecondName + " " + patient.FirstName, getapp.Id);
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = AppResource.user_Added;
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.user_Failed;
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
        [HttpGet]
        public ActionResult GetPatientMorbidity(int PatientId, int userType)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            try
            {
                if (userType == 4)
                {
                    data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                    where DateTime.UtcNow > appbk.StartDate && appbk.PatientId == PatientId && appbk.IsActive == 2 && appbk.DoctorId != null && appbk.DoctorId != 0 && appbk.UserRole == 2 && appbk.MyRole == 4
                                    //&& DateTime.UtcNow>appbk.EndDate 
                                    select new BookPharmaApp
                                    {
                                        Id = appbk.Id,
                                        StartDate = appbk.StartDate,
                                        EndDate = appbk.EndDate,
                                        BookingDate = appbk.BookingDate,
                                        AppointmentTitle = appbk.AppointmentTitle,
                                        Duration = (int)appbk.Duration,
                                        DoctorName = appbk.Doctor.SecondName + " " + appbk.Doctor.FirstName,
                                        DoctorId = appbk.Doctor.UserId,
                                        DocImage = appbk.Doctor.PhotoUrl,
                                        AppointmentId = appbk.Appoinmentcode,
                                        PatName = appbk.Patient.SecondName + " " + appbk.Patient.FirstName,
                                        PatImage = appbk.Patient.ProfileUrl,
                                        Appointmenttype = "Doctor Appointment",
                                        Status = (int)appbk.IsActive,
                                        //UserId = AppCommonLogic.ToHexString(Patientdata.UserId.ToString()),
                                        MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                        CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate),
                                        Expert = (from sp in _db.Doctorspecilization
                                                  where sp.DocId == appbk.DoctorId && sp.Status == 1 && sp.Name != null
                                                  select new Specializion
                                                  {
                                                      Specialization = sp.Name
                                                  }).ToList(),
                                        DocClinic = (from dc in _db.DoctorClinicMap
                                                     where dc.UserId == appbk.Doctor.UserId
                                                     select new DocCliMap
                                                     {
                                                         ClinicName = dc.ClinicName,
                                                         ClinicAddress = dc.ClinicAddress
                                                     }).ToList()
                                    }).ToList();
                }
                else if (userType == 2)
                {
                    data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                    where DateTime.UtcNow > appbk.StartDate && appbk.DoctorId == PatientId && appbk.IsActive == 2 && appbk.PatientId != null && appbk.PatientId != 0 && appbk.UserRole == 2 && appbk.MyRole == 4
                                    //&& DateTime.UtcNow>appbk.EndDate 
                                    select new BookPharmaApp
                                    {
                                        Id = appbk.Id,
                                        StartDate = appbk.StartDate,
                                        EndDate = appbk.EndDate,
                                        BookingDate = appbk.BookingDate,
                                        AppointmentTitle = appbk.AppointmentTitle,
                                        Duration = (int)appbk.Duration,
                                        DoctorName = appbk.Doctor.SecondName + " " + appbk.Doctor.FirstName,
                                        DoctorId = appbk.Doctor.UserId,
                                        DocImage = appbk.Doctor.PhotoUrl,
                                        AppointmentId = appbk.Appoinmentcode,
                                        PatName = appbk.Patient.SecondName + " " + appbk.Patient.FirstName,
                                        PatImage = appbk.Patient.ProfileUrl,
                                        Appointmenttype = "Doctor Appointment",
                                        Status = (int)appbk.IsActive,
                                        //UserId = AppCommonLogic.ToHexString(Patientdata.UserId.ToString()),
                                        MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                        CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate),
                                        Expert = (from sp in _db.Doctorspecilization
                                                  where sp.DocId == appbk.DoctorId && sp.Status == 1 && sp.Name != null
                                                  select new Specializion
                                                  {
                                                      Specialization = sp.Name
                                                  }).ToList(),
                                        DocClinic = (from dc in _db.DoctorClinicMap
                                                     where dc.UserId == appbk.Doctor.UserId
                                                     select new DocCliMap
                                                     {
                                                         ClinicName = dc.ClinicName,
                                                         ClinicAddress = dc.ClinicAddress
                                                     }).ToList()
                                    }).ToList();
                }
                if (data.Appdata.Count != 0)
                {
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
                resp.msg = AppResource.invalid_model;
                resp.code = (int)ApiCustomResponseCode.Error;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetPatientMorbidityDetail(int AppointmentId, int Id, int UserType)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            getpatmobidy data = new getpatmobidy();
            var appdet = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            try
            {
                if (UserType == 4)
                {
                    if (Id == appdet.PatientId)
                    {
                        data.getlist = (from mordet in _db.Morbidity
                                        where mordet.AppointmentId == AppointmentId
                                        select new PatMobidy
                                        {
                                            anamnesis = new AES_ALGORITHM().Decrypt(mordet.Anamnesis),
                                            diagnosis = new AES_ALGORITHM().Decrypt(mordet.Diagnosis),
                                            therapy = new AES_ALGORITHM().Decrypt(mordet.Therapy),
                                            AppointmentId = mordet.AppointmentId,
                                            Created = (DateTime)mordet.Created,
                                            Expert = AppDbCommonLogic.GetDoctorSpecility(AppointmentId),
                                            DocClinic = AppDbCommonLogic.GetDoctorWorkPlace(appdet.DoctorId),
                                            DoctorName = AppDbCommonLogic.GetDoctorName(appdet.DoctorId),
                                            PatientName = AppDbCommonLogic.GetPatientName(appdet.PatientId)
                                        }).FirstOrDefault();
                    }
                }
                else if (UserType == 2)
                {

                    data.getlist = (from mordet in _db.Morbidity
                                    where mordet.AppointmentId == AppointmentId
                                    select new PatMobidy
                                    {
                                        anamnesis = new AES_ALGORITHM().Decrypt(mordet.Anamnesis),
                                        diagnosis = new AES_ALGORITHM().Decrypt(mordet.Diagnosis),
                                        therapy = new AES_ALGORITHM().Decrypt(mordet.Therapy),
                                        AppointmentId = mordet.AppointmentId,
                                        Created = (DateTime)mordet.Created,
                                        Expert = AppDbCommonLogic.GetDoctorSpecility(AppointmentId),
                                        DocClinic = AppDbCommonLogic.GetDoctorWorkPlace(appdet.DoctorId),
                                        DoctorName = AppDbCommonLogic.GetDoctorName(appdet.DoctorId),
                                        PatientName = AppDbCommonLogic.GetPatientName(appdet.PatientId)
                                    }).FirstOrDefault();

                }

                if (data.getlist != null)
                {
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
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetPatientMorbidityDetails(int AppointmentId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            getpatmobidy data = new getpatmobidy();
            var appdet = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            try
            {

                data.getlist = (from mordet in _db.Morbidity
                                where mordet.AppointmentId == AppointmentId
                                select new PatMobidy
                                {
                                    anamnesis = new AES_ALGORITHM().Decrypt(mordet.Anamnesis),
                                    diagnosis = new AES_ALGORITHM().Decrypt(mordet.Diagnosis),
                                    therapy = new AES_ALGORITHM().Decrypt(mordet.Therapy),
                                    AppointmentId = mordet.AppointmentId,
                                    Created = (DateTime)mordet.Created,
                                    Expert = AppDbCommonLogic.GetDoctorSpecility(AppointmentId),
                                    DocClinic = AppDbCommonLogic.GetDoctorWorkPlace(appdet.DoctorId),
                                    DoctorName = AppDbCommonLogic.GetDoctorName(appdet.DoctorId),
                                    PatientName = AppDbCommonLogic.GetPatientName(appdet.PatientId)
                                }).FirstOrDefault();

                if (data.getlist != null)
                {
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
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetPatientReviewDetail(int AppointmentId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            getreviewdetail data = new getreviewdetail();
            try
            {
                data.getlist = (from review in _db.DoctorReview
                                where review.AppointmentId == AppointmentId
                                select new Feedback
                                {
                                    AppointmentId = review.AppointmentId,
                                    Created = (DateTime)review.Created,
                                    //Expert = AppDbCommonLogic.GetDoctorSpecility(AppointmentId),
                                    //DocClinic = AppDbCommonLogic.GetDoctorWorkPlace(review.DocId),
                                    DocName = AppDbCommonLogic.GetDoctorName(review.DocId),
                                    Rating = review.Rating,
                                    Review = review.Review
                                }).FirstOrDefault();
                if (data.getlist != null)
                {
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
                return Ok(resp);
            }
        }
        //[HttpGet]
        //public ActionResult GetPatientPaymentHistory(int PatientId)
        //{
        //    GetSpeCat data = new GetSpeCat();
        //    try
        //    {
        //        data.SpecializationCategory = (from r in _db.Appoinments
        //                                       where r.PatientId == PatientId && r.UserRole==2 && r.IsActive != 0
        //                                       select new doctor
        //                                       {
        //                                           Id = r.Doctor.Id,
        //                                           UserId = r.Doctor.UserId,
        //                                           FirstName = r.Doctor.FirstName + " " + r.Doctor.SecondName,
        //                                           PhotoUrl = r.Doctor.PhotoUrl,
        //                                           Status = r.IsActive,
        //                                           Duration= (int)r.Duration,
        //                                           AppointmentTitle = r.AppointmentTitle,
        //                                           LastVisitDate = r.StartDate,
        //                                           Amount = (int)_db.Payments.Where(a=>a.AppoinmentId==r.Id).FirstOrDefault().Amount,
        //                                           PaymentDate= (DateTime)_db.Payments.Where(a => a.AppoinmentId == r.Id).FirstOrDefault().PaymentDate
        //                                       }).ToList();
        //        if (data.SpecializationCategory.Count != 0)
        //        {
        //            data.msg = AppResource.user_listS;
        //            data.code = (int)ApiCustomResponseCode.Ok;
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            data.msg = AppResource.user_listE;
        //            data.code = (int)ApiCustomResponseCode.Error;
        //            return Ok(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        data.msg = AppResource.user_listE;
        //        data.code = (int)ApiCustomResponseCode.Error;
        //        return Ok(data);
        //    }
        //}
        [HttpGet]
        public ActionResult GetPatientAppointmentDetail(int Id, int UserId, int Type)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            AdminAPPOINTDetail obj = new AdminAPPOINTDetail();
            try
            {
                obj.data = (from r in _db.Appoinments
                            where r.Id == Id
                            select new BookPharmaApp
                            {
                                Id = r.Id,
                                StartDate = r.StartDate,
                                EndDate = r.EndDate,
                                BookingDate = r.BookingDate,
                                AppointmentTitle = r.AppointmentTitle,
                                AppointmentId = r.Appoinmentcode,
                                Duration = (int)r.Duration,
                                Description = r.Description,
                                //DoctorId = r.Doctor.UserId,
                                //DoctorName = r.Doctor.FirstName + " " + r.Doctor.SecondName,
                                //DocImage = r.Doctor.PhotoUrl,
                                //  PharmaRepId = r.PharmaRep.UserId,
                                //  RepName = r.PharmaRep.Name,
                                MyRole = (int)r.MyRole,
                                UserRole = (int)r.UserRole,
                                AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(r.Id, r.UserRole),
                                AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(r.Id, r.UserRole),
                                AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(r.Id, r.UserRole),
                                Role = (int)r.UserRole,
                                PatientId = (int)r.PatientId,
                                PatImage = _db.Patient.Where(a => a.Id == r.PatientId).FirstOrDefault().ProfileUrl,
                                PatName = _db.Patient.Where(a => a.Id == r.PatientId).FirstOrDefault().FirstName,
                                Status = (int)r.IsActive,
                                UserId = AppCommonLogic.ToHexString(UserId.ToString()),
                                Type = Type,
                                MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(r.StartDate, r.EndDate, r.Id),
                                CancelStatus = AppDbCommonLogic.GetCancelStatus(r.Id, r.EndDate),
                                UploadStatus = AppDbCommonLogic.getuploaddocumentstatus(r.EndDate),
                            }).FirstOrDefault();
                if (obj.data != null)
                {
                    obj.msg = AppResource.user_listS;
                    obj.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(obj);
                }
                else
                {
                    obj.msg = AppResource.user_listE;
                    obj.code = (int)ApiCustomResponseCode.Error;
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(resp);
            }
        }
        [HttpPost]
        public ActionResult CancelPateintMeeting(meeting data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                var meetdata = _db.Appoinments.Where(a => a.Id == data.Id).FirstOrDefault();
                if (meetdata != null)
                {
                    meetdata.IsActive = data.Status;
                    _db.SaveChangesAsync();
                    AppDbCommonLogic.SendCancelPatientMeetingEmailToUser(data.Id, data.Type);
                    AppDbCommonLogic.CancelMeetingActivity(data.Id);
                    resp.msg = AppResource.user_cancel;
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(resp);
                }
                else
                {
                    resp.msg = AppResource.user_Failed;
                    resp.code = (int)ApiCustomResponseCode.BadRequest;
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
        [HttpGet]
        public ActionResult GetPatientPaymentHistory(int PatientId, int Month, int Year)
        {
            string Lang = "";
            Lang = Request.Headers["Lang"];
            DocConsult data = new DocConsult();
            try
            {
                if (Month == 0 && Year == 0)
                {
                    data.ConsultList = (from r in _db.Appoinments
                                        where r.PatientId == PatientId && r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Doctor.Id,
                                            UserId = r.Doctor.UserId,
                                            FirstName = r.Doctor.SecondName + " " + r.Doctor.FirstName,
                                            PhotoUrl = r.Doctor.PhotoUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = AppDbCommonLogic.GetPatientAmount(r.Id),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang)
                                        }).ToList();
                    data.Amount = AppDbCommonLogic.PatientTotalAmount(PatientId);
                    if (data.ConsultList.Count != 0)
                    {
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
                else if (Month != 0 && Year != 0)
                {
                    data.ConsultList = (from r in _db.Appoinments
                                        where r.PatientId == PatientId && r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.BookingDate.Month == Month && r.BookingDate.Year == Year && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Doctor.Id,
                                            UserId = r.Doctor.UserId,
                                            FirstName = r.Doctor.SecondName + " " + r.Doctor.FirstName,
                                            PhotoUrl = r.Doctor.PhotoUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = AppDbCommonLogic.GetPatientAmount(r.Id),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang)
                                        }).ToList();
                    data.Amount = AppDbCommonLogic.PatientMonthyaerTotalAmount(PatientId, Month, Year);
                    if (data.ConsultList.Count != 0)
                    {
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
                else if (Month == 0 && Year != 0)
                {
                    data.ConsultList = (from r in _db.Appoinments
                                        where r.PatientId == PatientId && r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.BookingDate.Year == Year && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Doctor.Id,
                                            UserId = r.Doctor.UserId,
                                            FirstName = r.Doctor.SecondName + " " + r.Doctor.FirstName,
                                            PhotoUrl = r.Doctor.PhotoUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = AppDbCommonLogic.GetPatientAmount(r.Id),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang)
                                        }).ToList();
                    data.Amount = AppDbCommonLogic.PatientYearTotalAmount(PatientId, Year);
                    if (data.ConsultList.Count != 0)
                    {
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
                else if (Month != 0 && Year == 0)
                {
                    data.ConsultList = (from r in _db.Appoinments
                                        where r.PatientId == PatientId && r.MyRole == 4 && r.IsActive != 0 && r.BookingDate.Month == Month && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Doctor.Id,
                                            UserId = r.Doctor.UserId,
                                            FirstName = r.Doctor.SecondName + " " + r.Doctor.FirstName,
                                            PhotoUrl = r.Doctor.PhotoUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = AppDbCommonLogic.GetPatientAmount(r.Id),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang)
                                        }).ToList();
                    data.Amount = AppDbCommonLogic.PatientMonthTotalAmount(PatientId, Month);
                    if (data.ConsultList.Count != 0)
                    {
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
            }

            catch (Exception ex)
            {
                data.msg = AppResource.user_listE;
                data.code = (int)ApiCustomResponseCode.Error;
                return Ok(data);
            }
            return Ok(data);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetOurHCP(int Awialabilty, int Category, string Name)
        {
            GetSpeCat data = new GetSpeCat();
            List<doctor> getlist = new List<doctor>();
            List<Doctorspecilization> catlist = new List<Doctorspecilization>();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                //if (Category != 0)
                //{
                //    catlist = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.CatId == Category && a.Doc.Status == 1).ToList();
                //}
                //else
                if (Name != null && Category == 0)
                {
                    catlist = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.Doc.Status == 1 && (a.Doc.FirstName.Contains(Name) || a.Doc.SecondName.Contains(Name)) || (a.Doc.FirstName + " " + a.Doc.SecondName).ToLower().Contains(Name)).ToList();
                }
                else if (Name != null && Category != 0)
                {
                    catlist = _db.Doctorspecilization.Include(y => y.Doc).Where(y => (y.Doc.Status == 1 && y.CatId == Category) && (y.Doc.FirstName.Contains(Name) || y.Doc.SecondName.Contains(Name))).ToList();
                }
                else if (Name == null && Category != 0)
                {
                    catlist = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.CatId == Category && a.Doc.Status == 1).ToList();
                }
                else
                {
                    catlist = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.Doc.Status == 1).ToList();
                }
                if (catlist.Count != 0)
                {
                    data.SpecializationCategory = (from r in catlist
                                                   select new doctor
                                                   {
                                                       Id = r.Doc.Id,
                                                       UserId = r.Doc.UserId,
                                                       FirstName = r.Doc.SecondName + " " + r.Doc.FirstName,
                                                       YearsOfExperiecne = r.Doc.YearsOfExperiecne,
                                                       MedicalRegistrationNo = r.Doc.MedicalRegistrationNo,
                                                       City = r.Doc.City,
                                                       StreetNumber = r.Doc.StreetNumber,
                                                       Zipcode = r.Doc.Zipcode,
                                                       ContractualDoctorsCompany = r.Doc.ContractualDoctorsCompany,
                                                       ContractStartDate = r.Doc.ContractStartDate,
                                                       ContarctEndDate = r.Doc.ContarctEndDate,
                                                       ShortIntroduction = r.Doc.ShortIntroduction,
                                                       PhotoUrl = r.Doc.PhotoUrl,
                                                       PhoneNo = r.Doc.PhoneNo,
                                                       Status = r.Doc.Status,
                                                       DoctorConslutationCount = r.Doc.DoctorConslutationCount,
                                                       DoctorConsultation = r.Doc.DoctorConsultation,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(r.Doc.UserId),
                                                       Expert = AppDbCommonLogic.GetDoctorSpeciality(r.Doc.Id, Lang),
                                                       DocExperiance = (from sp in _db.Workplace
                                                                        where sp.UserId == r.Doc.UserId
                                                                        select new experinace
                                                                        {
                                                                            WorkplaceTitle = sp.WorkplaceTitle,
                                                                            AddressLine = sp.AddressLine
                                                                        }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctorAwailablity(Awialabilty, r.Doc.Id),
                                                       DoctorType = (int)r.Doc.DoctorType,
                                                       Price = AppDbCommonLogic.GetDoctorPrice(r.Doc.DoctorType)
                                                   }).ToList();
                    if (Awialabilty != 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.Awailability == 1 && a.Status == 1).ToList();
                    }
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.Status == 1).GroupBy(x => x.UserId).Select(x => x.First()).ToList();
                    }
                    data.DoctorCount = data.SpecializationCategory.Count();
                    if (data.SpecializationCategory.Count() > 0)
                    {
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
                else
                {
                    data.msg = AppResource.user_listE;
                    data.code = (int)ApiCustomResponseCode.Error;
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                data.msg = ex.Message;
                data.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(data);
            }
        }
        [HttpPost]
        public ActionResult UpdateWizardStatus(Updatetime data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                var user = _db.Patient.Where(a => a.UserId == data.UserId).FirstOrDefault();
                if (user != null)
                {
                    user.WizardStatus = 1;
                }
                _db.SaveChanges();
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = AppResource.user_update;
                return Ok(Resp);
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.Error;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public ActionResult GetPatientActivityList(int PatientId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            try
            {

                data.Appdata = (from appbk in _db.ActivityList.OrderByDescending(x => x.Id)
                                where appbk.PatientId == PatientId
                                select new BookPharmaApp
                                {
                                    Id = appbk.Appointment.Id,
                                    StartDate = appbk.Appointment.StartDate,
                                    EndDate = appbk.Appointment.EndDate,
                                    BookingDate = appbk.Appointment.BookingDate,
                                    AppointmentTitle = appbk.Appointment.AppointmentTitle,
                                    Duration = (int)appbk.Appointment.Duration,
                                    //DoctorName = appbk.Appointment.Doctor.FirstName + " " + appbk.Appointment.Doctor.SecondName,
                                    //DoctorId = appbk.Appointment.Doctor.UserId,
                                    //DocImage = appbk.Appointment.Doctor.PhotoUrl,
                                    AppointmentId = appbk.Appointment.Appoinmentcode,
                                    //  Appointmenttype = "Pharma Representative",
                                    Status = appbk.Status,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Appointment.Id, appbk.Appointment.UserRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Appointment.Id, appbk.Appointment.UserRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Appointment.Id, appbk.Appointment.UserRole),
                                    Role = (int)appbk.Appointment.MyRole,
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Appointment.Id, appbk.Appointment.EndDate)
                                }).ToList();
                if (data.Appdata.Count != 0)
                {
                    data.Appdata = data.Appdata.GroupBy(x => new { x.Id, x.Status }).Select(x => x.First()).ToList();
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
                resp.msg = AppResource.invalid_model;
                resp.code = (int)ApiCustomResponseCode.Error;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetNurseDoctorSuggestion(int PatientId)
        {
            nursedoctorsuggestion data = new nursedoctorsuggestion();
            try
            {

                data.list = (from today in _db.Appoinments.OrderByDescending(a => a.StartDate)
                             where today.PatientId == PatientId && today.NurseSuggestionStatus == 1
                             && today.NurseId != 0 && today.NurseId != null
                             select new nurseSuggetion
                             {
                                 StartDate = today.StartDate,
                                 EndDate = today.EndDate,
                                 PatientName = today.Patient.SecondName + " " + today.Patient.FirstName,
                                 NurseName = today.Nurse.SecondName + " " + today.Nurse.FirstName,
                                 Symptoms = today.Description
                             }).ToList();
                if (data.list.Count > 0)
                {
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
                data.msg = ex.Message;
                data.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(data);
            }
        }
        [HttpPost]
        public ActionResult CancelPateintMeetingbefore12Hours(meeting data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                var meetdata = _db.Appoinments.Where(a => a.Id == data.Id).FirstOrDefault();
                if (meetdata != null)
                {
                    meetdata.IsActive = data.Status;
                    _db.SaveChangesAsync();
                    AppDbCommonLogic.SendCancelPatientMeetingEmailToUser(data.Id, data.Type);
                    AppDbCommonLogic.CancelMeetingActivity(data.Id);
                    AppDbCommonLogic.UpdatePaymentHalfRefundStatus(data.Id);
                    resp.msg = AppResource.user_cancel;
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(resp);
                }
                else
                {
                    resp.msg = AppResource.user_Failed;
                    resp.code = (int)ApiCustomResponseCode.BadRequest;
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
        [HttpPost]
        public ActionResult BookPatientPartialAppointment(BookPharmaApp data)
        {
            ptapp resp = new ptapp();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            string AppCode = "";
            var cnt = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault();
            int count = 0;
            if (cnt != null)
            {
                count = cnt.Id;
            }
            else
            {
                count = 1;
            }
            Random RN = new Random();
            AppCode = "DPP" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow;
            DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
            int row = 0;
            var Patientdata = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).FirstOrDefault();
            var Docslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.PatientId == data.PatientId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    var DoctorSlotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.PatientId == Docslot.DocId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();

                    if (GetUserTime > data.StartDate)
                    {
                        resp.msg = AppResource.appointment_booked;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
                    }
                    else
                    {
                        if (Patientdata.PatientId == null && Patientdata.PatientId == 0)
                        {
                            resp.msg = AppResource.partial_appointment;
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                        else
                        {
                            if (Slotbook == null)
                            {
                                if (DoctorSlotbook == null)
                                {
                                    Appoinments obj = new Appoinments()
                                    {
                                        StartDate = data.StartDate,
                                        EndDate = data.StartDate.AddMinutes(data.Duration),
                                        BookingDate = DateTime.UtcNow,
                                        AppointmentTitle = data.AppointmentTitle,
                                        Duration = data.Duration,
                                        DoctorId = Docslot.DocId,
                                        PatientId = Patientdata.PatientId,
                                        Appoinmentcode = AppCode,
                                        IsActive = 3,
                                        // IsActive = 1,
                                        DocSlotId = data.SlotId,
                                        UserRole = 2,
                                        MyRole = 4
                                    };
                                    _db.Appoinments.Add(obj);
                                    row = _db.SaveChanges();
                                    int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                    AppDbCommonLogic.InsertPatientDoctorActivity(AppId, (int)Patientdata.PatientId, Docslot.DocId);
                                    AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                    if (row > 0)
                                    {
                                        var fillslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault();
                                        var GetTime = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DocSlotId == data.SlotId).FirstOrDefault();
                                        TimeSpan span = fillslot.FromTime.TimeOfDay - GetTime.EndDate.TimeOfDay;
                                        int Duration = Math.Abs((int)span.TotalMinutes);
                                        if (Duration < 15)
                                        {
                                            fillslot.Status = 1;
                                            _db.SaveChanges();
                                        }
                                        resp.msg = AppResource.appointment_success;
                                        resp.code = (int)ApiCustomResponseCode.Ok;
                                        resp.AppointmentId = AppId;
                                        return Ok(resp);
                                    }
                                    else
                                    {
                                        resp.msg = AppResource.appointment_booked;
                                        resp.code = (int)ApiCustomResponseCode.Error;
                                        return Ok(resp);
                                    }
                                }
                                else
                                {
                                    resp.msg = AppResource.pharma_slot;
                                    resp.code = (int)ApiCustomResponseCode.Error;
                                    return Ok(resp);
                                }

                            }
                            else
                            {
                                resp.msg = AppResource.appointment_booked;
                                resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(resp);
                            }
                        }
                    }
                }
                else
                {
                    resp.msg = AppResource.invalid_model;
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
        [HttpPost]
        public ActionResult BookPatientAppointmentwithoutpay(BookPharmaApp data)
        {
            ptapp resp = new ptapp();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            string AppCode = "";
            var cnt = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault();
            int count = 0;
            if (cnt != null)
            {
                count = cnt.Id;
            }
            else
            {
                count = 1;
            }

            Random RN = new Random();
            AppCode = "DP" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow;
            DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
            int row = 0;
            int UserId = _db.Patient.Where(a => a.Id == data.PatientId).FirstOrDefault().UserId;
            try
            {
                if (ModelState.IsValid)
                {
                    var Docslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault();
                    var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.PatientId == data.PatientId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    var checkdoctorSlotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == Docslot.DocId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    if (GetUserTime > data.StartDate)
                    {
                        resp.msg = AppResource.appointment_booked;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
                    }
                    else
                    {
                        if (checkdoctorSlotbook == null)
                        {
                            if (Slotbook == null)
                            {
                                Appoinments obj = new Appoinments()
                                {
                                    StartDate = data.StartDate,
                                    EndDate = data.StartDate.AddMinutes(data.Duration),
                                    BookingDate = DateTime.UtcNow,
                                    AppointmentTitle = data.AppointmentTitle,
                                    Duration = data.Duration,
                                    DoctorId = Docslot.DocId,
                                    PatientId = data.PatientId,
                                    Appoinmentcode = AppCode,
                                     IsActive = 1,
                                    DocSlotId = data.SlotId,
                                    UserRole = 2,
                                    MyRole = 4
                                };
                                _db.Appoinments.Add(obj);
                                row = _db.SaveChanges();
                                int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                AppDbCommonLogic.InsertPatientDoctorActivity(AppId, data.PatientId, Docslot.DocId);
                                AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                if (row > 0)
                                {
                                    resp.msg = AppResource.appointment_success;
                                    resp.code = (int)ApiCustomResponseCode.Ok;
                                    resp.AppointmentId = AppId;
                                    return Ok(resp);
                                }
                                else
                                {
                                    resp.msg = AppResource.appointment_booked;
                                    resp.code = (int)ApiCustomResponseCode.Error;
                                    return Ok(resp);
                                }
                            }
                            else
                            {
                                resp.msg = AppResource.pharma_slot;
                                resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(resp);
                            }

                        }
                        else
                        {
                            resp.msg = AppResource.appointment_booked;
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                    }
                }
                else
                {
                    resp.msg = AppResource.invalid_model;
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
    }
}
