using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyClickDoctorBE.App_Resources;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using static MyClickDoctorBE.Models.AppCommonLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using BarionClientLibrary;
using Microsoft.Extensions.Logging;
using BarionClientLibrary.Operations.Refund;
using Newtonsoft.Json;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class DoctorController : ControllerBase
    {
        private readonly MyClickDoctorV4 _db;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly BarionClient _barionClient;
        private readonly BarionSettings _barionSettings;
        private readonly ILogger<DoctorController> _log;
        public DoctorController(MyClickDoctorV4 context, IHostingEnvironment appEnvironment, BarionClient barionClient, BarionSettings barionSettings, ILogger<DoctorController> log)
        {
            _db = context;
            _appEnvironment = appEnvironment;
            _barionClient = barionClient;
            _barionSettings = barionSettings;
            _log = log;
        }
        [HttpPost]
        public async Task<IActionResult> PostDoctorProfile([FromBody] doctor data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int ClinicId = 0;
            try
            {
                var doc = _db.Doctors.Where(a => a.UserId == data.UserId).FirstOrDefault();
                if (doc != null)
                {
                    doc.FirstName = data.FirstName;
                    doc.SecondName = data.SecondName;
                    doc.PhoneNo = data.PhoneNo;
                    doc.City = data.City;
                    doc.StreetNumber = data.StreetNumber;
                    doc.Zipcode = data.Zipcode;
                    doc.ShortIntroduction = data.ShortIntroduction;
                    doc.MedicalRegistrationNo = data.MedicalRegistrationNo;
                    doc.PhotoUrl = data.PhotoUrl;
                    if (data.DoctorType != 0)
                    {
                        doc.DoctorType = data.DoctorType;
                    }
                    await _db.SaveChangesAsync();
                }
                if (data.Expert.Count > 0)
                {
                    List<SpecializationDetailsAndDiploma> tw = new List<SpecializationDetailsAndDiploma>();
                    tw = _db.SpecializationDetailsAndDiploma.Where(x => x.UserId == data.UserId).ToList();
                    for (int k = 0; k < data.Expert.Count(); k++)
                    {
                        var sp = tw.Skip(k).FirstOrDefault();
                        if (sp != null)
                        {
                            sp.YearOfDiploma = data.Expert[k].YearOfDiploma;
                            sp.NameofDegree = data.Expert[k].NameofDegree;
                            sp.University = data.Expert[k].University;
                            sp.Specialization = data.Expert[k].Specialization;
                            sp.Remark = data.Expert[k].Remark;
                            sp.UserId = data.UserId;
                        }
                        else
                        {
                            tw.Add(new SpecializationDetailsAndDiploma
                            {
                                YearOfDiploma = data.Expert[k].YearOfDiploma,
                                NameofDegree = data.Expert[k].NameofDegree,
                                University = data.Expert[k].University,
                                Specialization = data.Expert[k].Specialization,
                                Remark = data.Expert[k].Remark,
                                UserId = data.UserId
                            });
                        }
                    }
                    int i = await _db.SaveChangesAsync();
                }
                if (data.DaysWeek.Count > 0)
                {
                    List<DoctorNurseAvailability> gtlst = new List<DoctorNurseAvailability>();
                    gtlst = _db.DoctorNurseAvailability.Where(x => x.UserId == data.UserId).ToList();
                    for (int j = 0; j < data.DaysWeek.Count(); j++)
                    {
                        var sp = gtlst.Skip(j).FirstOrDefault();
                        if (sp != null)
                        {
                            sp.StartTime = data.DaysWeek[j].StartTime;
                            sp.EndTime = data.DaysWeek[j].EndTime;
                            sp.DayofWeek = data.DaysWeek[j].DayofWeek;
                            sp.UserId = data.UserId;
                        }
                        else
                        {
                            DoctorNurseAvailability ds = new DoctorNurseAvailability()
                            {
                                StartTime = data.DaysWeek[j].StartTime,
                                EndTime = data.DaysWeek[j].EndTime,
                                DayofWeek = data.DaysWeek[j].DayofWeek,
                                UserId = data.UserId
                            };
                            _db.DoctorNurseAvailability.Add(ds);
                        }

                    }
                    await _db.SaveChangesAsync();
                }
                if (data.DocExperiance.Count > 0)
                {
                    List<Workplace> gtwrk = new List<Workplace>();
                    gtwrk = _db.Workplace.Where(x => x.UserId == data.UserId).ToList();
                    for (int n = 0; n < data.DocExperiance.Count(); n++)
                    {
                        var wp = gtwrk.Skip(n).FirstOrDefault();
                        if (wp != null)
                        {
                            wp.WorkplaceTitle = data.DocExperiance[n].WorkplaceTitle;
                            wp.StartDate = data.DocExperiance[n].StartDate;
                            wp.EndDate = data.DocExperiance[n].EndDate;
                            wp.Description = data.DocExperiance[n].Description;
                            wp.UserId = data.UserId;
                            wp.AddressLine = data.DocExperiance[n].AddressLine;
                        }
                        else
                        {
                            Workplace wrk = new Workplace()
                            {
                                WorkplaceTitle = data.DocExperiance[n].WorkplaceTitle,
                                StartDate = data.DocExperiance[n].StartDate,
                                EndDate = data.DocExperiance[n].EndDate,
                                Description = data.DocExperiance[n].Description,
                                UserId = data.UserId,
                                AddressLine = data.DocExperiance[n].AddressLine
                            };
                            _db.Workplace.Add(wrk);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                if (data.DocAward.Count > 0)
                {
                    List<DocAward> gtawart = new List<DocAward>();
                    gtawart = _db.DocAward.Where(x => x.UserId == data.UserId).ToList();
                    for (int n = 0; n < data.DocAward.Count(); n++)
                    {
                        var wp = gtawart.Skip(n).FirstOrDefault();
                        if (wp != null)
                        {
                            wp.AwardTitle = data.DocAward[n].AwardTitle;
                            wp.Year = data.DocAward[n].Year;
                            wp.Description = data.DocAward[n].Description;
                            wp.UserId = data.UserId;
                        }
                        else
                        {
                            DocAward dcawd = new DocAward()
                            {
                                AwardTitle = data.DocAward[n].AwardTitle,
                                Year = data.DocAward[n].Year,
                                Description = data.DocAward[n].Description,
                                UserId = data.UserId
                            };
                            _db.DocAward.Add(dcawd);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                if (data.DocSpeciality.Count > 0)
                {
                    int DocId = _db.Doctors.Where(a => a.UserId == data.UserId).FirstOrDefault().Id;
                    List<Doctorspecilization> Sc = new List<Doctorspecilization>();
                    Sc = _db.Doctorspecilization.Where(x => x.DocId == DocId).ToList();
                    for (int k = 0; k < data.DocSpeciality.Count(); k++)
                    {
                        var ds = Sc.Skip(k).FirstOrDefault();
                        if (ds != null)
                        {
                            ds.Name = data.DocSpeciality[k].Name;
                            ds.DocId = DocId;
                        }
                        else
                        {
                            Doctorspecilization docsp = new Doctorspecilization()
                            {
                                Name = data.DocSpeciality[k].Name,
                                DocId = DocId
                            };
                            _db.Doctorspecilization.Add(docsp);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                if (data.DocClinic.Count > 0)
                {
                    List<DoctorClinicMap> dcm = new List<DoctorClinicMap>();
                    dcm = _db.DoctorClinicMap.Where(x => x.UserId == data.UserId).ToList();
                    for (int k = 0; k < data.DocClinic.Count(); k++)
                    {
                        var ds = dcm.Skip(k).FirstOrDefault();
                        if (ds != null)
                        {
                            ds.ClinicName = data.DocClinic[k].ClinicName;
                            ds.ClinicAddress = data.DocClinic[k].ClinicAddress;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            DoctorClinicMap docclimp = new DoctorClinicMap()
                            {
                                ClinicName = data.DocClinic[k].ClinicName,
                                ClinicAddress = data.DocClinic[k].ClinicAddress,
                                UserId = data.UserId
                            };
                            _db.DoctorClinicMap.Add(docclimp);
                            int j = await _db.SaveChangesAsync();
                            ClinicId = _db.DoctorClinicMap.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                            if (j > 0)
                            {
                                for (int l = 0; l < data.DocClinic[k].Image.Count(); l++)
                                {
                                    Documents documnt = new Documents
                                    {
                                        Url = data.DocClinic[k].Image[l].Path,
                                        CreatedAt = DateTime.UtcNow,
                                        Status = (int)UserStatus.Active,
                                        Type = (int)UserType.Doctor,
                                        Title = "Clinic Document"
                                    };
                                    _db.Documents.Add(documnt);
                                    await _db.SaveChangesAsync();
                                    int DocumentId = _db.Documents.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                    ClinicDocMap mp = new ClinicDocMap()
                                    {
                                        DocId = DocumentId,
                                        ClinicId = ClinicId,
                                        UserId = data.UserId
                                    };
                                    _db.ClinicDocMap.Add(mp);
                                    await _db.SaveChangesAsync();
                                }
                            }
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
        public ActionResult GetDoctorBookAppointment(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            List<BookPharmaApp> list = new List<BookPharmaApp>();
            list = (from appbk in _db.Appoinments
                    where appbk.DoctorConsultationId == DocId && appbk.IsActive != 0
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
                        AppointmentId = appbk.Appoinmentcode,
                        AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.UserRole),
                        AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(appbk.Id, appbk.UserRole, appbk.Appoinmentcode),
                        AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.UserRole),
                        Role = (int)appbk.MyRole,
                        MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                        CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate)
                    }).ToList();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments
                                where appbk.DoctorId == DocId && appbk.IsActive != 0
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    // RepName = appbk.PharmaRep.Name,
                                    DoctorName = appbk.Doctor.SecondName + " " + appbk.Doctor.FirstName,
                                    DoctorId = appbk.Doctor.UserId,
                                    DocImage = appbk.Doctor.PhotoUrl,
                                    AppointmentId = appbk.Appoinmentcode,
                                    Appointmenttype = "Pharma Representative",
                                    Status = (int)appbk.IsActive,
                                    // PharmaRepImage = appbk.PharmaRep.ProfileUrl,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(appbk.Id, appbk.MyRole, appbk.Appoinmentcode),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.MyRole,
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate)
                                }).ToList();
                if (list.Count > 0)
                {
                    data.Appdata.AddRange(list);
                    data.Appdata = data.Appdata.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
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
        public ActionResult GetDoctorBookAppointmentCalender(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();

            List<BookPharmaApp> list = new List<BookPharmaApp>();
            List<BookPharmaApp> ownconsultation = new List<BookPharmaApp>();
            list = (from appbk in _db.Appoinments
                    where appbk.DoctorConsultationId == DocId && appbk.IsActive != 0 && appbk.StartDate.Month >= DateTime.UtcNow.Month
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
                        AppointmentId = appbk.Appoinmentcode,
                        AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.UserRole),
                        AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.UserRole),
                        AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.UserRole),
                        Role = (int)appbk.MyRole,
                        MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                        CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate),
                        Status = (int)appbk.IsActive,
                        ConsultationType = 0,
                        Colorcode = appbk.ColorCode,
                    }).ToList();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments
                                where appbk.DoctorId == DocId && appbk.IsActive != 0 && appbk.StartDate.Month >= DateTime.UtcNow.Month
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    // RepName = appbk.PharmaRep.Name,
                                    DoctorName = appbk.Doctor.SecondName + " " + appbk.Doctor.FirstName,
                                    DoctorId = appbk.Doctor.UserId,
                                    AppointmentId = appbk.Appoinmentcode,
                                    // PharmaRepId = (int)appbk.PharmaRepId,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.MyRole,
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate),
                                    Status = (int)appbk.IsActive,
                                    Colorcode = appbk.ColorCode,
                                    ConsultationType = 0
                                }).ToList();
                ownconsultation = (from appbk in _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id)
                                   where appbk.DocId == DocId && appbk.ToTime.Month >= DateTime.UtcNow.Month && appbk.SlotType == "other" && appbk.Status == 0
                                   select new BookPharmaApp
                                   {
                                       Id = appbk.Id,
                                       StartDate = appbk.ToTime,
                                       EndDate = appbk.FromTime,
                                       AppointmentUserId = appbk.DocId,
                                       AppointmentUserName = appbk.Type,
                                       Duration = (int)appbk.Duration,
                                       AppointmentTitle = appbk.Subject,
                                       AppoinmentId = appbk.Id.ToString(),
                                       ConsultationType = 1,
                                       Colorcode = "#008000",
                                       //Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                   }).ToList();
                if (list.Count > 0)
                {
                    data.Appdata.AddRange(list);
                    data.Appdata = data.Appdata.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                }
                if (data.Appdata.Count > 0)
                {

                    data.Appdata = data.Appdata.OrderBy(a => a.StartDate).GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                    if (ownconsultation.Count > 0)
                    {
                        data.Appdata.AddRange(ownconsultation);
                    }
                    // data.Appdata.ForEach(x => x.Colorcode = getRandColor());
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
        public async Task<IActionResult> AddDoctorAppoinmentAvailibilty([FromBody] Bookfreeavailbility data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                DoctorAvailibiltyforAppointment Obj = new DoctorAvailibiltyforAppointment()
                {
                    ToTime = data.ToTime,
                    FromTime = data.FromTime,
                    DocId = data.DocId
                };
                _db.DoctorAvailibiltyforAppointment.Add(Obj);
                int i = await _db.SaveChangesAsync();
                if (i > 0)
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    resp.msg = "Successfully Added";
                    return Ok(resp);
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Ok;
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
        public ActionResult GetDoctorFreeTimeSlot(int DocId, int usertype)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            freeslot data = new freeslot();
            List<DoctorAvailibiltyforAppointment> list = new List<DoctorAvailibiltyforAppointment>();
            try
            {
                if (usertype == 2)
                {
                    list = _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id).Where(appbk => appbk.DocId == DocId && appbk.Status == 0 && appbk.FromTime > DateTime.UtcNow.AddMinutes(135) && appbk.SlotType == "win" && appbk.Docavailable == true).ToList();
                }
                else if (usertype == 4)
                {
                    list = _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id).Where(appbk => appbk.DocId == DocId && appbk.Status == 0 && appbk.FromTime > DateTime.UtcNow.AddMinutes(135) && appbk.SlotType == "win" && appbk.Patientavailable == true).ToList();
                }
                else if (usertype == 5)
                {
                    list = _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id).Where(appbk => appbk.DocId == DocId && appbk.Status == 0 && appbk.FromTime > DateTime.UtcNow.AddMinutes(135) && appbk.SlotType == "win" && appbk.Pharmaavailable == true).ToList();
                }
                else if (usertype == 6)
                {
                    list = _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id).Where(appbk => appbk.DocId == DocId && appbk.Status == 0 && appbk.FromTime > DateTime.UtcNow.AddMinutes(135) && appbk.SlotType == "win" && appbk.Nurseavailable == true).ToList();
                }
                if (list.Count > 0)
                {
                    data.getList = (from appbk in list
                                    select new Bookfreeavailbility
                                    {
                                        Id = appbk.Id,
                                        ToTime = appbk.ToTime,
                                        FromTime = appbk.FromTime,
                                        DocId = appbk.DocId,
                                        Name = _db.Doctors.Where(a => a.Id == appbk.DocId).FirstOrDefault().FirstName,
                                        Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                    }).ToList();
                    if (data.getList.Count != 0)
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
                data.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostDoctorFreeSlot(Bookfreeavailbility data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                TimeSpan span = data.FromTime.TimeOfDay - data.ToTime.TimeOfDay;
                int Duration = Math.Abs((int)span.TotalMinutes);
                var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                if (Duration >= 20)
                {
                    DateTime satrt = data.ToTime.AddDays(-1);
                    DateTime end = data.FromTime;
                    DateTime enddate = DateTime.UtcNow;
                    //TimeSpan getendtime = end.TimeOfDay;
                    //enddate = data.ToTime.AddDays(-1) + getendtime;
                    int day = (end - satrt).Days;
                    enddate = data.FromTime.AddDays(-day);
                    for (int j = 1; j <= day; j++)
                    {
                        var Newst = satrt.AddDays(j);
                        var newed = enddate.AddDays(j);
                        var GetDoctorSlot = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == data.DocId && a.Status == 0).ToList();
                        if (GetDoctorSlot.Count > 0)
                        {
                            var check = GetDoctorSlot.Where(a => a.DocId == data.DocId && a.ToTime <= Newst && a.FromTime >= newed || a.FromTime >= Newst && a.ToTime <= newed).FirstOrDefault();
                            // var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.ToTime.Date >= Newst.Date && a.ToTime.TimeOfDay >= Newst.TimeOfDay && a.FromTime.Date <= newed.Date && a.FromTime.TimeOfDay <= newed.TimeOfDay && a.DocId == data.DocId).FirstOrDefault();
                            // var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.FromTime >= Newst && a.ToTime <= newed && a.DocId == data.DocId).FirstOrDefault();
                            if (check != null)
                            {
                                Resp.msg = AppResource.Slot_added;
                                Resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(Resp);
                            }
                            else
                            {
                                DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                                {
                                    ToTime = satrt.AddDays(j),
                                    FromTime = enddate.AddDays(j),
                                    SlotType = data.SlotType,
                                    DocId = data.DocId,
                                    Status = 0,
                                    Docavailable = data.Docavailable,
                                    Patientavailable = data.Patientavailable,
                                    Nurseavailable = data.Nurseavailable,
                                    Pharmaavailable = data.Pharmaavailable,
                                    DoctorType = Doctor.DocCategory
                                };
                                _db.DoctorAvailibiltyforAppointment.Add(obj);
                            }
                        }
                        else
                        {
                            DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                            {
                                ToTime = satrt.AddDays(j),
                                FromTime = enddate.AddDays(j),
                                SlotType = data.SlotType,
                                DocId = data.DocId,
                                Status = 0,
                                Docavailable = data.Docavailable,
                                Patientavailable = data.Patientavailable,
                                Nurseavailable = data.Nurseavailable,
                                Pharmaavailable = data.Pharmaavailable,
                                DoctorType = Doctor.DocCategory
                            };
                            _db.DoctorAvailibiltyforAppointment.Add(obj);
                        }
                    }
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        Resp.msg = AppResource.user_Added;
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.msg = AppResource.appointment_failed;
                        Resp.code = (int)ApiCustomResponseCode.BadRequest;
                        return Ok(Resp);
                    }
                }
                else
                {
                    Resp.msg = AppResource.wrong_slot;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public ActionResult GetDoctorDashboard(int UserId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            List<BookPharmaApp> TodayCnsList = new List<BookPharmaApp>();
            List<BookPharmaApp> weekCnsList = new List<BookPharmaApp>();
            var Doc = _db.Doctors.Where(a => a.UserId == UserId).FirstOrDefault();
            TodayCnsList = (from ta in _db.Appoinments.OrderByDescending(a => a.StartDate)
                            where ta.StartDate.Date == DateTime.UtcNow.Date && ta.DoctorConsultationId == Doc.Id && ta.IsActive != 0
                            select new BookPharmaApp
                            {
                                Id = ta.Id,
                                StartDate = ta.StartDate,
                                EndDate = ta.EndDate,
                                BookingDate = ta.BookingDate,
                                AppointmentTitle = ta.AppointmentTitle,
                                AppointmentId = ta.Appoinmentcode,
                                Duration = (int)ta.Duration,
                                DoctorId = Doc.UserId,
                                DoctorName = Doc.SecondName + " " + Doc.FirstName,
                                DocImage = Doc.PhotoUrl,
                                AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(ta.Id, ta.UserRole),
                                AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta.Id, ta.UserRole, ta.Appoinmentcode),
                                AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(ta.Id, ta.UserRole),
                                Role = (int)ta.MyRole,
                                Status = (int)ta.IsActive,
                                MyRole = (int)ta.MyRole,
                                UserRole = (int)ta.UserRole,
                                CancelStatus = AppDbCommonLogic.GetCancelStatus(ta.Id, ta.EndDate),
                                //  UploadStatus = AppDbCommonLogic.getuploaddocumentstatus(ta.EndDate),
                                Expert = (from sp in _db.Doctorspecilization
                                          where sp.DocId == ta.DoctorId
                                          select new Specializion
                                          {
                                              Specialization = sp.Name
                                          }).ToList(),
                            }).ToList();
            weekCnsList = (from ta2 in _db.Appoinments.OrderByDescending(a => a.StartDate)
                           where ta2.EndDate >= DateTime.UtcNow && ta2.DoctorConsultationId == Doc.Id && ta2.IsActive != 0
                           select new BookPharmaApp
                           {
                               Id = ta2.Id,
                               StartDate = ta2.StartDate,
                               EndDate = ta2.EndDate,
                               BookingDate = ta2.BookingDate,
                               AppointmentTitle = ta2.AppointmentTitle,
                               AppointmentId = ta2.Appoinmentcode,
                               Duration = (int)ta2.Duration,
                               DoctorId = Doc.UserId,
                               DoctorName = Doc.SecondName + " " + Doc.FirstName,
                               DocImage = Doc.PhotoUrl,
                               AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(ta2.Id, ta2.UserRole),
                               AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta2.Id, ta2.UserRole, ta2.Appoinmentcode),
                               AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(ta2.Id, ta2.UserRole),
                               Role = (int)ta2.MyRole,
                               Status = (int)ta2.IsActive,
                               MyRole = (int)ta2.MyRole,
                               UserRole = (int)ta2.UserRole,
                               CancelStatus = AppDbCommonLogic.GetCancelStatus(ta2.Id, ta2.EndDate),
                               //    UploadStatus = AppDbCommonLogic.getuploaddocumentstatus(ta2.EndDate),
                               Expert = (from sp in _db.Doctorspecilization
                                         where sp.DocId == ta2.DoctorId
                                         select new Specializion
                                         {
                                             Specialization = sp.Name
                                         }).ToList()
                           }).ToList();
            GetAdminDash data = new GetAdminDash();
            try
            {
                data.getdata = (from dsh in _db.Doctors
                                where dsh.UserId == UserId
                                select new DocDashboard
                                {
                                    Name = dsh.SecondName + " " + dsh.FirstName,
                                    TodayPatient = _db.Appoinments.Where(a => a.DoctorId == dsh.Id && a.IsActive != 0 && a.StartDate.Date == DateTime.UtcNow.Date && a.PatientId != null && a.PatientId != 0).Count(),
                                    TotalPatient = _db.Appoinments.Where(a => a.DoctorId == dsh.Id && a.IsActive != 0 && a.PatientId != null && a.PatientId != 0).GroupBy(x => x.PatientId).Select(x => x.First()).Count(),
                                    Appointments = _db.Appoinments.Where(a => a.DoctorId == dsh.Id && a.IsActive != 0).Count(),
                                    TotalPharma = _db.Appoinments.Where(a => a.DoctorId == dsh.Id && a.IsActive != 0 && a.PharmaRepId != null && a.PharmaRepId != 0).GroupBy(x => x.PharmaRepId).Select(x => x.First()).Count(),
                                    TodayAppList = (from ta in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                    where ta.StartDate.Date == DateTime.UtcNow.Date && ta.DoctorId == dsh.Id && ta.IsActive != 0 /*|| (ta.StartDate.Date == DateTime.Now.Date && ta.DoctorConsultationId == dsh.Id && ta.IsActive != 0)*/
                                                    select new BookPharmaApp
                                                    {
                                                        Id = ta.Id,
                                                        StartDate = ta.StartDate,
                                                        EndDate = ta.EndDate,
                                                        BookingDate = ta.BookingDate,
                                                        AppointmentTitle = ta.AppointmentTitle,
                                                        AppointmentId = ta.Appoinmentcode,
                                                        Duration = (int)ta.Duration,
                                                        DoctorId = dsh.UserId,
                                                        DoctorName = dsh.SecondName + " " + dsh.FirstName,
                                                        DocImage = dsh.PhotoUrl,
                                                        AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(ta.Id, ta.MyRole),
                                                        AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta.Id, ta.MyRole, ta.Appoinmentcode),
                                                        AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(ta.Id, ta.MyRole),
                                                        Role = (int)ta.MyRole,
                                                        Appointmenttype = "Pharma Representative",
                                                        Status = (int)ta.IsActive,
                                                        MyRole = (int)ta.MyRole,
                                                        UserRole = (int)ta.UserRole,
                                                        CancelStatus = AppDbCommonLogic.GetCancelStatus(ta.Id, ta.EndDate),
                                                        Expert = (from sp in _db.Doctorspecilization
                                                                  where sp.DocId == ta.DoctorId
                                                                  select new Specializion
                                                                  {
                                                                      Specialization = sp.Name
                                                                  }).ToList(),
                                                    }).ToList(),
                                    WeekAppList = (from ta2 in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                                   where ta2.EndDate >= DateTime.UtcNow && ta2.DoctorId == dsh.Id && ta2.IsActive != 0/*||(ta2.StartDate.Date >= DateTime.UtcNow.Date && ta2.StartDate <= DateTime.UtcNow.Date.AddDays(7) && ta2.DoctorConsultationId == dsh.Id && ta2.IsActive != 0)*/
                                                   select new BookPharmaApp
                                                   {
                                                       Id = ta2.Id,
                                                       StartDate = ta2.StartDate,
                                                       EndDate = ta2.EndDate,
                                                       BookingDate = ta2.BookingDate,
                                                       AppointmentTitle = ta2.AppointmentTitle,
                                                       AppointmentId = ta2.Appoinmentcode,
                                                       Duration = (int)ta2.Duration,
                                                       DoctorId = dsh.UserId,
                                                       DoctorName = dsh.SecondName + " " + dsh.FirstName,
                                                       DocImage = dsh.PhotoUrl,
                                                       AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(ta2.Id, ta2.MyRole),
                                                       AppointmentUserName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta2.Id, ta2.MyRole, ta2.Appoinmentcode),
                                                       AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(ta2.Id, ta2.MyRole),
                                                       Role = (int)ta2.MyRole,
                                                       Appointmenttype = "Pharma Representative",
                                                       Status = (int)ta2.IsActive,
                                                       MyRole = (int)ta2.MyRole,
                                                       UserRole = (int)ta2.UserRole,
                                                       CancelStatus = AppDbCommonLogic.GetCancelStatus(ta2.Id, ta2.EndDate),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == ta2.DoctorId
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = sp.Name
                                                                 }).ToList()
                                                   }).ToList(),
                                }).FirstOrDefault();
                if (TodayCnsList.Count > 0)
                {
                    data.getdata.TodayAppList.AddRange(TodayCnsList);
                    data.getdata.TodayAppList = data.getdata.TodayAppList.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                }
                else if (data.getdata.TodayAppList.Count > 0)
                {
                    data.getdata.TodayAppList = data.getdata.TodayAppList.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                }
                if (weekCnsList.Count > 0)
                {
                    data.getdata.WeekAppList.AddRange(weekCnsList);
                    data.getdata.WeekAppList = data.getdata.WeekAppList.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                }
                else if (data.getdata.WeekAppList.Count > 0)
                {
                    data.getdata.WeekAppList = data.getdata.WeekAppList.GroupBy(x => x.AppointmentId).Select(x => x.First()).ToList();
                }
                if (data.getdata != null)
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
        public ActionResult GetDoctorActivityList(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            List<BookPharmaApp> list = new List<BookPharmaApp>();
            //list = (from appbk in _db.ActivityList.OrderByDescending(x => x.Id)
            //        where appbk.DocToDocId == DocId
            //        select new BookPharmaApp
            //        {
            //            Id = appbk.Appointment.Id,
            //            StartDate = appbk.Appointment.StartDate,
            //            EndDate = appbk.Appointment.EndDate,
            //            BookingDate = appbk.Appointment.BookingDate,
            //            AppointmentTitle = appbk.Appointment.AppointmentTitle,
            //            Duration = (int)appbk.Appointment.Duration,
            //            DoctorName = appbk.Appointment.Doctor.FirstName + " " + appbk.Appointment.Doctor.SecondName,
            //            DoctorId = appbk.Appointment.Doctor.UserId,
            //            DocImage = appbk.Appointment.Doctor.PhotoUrl,
            //            AppointmentId = appbk.Appointment.Appoinmentcode,
            //            Appointmenttype = "Pharma Representative",
            //            Status = (int)appbk.Status,
            //            AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Appointment.Id, appbk.Appointment.MyRole),
            //            AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Appointment.Id, appbk.Appointment.MyRole),
            //            AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Appointment.Id, appbk.Appointment.MyRole),
            //            Role = (int)appbk.Appointment.MyRole,
            //           // CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Appointment.Id, appbk.Appointment.EndDate)
            //        }).ToList();
            try
            {
                data.Appdata = (from appbk in _db.ActivityList.OrderByDescending(x => x.Id)
                                where appbk.DocId == DocId || appbk.DocToDocId == DocId
                                select new BookPharmaApp
                                {
                                    Id = appbk.Appointment.Id,
                                    StartDate = appbk.Appointment.StartDate,
                                    EndDate = appbk.Appointment.EndDate,
                                    BookingDate = appbk.Appointment.BookingDate,
                                    AppointmentTitle = appbk.Appointment.AppointmentTitle,
                                    Duration = (int)appbk.Appointment.Duration,
                                    //  RepName = appbk.PharmaRep.Name,
                                    //DoctorName = appbk.Appointment.Doctor.FirstName + " " + appbk.Appointment.Doctor.SecondName,
                                    //DoctorId = appbk.Appointment.Doctor.UserId,
                                    //DocImage = appbk.Appointment.Doctor.PhotoUrl,
                                    AppointmentId = appbk.Appointment.Appoinmentcode,
                                    Appointmenttype = "Pharma Representative",
                                    Status = appbk.Status,
                                    //  PharmaRepImage = appbk.PharmaRep.ProfileUrl,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Appointment.Id, appbk.Appointment.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Appointment.Id, appbk.Appointment.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Appointment.Id, appbk.Appointment.MyRole),
                                    Role = (int)appbk.Appointment.MyRole,
                                    //  CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Appointment.Id, appbk.Appointment.EndDate)
                                }).ToList();
                if (data.Appdata.Count != 0)
                {
                    data.Appdata = data.Appdata.GroupBy(x => new { x.Id, x.Status }).Select(x => x.First()).ToList();
                    data.msg = AppResource.user_listS;
                    data.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(data);
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
                resp.code = (int)ApiCustomResponseCode.Error;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetMedicalConsultationList(int DocId)
        {

            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            // List<BookPharmaApp> list = new List<BookPharmaApp>();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                where appbk.DoctorId == DocId && appbk.PharmaRepId != null && appbk.PharmaRepId != 0
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    //  RepName = appbk.PharmaRep.Name,
                                    AppointmentId = appbk.Appoinmentcode,
                                    Appointmenttype = "Pharma Representative",
                                    Status = (int)appbk.IsActive,
                                    DocImage = appbk.Doctor.PhotoUrl,
                                    DoctorName = appbk.Doctor.SecondName + " " + appbk.Doctor.FirstName,
                                    DoctorId = appbk.Doctor.UserId,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.MyRole,
                                    // PharmaRepImage = appbk.PharmaRep.ProfileUrl,
                                    //    Attachment = _db.Documents.Where(a => a.Id == (int)_db.AppointmentDocuments.Where(b => b.AppointmetId == appbk.Id).FirstOrDefault().DocumentId).FirstOrDefault().Url
                                }).ToList();

                //data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                //                where appbk.DoctorId == DocId && appbk.StartDate.Date <= DateTime.UtcNow.Date && appbk.IsActive == 2 && appbk.PharmaRepId != null && appbk.PharmaRepId != 0
                //                select new BookPharmaApp
                //                {
                //                    Id = appbk.Id,
                //                    StartDate = appbk.StartDate,
                //                    EndDate = appbk.EndDate,
                //                    BookingDate = appbk.BookingDate,
                //                    AppointmentTitle = appbk.AppointmentTitle,
                //                    Duration = (int)appbk.Duration,
                //                    //  RepName = appbk.PharmaRep.Name,
                //                    AppointmentId = appbk.Appoinmentcode,
                //                    Appointmenttype = "Pharma Representative",
                //                    Status = (int)appbk.IsActive,
                //                    DocImage = appbk.Doctor.PhotoUrl,
                //                    DoctorName = appbk.Doctor.FirstName + " " + appbk.Doctor.SecondName,
                //                    DoctorId = appbk.Doctor.UserId,
                //                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                //                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                //                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                //                    Role = (int)appbk.MyRole,
                //                    // PharmaRepImage = appbk.PharmaRep.ProfileUrl,
                //                    Attachment = _db.Documents.Where(a => a.Id == (int)_db.AppointmentDocuments.Where(b => b.AppointmetId == appbk.Id).FirstOrDefault().DocumentId).FirstOrDefault().Url
                //                }).ToList();
                //if (list.Count > 0)
                //{
                //    data.Appdata.AddRange(list);
                //}
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
        [HttpDelete]
        public async Task<IActionResult> DeleteDoctorSlot(int Id)
        {
            ApiResponse resp = new ApiResponse();
            var dc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == Id).FirstOrDefault();
            try
            {
                if (dc != null)
                {
                    //_db.DoctorAvailibiltyforAppointment.Remove(dc);
                    var chckapp = _db.Appoinments.Where(a => a.DocSlotId == Id && a.IsActive != 0).FirstOrDefault();
                    if (chckapp != null)
                    {
                        resp.code = (int)ApiCustomResponseCode.Error;
                        resp.msg = AppResource.user_Failed;
                        return Ok(resp);
                    }
                    else
                    {
                        dc.Status = 1;
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
                }
                else
                {
                    resp.code = (int)ApiCustomResponseCode.Error;
                    resp.msg = AppResource.invalid_model;
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = AppResource.invalid_model;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetDoctorTimeMangementList(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            freeslot data = new freeslot();
            try
            {
                data.getList = (from appbk in _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id)
                                where appbk.DocId == DocId && appbk.FromTime.AddMinutes(-20) > DateTime.UtcNow && appbk.SlotType == "win" && appbk.Status == 0
                                select new Bookfreeavailbility
                                {
                                    Id = appbk.Id,
                                    ToTime = appbk.ToTime,
                                    FromTime = appbk.FromTime,
                                    DocId = appbk.DocId,
                                    Name = _db.Doctors.Where(a => a.Id == appbk.DocId).FirstOrDefault().FirstName,
                                    //Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                }).ToList();
                if (data.getList.Count != 0)
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
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult GetDoctorReferralTimeMangementList(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            freeslot data = new freeslot();
            try
            {
                data.getList = (from appbk in _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id)
                                where appbk.DocId == DocId && appbk.FromTime.AddMinutes(-20) > DateTime.UtcNow && appbk.SlotType == "ref" && appbk.Status == 0
                                select new Bookfreeavailbility
                                {
                                    Id = appbk.Id,
                                    ToTime = appbk.ToTime,
                                    FromTime = appbk.FromTime,
                                    DocId = appbk.DocId,
                                    Name = _db.Doctors.Where(a => a.Id == appbk.DocId).FirstOrDefault().FirstName,
                                    //Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                }).ToList();
                if (data.getList.Count != 0)
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
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDoctorConsultation(UpdateDoctorCounsultation data)
        {
            ApiResponse Resp = new ApiResponse();
            int row = 0;
            var doctor = _db.Doctors.Where(a => a.UserId == data.UserId).FirstOrDefault();
            try
            {
                if (data.Count != 0)
                {
                    doctor.DoctorConslutationCount = data.Count;
                }
                doctor.DoctorConsultation = data.Status;
                row = await _db.SaveChangesAsync();
                if (row > 0)
                {
                    Resp.msg = AppResource.user_update;
                    Resp.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(Resp);
                }
                else
                {
                    Resp.msg = AppResource.user_Failed;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public ActionResult BookDoctorAppointment(BookPharmaApp data)
        {
            ApiResponse resp = new ApiResponse();
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
            AppCode = "DR" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow;
            int row = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    //   int DoctorCount = 0;
                    // int GetDoctorWeekCount = (int)_db.Doctors.Where(a => a.Id == data.DoctorConsultationId).FirstOrDefault().DoctorConslutationCount;
                    //DateTime Monday = DateTime.Now;
                    //DateTime Saturday = DateTime.Now;
                    //DayOfWeek day = DateTime.Now.DayOfWeek;
                    //Monday = DateTime.Today.AddDays(-(int)(data.StartDate.DayOfWeek - DayOfWeek.Monday));
                    //Saturday = DateTime.Today.AddDays(-(int)(data.StartDate.DayOfWeek - DayOfWeek.Saturday));
                    string path = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                    string Completefile = path + "/" + "My" + DateTime.UtcNow.Ticks + "" + "." + "json";
                    System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(data.DoctorsId.Count()));
                    DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
                    if (GetUserTime > data.StartDate)
                    {
                        resp.msg = AppResource.appointment_booked;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
                    }
                    else
                    {
                        var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == data.DoctorId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                        if (Slotbook == null)
                        {
                            foreach (var doctcorIdObj in data.DoctorsId)
                            {
                                string path1 = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                                string Completefile1 = path1 + "/" + "My1" + DateTime.UtcNow.Ticks + "" + "." + "json";
                                System.IO.File.WriteAllText(Completefile1, JsonConvert.SerializeObject(doctcorIdObj.DoctorId));
                                Appoinments obj = new Appoinments()
                                {
                                    StartDate = data.StartDate,
                                    EndDate = data.StartDate.AddMinutes(data.Duration),
                                    BookingDate = DateTime.UtcNow,
                                    AppointmentTitle = data.AppointmentTitle,
                                    Duration = data.Duration,
                                    DoctorId = data.DoctorId,
                                    DoctorConsultationId = doctcorIdObj.DoctorId,
                                    Appoinmentcode = AppCode,
                                    IsActive = 1,
                                    DocSlotId = data.SlotId,
                                    UserRole = 2,
                                    MyRole = 8
                                };
                                _db.Appoinments.Add(obj);
                                row = _db.SaveChanges();
                                if (row > 0)
                                {
                                    int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                    AppDbCommonLogic.InsertDoctorDoctorActivity(AppId, doctcorIdObj.DoctorId, data.DoctorId);
                                    AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                    resp.msg = AppResource.appointment_success;
                                    resp.code = (int)ApiCustomResponseCode.Ok;
                                }
                            }
                        }
                        else
                        {
                            resp.msg = AppResource.pharma_slot;
                            resp.code = (int)ApiCustomResponseCode.Error;
                        }
                    }
                }
                else
                {
                    resp.msg = AppResource.invalid_model;
                    resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(resp);
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CancelDoctorMeeting(meeting data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                var meetdata = _db.Appoinments.Where(a => a.Id == data.Id).FirstOrDefault();
                if (meetdata != null)
                {
                    meetdata.IsActive = 0;
                    await _db.SaveChangesAsync();
                    AppDbCommonLogic.CancelMeetingActivity(data.Id);
                    resp.msg = AppResource.user_cancel;
                    resp.code = (int)ApiCustomResponseCode.Ok;
                    if (meetdata.MyRole == 4 && meetdata.UserRole == 2)
                    {
                        AppDbCommonLogic.CancelPaymenttStatus(data.Id);
                        AppDbCommonLogic.SendCancelDoctorMeetingEmailToUser(data.Id, 2);
                    }
                    else if (meetdata.MyRole == 8 && meetdata.UserRole == 2)
                    {
                        AppDbCommonLogic.SendCancelDoctorMeetingEmailToUser(data.Id, 2);
                    }
                    else
                    {
                        AppDbCommonLogic.SendCancelDoctorMeetingEmailToUser(data.Id, 2);
                    }
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
                var doctor = _db.Doctors.Where(a => a.Id == data.Id).FirstOrDefault();
                AppDbCommonLogic.InsertUserLog(doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctors(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.Doctors.Where(a => a.Status == 1 && a.Id != DocId && a.DocCategory == "KeyOpinionLeader").OrderByDescending(a => a.Id).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = r.UserId,
                                                       FirstName = r.SecondName + " " + r.FirstName,
                                                       YearsOfExperiecne = r.YearsOfExperiecne,
                                                       MedicalRegistrationNo = r.MedicalRegistrationNo,
                                                       City = r.City,
                                                       StreetNumber = r.StreetNumber,
                                                       Zipcode = r.Zipcode,
                                                       ContractualDoctorsCompany = r.ContractualDoctorsCompany,
                                                       ContractStartDate = r.ContractStartDate,
                                                       ContarctEndDate = r.ContarctEndDate,
                                                       ShortIntroduction = r.ShortIntroduction,
                                                       PhotoUrl = r.PhotoUrl,
                                                       Status = r.Status,
                                                       DoctorConslutationCount = r.DoctorConslutationCount,
                                                       DoctorConsultation = r.DoctorConsultation,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(r.UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.Id && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = sp.Name
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.Id)
                                                   }).ToList();
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
            return Ok(list);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetFilterDoctors(List<int> specializationArray)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            List<doctor> getlist = new List<doctor>();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                if (specializationArray.Count > 0)
                {
                    foreach (var item in specializationArray)
                    {
                        var getDoc = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.CatId == item && a.Doc.Status == 1 && a.Doc.DocCategory == "KeyOpinionLeader").ToList();
                        if (getDoc.Count != 0)
                        {
                            data.SpecializationCategory = (from r in getDoc
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
                                                               //Expert = (from sp in _db.Doctorspecilization
                                                               //          where sp.DocId == r.Doc.Id && sp.Status == 1
                                                               //          select new Specializion
                                                               //          {
                                                               //              Specialization = sp.Name
                                                               //          }).ToList(),
                                                               Expert = AppDbCommonLogic.GetDoctorSpeciality(r.Doc.Id, Lang),
                                                               DocExperiance = (from sp in _db.Workplace
                                                                                where sp.UserId == r.Doc.UserId
                                                                                select new experinace
                                                                                {
                                                                                    WorkplaceTitle = sp.WorkplaceTitle,
                                                                                    AddressLine = sp.AddressLine
                                                                                }).ToList(),
                                                               DoctorType = (int)r.Doc.DoctorType,
                                                               DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doc.DoctorType, Lang),
                                                               Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.Doc.Id)
                                                           }).ToList();
                        }
                        getlist.AddRange(data.SpecializationCategory);
                    }
                }
                DataResponse dataResponse = new DataResponse();
                dataResponse.getList = getlist.GroupBy(x => x.UserId).Select(x => x.First()).ToList();
                if (dataResponse.getList.Count() > 0)
                {
                    dataResponse.msg = AppResource.user_listS;
                    dataResponse.code = (int)ApiCustomResponseCode.Ok;
                    return Ok(dataResponse);
                }
                else
                {
                    dataResponse.msg = AppResource.user_listE;
                    dataResponse.code = (int)ApiCustomResponseCode.Error;
                    return Ok(dataResponse);
                }
            }
            catch (Exception ex)
            {
                //resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.Error;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetNurseDoctors(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.Doctors.Where(a => a.Status == 1 && a.Id != DocId).OrderByDescending(a => a.Id).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = r.UserId,
                                                       FirstName = r.SecondName + " " + r.FirstName,
                                                       YearsOfExperiecne = r.YearsOfExperiecne,
                                                       MedicalRegistrationNo = r.MedicalRegistrationNo,
                                                       City = r.City,
                                                       StreetNumber = r.StreetNumber,
                                                       Zipcode = r.Zipcode,
                                                       ContractualDoctorsCompany = r.ContractualDoctorsCompany,
                                                       ContractStartDate = r.ContractStartDate,
                                                       ContarctEndDate = r.ContarctEndDate,
                                                       ShortIntroduction = r.ShortIntroduction,
                                                       PhotoUrl = r.PhotoUrl,
                                                       Status = r.Status,
                                                       DoctorConslutationCount = r.DoctorConslutationCount,
                                                       DoctorConsultation = r.DoctorConsultation,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(r.UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.Id && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = sp.Name
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.Id)
                                                   }).ToList();
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
            return Ok(list);
        }
        [HttpGet]
        public ActionResult GetConsultationHistory(int DoctorId, int Month, int Year)
        {
            DocConsult data = new DocConsult();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                if (Month == 0 && Year == 0)
                {
                    data.ConsultList = (from r in _db.Appoinments
                                        where r.DoctorId == DoctorId && r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Patient.Id,
                                            UserId = r.Patient.UserId,
                                            FirstName = r.Patient.SecondName + " " + r.Patient.FirstName,
                                            PhotoUrl = r.Patient.ProfileUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = Convert.ToInt32(AppDbCommonLogic.Get80percentAmount(r.Id)),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang),
                                            ssnno = r.Patient.SocialSecurityNumber
                                        }).ToList();
                    data.Amount = (int)AppDbCommonLogic.TotalAmount(DoctorId);
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
                                        where r.DoctorId == DoctorId && r.MyRole == 4 && r.IsActive != 0 && r.BookingDate.Month == Month && r.BookingDate.Year == Year && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Patient.Id,
                                            UserId = r.Patient.UserId,
                                            FirstName = r.Patient.SecondName + " " + r.Patient.FirstName,
                                            PhotoUrl = r.Patient.ProfileUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = Convert.ToInt32(AppDbCommonLogic.Get80percentAmount(r.Id)),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang),
                                            ssnno = r.Patient.SocialSecurityNumber
                                        }).ToList();
                    data.Amount = (int)AppDbCommonLogic.DoctorMonthyaerTotalAmount(DoctorId, Month, Year);
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
                                        where r.DoctorId == DoctorId && r.MyRole == 4 && r.IsActive != 0 && r.BookingDate.Year == Year && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Patient.Id,
                                            UserId = r.Patient.UserId,
                                            FirstName = r.Patient.SecondName + " " + r.Patient.FirstName,
                                            PhotoUrl = r.Patient.ProfileUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = Convert.ToInt32(AppDbCommonLogic.Get80percentAmount(r.Id)),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang),
                                            ssnno = r.Patient.SocialSecurityNumber
                                        }).ToList();
                    data.Amount = (int)AppDbCommonLogic.DoctorYearTotalAmount(DoctorId, Year);
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
                                        where r.DoctorId == DoctorId && r.MyRole == 4 && r.IsActive != 0 && r.BookingDate.Month == Month && r.IsActive != 3
                                        select new doctor
                                        {
                                            Id = r.Patient.Id,
                                            UserId = r.Patient.UserId,
                                            FirstName = r.Patient.SecondName + " " + r.Patient.FirstName,
                                            PhotoUrl = r.Patient.ProfileUrl,
                                            Status = r.IsActive,
                                            Duration = (int)r.Duration,
                                            AppointmentTitle = r.AppointmentTitle,
                                            LastVisitDate = r.StartDate,
                                            Amount = Convert.ToInt32(AppDbCommonLogic.Get80percentAmount(r.Id)),
                                            PaymentDate = AppDbCommonLogic.GetPatientAmountDate(r.Id),
                                            Invoice = AppDbCommonLogic.GetInvoice(r.Id),
                                            InvoiceStatus = AppDbCommonLogic.GetInvoiceStatus(r.Id),
                                            DoctorTypeName = AppDbCommonLogic.GetDoctorTypeName(r.Doctor.DoctorType, Lang),
                                            ssnno = r.Patient.SocialSecurityNumber
                                        }).ToList();
                    data.Amount = (int)AppDbCommonLogic.MonthTotalAmountDoctor(DoctorId, Month);
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
        public ActionResult GetPatientConsultation(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                where appbk.PatientId != 0 && appbk.IsActive == 2 && appbk.PatientId != null && appbk.DoctorId == DocId
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.UserRole,
                                    AppointmentId = appbk.Appoinmentcode,
                                    Appointmenttype = "Patient Appointment",
                                    Status = (int)appbk.IsActive,
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate)
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
        [HttpGet]
        public ActionResult GetNurseConsultation(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                where appbk.NurseId != 0 && appbk.IsActive == 2 && appbk.NurseId != null && appbk.DoctorId == DocId
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.UserRole,
                                    AppointmentId = appbk.Appoinmentcode,
                                    Appointmenttype = "Nurse Appointment",
                                    Status = (int)appbk.IsActive,
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate)
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
        [HttpGet]
        public ActionResult GetDoctorToDoctorConsultation(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetApp data = new GetApp();
            try
            {
                data.Appdata = (from appbk in _db.Appoinments.OrderByDescending(x => x.Id)
                                where appbk.DoctorConsultationId != 0 && appbk.IsActive == 2 && appbk.DoctorConsultationId != null && appbk.DoctorId == DocId
                                select new BookPharmaApp
                                {
                                    Id = appbk.Id,
                                    StartDate = appbk.StartDate,
                                    EndDate = appbk.EndDate,
                                    BookingDate = appbk.BookingDate,
                                    AppointmentTitle = appbk.AppointmentTitle,
                                    Duration = (int)appbk.Duration,
                                    AppointmentUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    AppointmentUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    AppointmentUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserId = AppDbCommonLogic.GetAppintmentUserId(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserName = AppDbCommonLogic.GetAppintmentUserName(appbk.Id, appbk.MyRole),
                                    //AppointmentSUserImage = AppDbCommonLogic.GetAppintmentUserImage(appbk.Id, appbk.MyRole),
                                    Role = (int)appbk.UserRole,
                                    AppointmentId = appbk.Appoinmentcode,
                                    Appointmenttype = "Doctor Appointment",
                                    Status = (int)appbk.IsActive,
                                    MeetingJoinStatus = AppDbCommonLogic.GetMeetingStatus(appbk.StartDate, appbk.EndDate, appbk.Id),
                                    CancelStatus = AppDbCommonLogic.GetCancelStatus(appbk.Id, appbk.EndDate)
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
        [HttpGet]
        public ActionResult GetMoreDoctorList(DateTime StartDate, int Duration)
        {
            {
                ApiResponse resp = new ApiResponse();
                resp.msg = AppResource.invalid_model;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                GetSpeCat data = new GetSpeCat();
                string Lang = "";
                Lang = Request.Headers["Lang"];
                DateTime EndDate = StartDate.AddMinutes(Duration);
                try
                {
                    var getDoc = _db.Doctors.Where(a => a.Status == 1).ToList();
                    if (getDoc.Count != 0)
                    {
                        data.SpecializationCategory = (from r in getDoc
                                                       select new doctor
                                                       {
                                                           Id = r.Id,
                                                           UserId = r.UserId,
                                                           FirstName = r.SecondName + " " + r.FirstName,
                                                           YearsOfExperiecne = r.YearsOfExperiecne,
                                                           MedicalRegistrationNo = r.MedicalRegistrationNo,
                                                           City = r.City,
                                                           StreetNumber = r.StreetNumber,
                                                           Zipcode = r.Zipcode,
                                                           ContractualDoctorsCompany = r.ContractualDoctorsCompany,
                                                           ContractStartDate = r.ContractStartDate,
                                                           ContarctEndDate = r.ContarctEndDate,
                                                           ShortIntroduction = r.ShortIntroduction,
                                                           PhotoUrl = r.PhotoUrl,
                                                           PhoneNo = r.PhoneNo,
                                                           Status = r.Status,
                                                           DoctorConslutationCount = r.DoctorConslutationCount,
                                                           DoctorConsultation = r.DoctorConsultation,
                                                           BussinessStatus = AppDbCommonLogic.GetBussinessStatus(r.UserId),
                                                           Expert = AppDbCommonLogic.GetDoctorSpeciality(r.Id, Lang),
                                                           Awailability = AppDbCommonLogic.CheckDoctorAvalabilityForDoctorPharmaAppointment(r.Id, StartDate, EndDate),
                                                           CheckCount = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.Id)
                                                       }).ToList();
                    }
                    if (data.SpecializationCategory.Count() > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.CheckCount != 0).GroupBy(x => x.Id).Select(x => x.First()).ToList();
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
        }
        [HttpGet]
        public ActionResult GetDoctorReferralTimeSlot(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            freeslot data = new freeslot();
            try
            {
                data.getList = (from appbk in _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id)
                                where appbk.DocId == DocId && appbk.Status == 0 && appbk.FromTime > DateTime.UtcNow.AddMinutes(135) && appbk.SlotType == "ref" && appbk.Status == 0
                                select new Bookfreeavailbility
                                {
                                    Id = appbk.Id,
                                    ToTime = appbk.ToTime,
                                    FromTime = appbk.FromTime,
                                    DocId = appbk.DocId,
                                    Name = _db.Doctors.Where(a => a.Id == appbk.DocId).FirstOrDefault().FirstName,
                                    Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                }).ToList();
                if (data.getList.Count != 0)
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
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpPost]
        public ActionResult BookDoctorReferralAppointment()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            BookPharmaApp data = new BookPharmaApp();
            var files = HttpContext.Request.Form.Files;
            data.AppointmentTitle = HttpContext.Request.Form["AppointmentTitle"];
            data.DoctorConsultationId = Convert.ToInt32(HttpContext.Request.Form["DoctorConsultationId"]);
            data.DoctorId = Convert.ToInt32(HttpContext.Request.Form["DoctorId"]);
            data.Duration = Convert.ToInt32(HttpContext.Request.Form["Duration"]);
            data.SlotId = Convert.ToInt32(HttpContext.Request.Form["SlotId"]);
            data.StartDate = Convert.ToDateTime(HttpContext.Request.Form["StartDate"]);
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
            AppCode = "DR-R" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow;
            int row = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
                    if (GetUserTime > data.StartDate)
                    {
                        resp.msg = AppResource.appointment_booked;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
                    }
                    else
                    {
                        var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == data.DoctorId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                        if (Slotbook == null)
                        {
                            var checkReferalDoctor = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorConsultationId == data.DoctorConsultationId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                            if (checkReferalDoctor != null)
                            {
                                AppCode = checkReferalDoctor.Appoinmentcode;
                            }
                            Appoinments obj = new Appoinments()
                            {
                                StartDate = data.StartDate,
                                EndDate = data.StartDate.AddMinutes(data.Duration),
                                BookingDate = DateTime.UtcNow,
                                AppointmentTitle = data.AppointmentTitle,
                                Duration = data.Duration,
                                DoctorId = data.DoctorId,
                                DoctorConsultationId = data.DoctorConsultationId,
                                Appoinmentcode = AppCode,
                                IsActive = 1,
                                DocSlotId = data.SlotId,
                                UserRole = 2,
                                MyRole = 8
                            };
                            _db.Appoinments.Add(obj);
                            row = _db.SaveChanges();
                            if (row > 0)
                            {
                                int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                AppDbCommonLogic.InsertDoctorDoctorActivity(AppId, data.DoctorConsultationId, data.DoctorId);
                                AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                foreach (var Image in files)
                                {
                                    if (Image != null && Image.Length > 0)
                                    {
                                        var Extetion = Path.GetExtension(files[0].FileName);
                                        using (var ms = new MemoryStream())
                                        {
                                            Image.CopyTo(ms);
                                            var fileBytes = ms.ToArray();
                                            string convertbyte = Convert.ToBase64String(fileBytes);
                                            AppointmentDocuments doc = new AppointmentDocuments
                                            {
                                                AppointmetId = AppId,
                                                DocUrl = new AES_ALGORITHM().Encrypt(convertbyte),
                                                Isseen = 0,
                                                Filetype = Extetion,
                                                Usertype = 8,
                                                MeetingId = AppCode,
                                                DocId = data.DoctorId
                                            };
                                            _db.AppointmentDocuments.Add(doc);
                                            _db.SaveChanges();
                                        }
                                    }
                                }
                                resp.msg = AppResource.appointment_success;
                                resp.code = (int)ApiCustomResponseCode.Ok;
                            }
                        }
                        else
                        {
                            resp.msg = AppResource.pharma_slot;
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
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorsWithAppointmentDate(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.FromTime >= DateTime.UtcNow && a.DocId != DocId && a.DoctorType == "KeyOpinionLeader").OrderByDescending(a => a.FromTime).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                       AppointmentDate = r.ToTime,
                                                       AppointmentType = r.SlotType,
                                                       SlotId = r.Id,
                                                       DoctorId = r.DocId,
                                                       PhotoUrl = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().PhotoUrl,
                                                       Status = r.Status,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                                       ToTime = r.ToTime,
                                                       FromTime = r.FromTime,
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.OrderBy(m => m.AppointmentDate).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> PostDoctorNewFreeSlot(Bookfreeavailbility data)
        {
            ApiResponse Resp = new ApiResponse();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                var Newst = data.ToTime;
                var newed = data.ToTime.AddMinutes(data.Duration);
                var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == data.DocId && a.Status == 0 && (a.ToTime <= Newst && a.FromTime >= newed || a.FromTime >= Newst && a.ToTime <= newed)).FirstOrDefault();
                if (check != null)
                {
                    Resp.msg = AppResource.Slot_added;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
                else
                {
                    var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                    DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                    {
                        ToTime = data.ToTime,
                        FromTime = data.ToTime.AddMinutes(data.Duration),
                        SlotType = data.SlotType,
                        DocId = data.DocId,
                        Status = 0,
                        Docavailable = data.Docavailable,
                        Patientavailable = data.Patientavailable,
                        Nurseavailable = data.Nurseavailable,
                        Pharmaavailable = data.Pharmaavailable,
                        DoctorType = Doctor.DocCategory,
                        MaxCount = data.MaxCount,
                        Type = data.Type,
                        Subject = data.Subject,
                        Duration = data.Duration
                    };
                    _db.DoctorAvailibiltyforAppointment.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        var timeUtc = data.ToTime;
                        var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                        string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                        List<Specializion> Expert = new List<Specializion>();
                        Expert = AppDbCommonLogic.GetDoctorSpeciality(Doctor.Id, Lang);
                        string Specility = "";
                        if (Expert.Count() > 0)
                        {
                            foreach (var item in Expert)
                            {
                                if (Specility != "")
                                {
                                    Specility = Specility + "," + item.Specialization;
                                }
                                else
                                {
                                    Specility = item.Specialization;
                                }
                            }
                        }
                        var users = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault();
                        AppCommonLogic.SendTimeCreationEmailToHelpdesk(Doctor.SecondName + " " + Doctor.FirstName, Doctor.DocCategory, Specility, users.Email, users.PhoneNo, formattedDate, data.Duration, data.SlotType, data.Subject);
                        AppDbCommonLogic.InsertUserLog(Doctor.UserId, "Add New Time Slot", "Success");
                        Resp.msg = AppResource.user_Added;
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.msg = AppResource.appointment_failed;
                        Resp.code = (int)ApiCustomResponseCode.BadRequest;
                        return Ok(Resp);
                    }
                }
            }
            catch (Exception ex)
            {
                var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public ActionResult GetDoctorNewTimeMangementList(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            freeslot data = new freeslot();
            try
            {
                data.getList = (from appbk in _db.DoctorAvailibiltyforAppointment.OrderByDescending(a => a.Id)
                                where appbk.DocId == DocId && appbk.ToTime >= DateTime.UtcNow && appbk.Status == 0
                                select new Bookfreeavailbility
                                {
                                    Id = appbk.Id,
                                    ToTime = appbk.ToTime,
                                    FromTime = appbk.FromTime,
                                    DocId = appbk.DocId,
                                    Name = _db.Doctors.Where(a => a.Id == appbk.DocId).FirstOrDefault().FirstName,
                                    Duration = (int)appbk.Duration,
                                    Type = appbk.Type,
                                    MaxCount = (int)appbk.MaxCount,
                                    Subject = appbk.Subject
                                    //Title = "(" + appbk.ToTime.ToString("HH:mm") + "-" + appbk.FromTime.ToString("HH:mm") + ")"
                                }).ToList();
                var doctorcount = _db.PharmaMaxConsultationCount.FirstOrDefault();
                if (doctorcount != null)
                {
                    data.DoctorCallCount = (int)doctorcount.CallCount;
                }
                else
                {
                    data.DoctorCallCount = 0;
                }
                if (data.getList.Count > 0)
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
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorNewTimeSlotList(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.DocId != DocId && a.SlotType == "other").OrderByDescending(a => a.ToTime).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                       AppointmentDate = r.ToTime,
                                                       AppointmentType = r.Type,
                                                       Duration = (int)r.Duration,
                                                       MaxCount = (int)r.MaxCount,
                                                       Subject = r.Subject,
                                                       SlotId = r.Id,
                                                       DoctorId = r.DocId,
                                                       PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                                       AppointmentCount = AppDbCommonLogic.GetAppointmentCount(r.Id),
                                                       Status = r.Status,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Id = sp.CatId,
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                                       ToTime = r.ToTime,
                                                       FromTime = r.FromTime,
                                                       bookingawailibility = AppDbCommonLogic.CheckDoctorRemainingAailability(r.DocId)
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.bookingawailibility == false).OrderBy(m => m.AppointmentDate).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpPost]
        public ActionResult BookDoctorNewAppointment()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            BookPharmaApp data = new BookPharmaApp();
            var files = HttpContext.Request.Form.Files;
            data.AppointmentTitle = HttpContext.Request.Form["AppointmentTitle"];
            data.DoctorConsultationId = Convert.ToInt32(HttpContext.Request.Form["DoctorConsultationId"]);
            data.DoctorId = Convert.ToInt32(HttpContext.Request.Form["DoctorId"]);
            data.SlotId = Convert.ToInt32(HttpContext.Request.Form["SlotId"]);
            data.Duration = Convert.ToInt32(HttpContext.Request.Form["Duration"]);
            data.StartDate = Convert.ToDateTime(HttpContext.Request.Form["StartDate"]);
            string Name = HttpContext.Request.Form["Name"];
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
            AppCode = "DR-R" + "-" + RN.Next(100000, 999999) + count;
            DateTime GetUserTime = DateTime.UtcNow.AddHours(2);
            int GetMaxCount = (int)_db.DoctorAvailibiltyforAppointment.Where(a => a.Id == data.SlotId).FirstOrDefault().MaxCount;
            int AppCount = AppDbCommonLogic.GetAppointmentCount(data.SlotId);
            int row = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    if (GetMaxCount >= AppCount)
                    {
                        DateTime EndDate = data.StartDate.AddMinutes(data.Duration);
                        if (GetUserTime > data.StartDate)
                        {
                            resp.msg = AppResource.twohourbefore;
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                        else
                        {

                            var Slotbook = _db.Appoinments.Where(a => a.DoctorId == data.DoctorId && a.IsActive != 0 && a.DocSlotId == data.SlotId || a.DoctorConsultationId == data.DoctorId && a.DocSlotId == data.SlotId && a.IsActive != 0).FirstOrDefault();
                            if (Slotbook == null)
                            {
                                var checkReferalDoctor = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorConsultationId == data.DoctorConsultationId && a.IsActive != 0 && ((data.StartDate >= a.StartDate && data.StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                                if (checkReferalDoctor != null)
                                {
                                    AppCode = checkReferalDoctor.Appoinmentcode;
                                }
                                Appoinments obj = new Appoinments()
                                {
                                    StartDate = data.StartDate,
                                    EndDate = data.StartDate.AddMinutes(data.Duration),
                                    BookingDate = DateTime.UtcNow,
                                    AppointmentTitle = data.AppointmentTitle,
                                    Duration = data.Duration,
                                    DoctorId = data.DoctorId,
                                    DoctorConsultationId = data.DoctorConsultationId,
                                    Appoinmentcode = AppCode,
                                    IsActive = 1,
                                    DocSlotId = data.SlotId,
                                    UserRole = 2,
                                    MyRole = 8,
                                    ColorCode = getRandColor(),
                                };
                                _db.Appoinments.Add(obj);
                                row = _db.SaveChanges();
                                if (row > 0)
                                {
                                    AppDbCommonLogic.InsertDoctorDoctorActivity(obj.Id, data.DoctorConsultationId, data.DoctorId);
                                    AppDbCommonLogic.SendAppointmentBookingConfirmation(obj.Id);
                                    var doctor = _db.Doctors.Where(a => a.Id == data.DoctorId).FirstOrDefault();
                                    AppDbCommonLogic.InsertUserLog(doctor.UserId, "Appointment booked with doctor " + data.StartDate, "Success");
                                    //foreach (var Image in files)
                                    //{
                                    //    if (Image != null && Image.Length > 0)
                                    //    {
                                    //        var Extetion = Path.GetExtension(files[0].FileName);
                                    //        using (var ms = new MemoryStream())
                                    //        {
                                    //            Image.CopyTo(ms);
                                    //            var fileBytes = ms.ToArray();
                                    //            string convertbyte = Convert.ToBase64String(fileBytes);
                                    //            AppointmentDocuments doc = new AppointmentDocuments
                                    //            {
                                    //                AppointmetId = obj.Id,
                                    //                DocUrl = new AES_ALGORITHM().Encrypt(convertbyte),
                                    //                Isseen = 0,
                                    //                Filetype = Extetion,
                                    //                Usertype = 8,
                                    //                MeetingId = AppCode,
                                    //                DocId = data.DoctorId
                                    //            };
                                    //            _db.AppointmentDocuments.Add(doc);
                                    //            _db.SaveChanges();
                                    //        }
                                    //    }
                                    //}
                                    foreach (var Image in files)
                                    {
                                        if (Image != null && Image.Length > 0)
                                        {
                                            var file = Image;
                                            var uploads = Path.Combine(_appEnvironment.WebRootPath, @"TempImages");
                                            var Extetion = Path.GetExtension(files[0].FileName);
                                            if (file.Length > 0)
                                            {
                                                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                                                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                                {
                                                    file.CopyToAsync(fileStream);
                                                    string path = "/TempImages/" + fileName;
                                                    AppointmentDocuments doc = new AppointmentDocuments
                                                    {
                                                        AppointmetId = obj.Id,
                                                        DocUrl = path,
                                                        Isseen = 0,
                                                        Filetype = Extetion,
                                                        Usertype = 8,
                                                        MeetingId = AppCode,
                                                        DocId = data.DoctorId,
                                                        FileName = Name,
                                                    };
                                                    _db.AppointmentDocuments.Add(doc);
                                                    _db.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                    resp.msg = AppResource.appointment_success;
                                    resp.code = (int)ApiCustomResponseCode.Ok;
                                }
                            }
                            else
                            {
                                resp.msg = AppResource.pharma_slot;
                                resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(resp);
                            }
                        }
                    }
                    else
                    {
                        resp.msg = AppResource.MaxAccesseedCallCount;
                        resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(resp);
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
                var doctor = _db.Doctors.Where(a => a.Id == data.DoctorId).FirstOrDefault();
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                resp.msg = ex.Message;
                AppDbCommonLogic.InsertUserLog(doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                return Ok(resp);
            }
            return Ok(resp);
        }

        private string getRandColor()
        {
            Random rnd = new Random();
            string hexOutput = String.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorsForCollegeus(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.Doctors.Where(a => a.Status == 1 && a.Id != DocId && a.IsValidated == true).OrderByDescending(a => a.Id).ToList();
                if (doc.Count > 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.Id).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.Id),
                                                       SlotId = r.Id,
                                                       PhotoUrl = r.PhotoUrl,
                                                       Status = r.Status,
                                                       Awailability = AppDbCommonLogic.CheckTimeawailability(r.Id),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.Id && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Id = sp.CatId,
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       MaxCallCount = r.MaxCallCount,
                                                       awilablecallcountmonth = AppDbCommonLogic.CheckMonthAVAILABLEaPPOINTMENT(r.Id)
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpPost]
        public ActionResult UpdateCallAttendDuration(Appointmentscall data)
        {
            Appointmentscall app = new Appointmentscall();
            app.code = (int)ApiCustomResponseCode.Ok;
            try
            {
                var details = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId && a.DoctorId == data.DocId).ToList();
                if (details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        if (item.Calllength != null)
                        {
                            item.Calllength = item.Calllength + 1;
                        }
                        else
                        {
                            item.Calllength = 1;
                        }
                    }
                    _db.SaveChanges();
                    var appointment = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).FirstOrDefault();
                    if (appointment != null)
                    {
                        app.Calllength = (int)appointment.Calllength;
                    }
                }
            }
            catch (Exception ex)
            {
                app.code = (int)ApiCustomResponseCode.Ok;
            }
            return Ok(app);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorNewTimeSlotListforChat(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.DocId == DocId && a.SlotType == "other").OrderByDescending(a => a.ToTime).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                       AppointmentDate = r.ToTime,
                                                       AppointmentType = r.Type,
                                                       Duration = (int)r.Duration,
                                                       MaxCount = (int)r.MaxCount,
                                                       Subject = r.Subject,
                                                       SlotId = r.Id,
                                                       DoctorId = r.DocId,
                                                       PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                                       AppointmentCount = AppDbCommonLogic.GetAppointmentCount(r.Id),
                                                       Status = r.Status,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                                       ToTime = r.ToTime,
                                                       FromTime = r.FromTime,
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.OrderBy(m => m.AppointmentDate).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        public ActionResult GetMyclickInfoodetails()
        {
            Info data = new Info();
            var details = _db.MyclickdoctorInfo.FirstOrDefault();
            if (details != null)
            {
                data.Infomation = details.Info;
            }
            data.msg = AppResource.user_listS;
            data.code = (int)ApiCustomResponseCode.Ok;
            return Ok(data);
        }
        [HttpGet]
        public ActionResult GetDashBoardBookedAppointment(int DocId)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            List<doctor> weekCnsList = new List<doctor>();
            weekCnsList = (from ta2 in _db.Appoinments.OrderByDescending(a => a.StartDate)
                           where ta2.EndDate >= DateTime.UtcNow && ta2.DoctorConsultationId == DocId && ta2.IsActive != 0
                           select new doctor
                           {
                               AppointmentId = ta2.Id,
                               Id = ta2.DocSlot.DocId,
                               UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == ta2.DocSlot.DocId).FirstOrDefault().UserId,
                               FirstName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta2.Id, ta2.UserRole, ta2.Appoinmentcode),
                               AppointmentDate = ta2.DocSlot.ToTime,
                               AppointmentType = ta2.DocSlot.Type,
                               Duration = (int)ta2.DocSlot.Duration,
                               MaxCount = (int)ta2.DocSlot.MaxCount,
                               Subject = ta2.DocSlot.Subject,
                               SlotId = ta2.DocSlot.Id,
                               DoctorId = ta2.DocSlot.DocId,
                               AppointmentCode = ta2.Appoinmentcode,
                               PhotoUrl = AppDbCommonLogic.GetAppintmentUserImage(ta2.Id, ta2.UserRole),
                               AppointmentCount = AppDbCommonLogic.GetAppointmentCount(ta2.DocSlot.Id),
                               Status = ta2.DocSlot.Status,
                               BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == ta2.DocSlot.DocId).FirstOrDefault().UserId),
                               Expert = (from sp in _db.Doctorspecilization
                                         where sp.DocId == ta2.DocSlot.DocId && sp.Status == 1 && sp.Name != null
                                         select new Specializion
                                         {
                                             Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                             HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                         }).ToList(),
                               Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(ta2.DocSlot.DocId),
                               ToTime = ta2.DocSlot.ToTime,
                               FromTime = ta2.DocSlot.FromTime,
                               Booked = 0
                           }).ToList();
            GetSpeCat data = new GetSpeCat();
            try
            {

                data.SpecializationCategory = (from ta2 in _db.Appoinments.OrderByDescending(a => a.StartDate)
                                               where ta2.EndDate >= DateTime.UtcNow && ta2.DoctorId == DocId && ta2.IsActive != 0/*||(ta2.StartDate.Date >= DateTime.UtcNow.Date && ta2.StartDate <= DateTime.UtcNow.Date.AddDays(7) && ta2.DoctorConsultationId == dsh.Id && ta2.IsActive != 0)*/
                                               select new doctor
                                               {
                                                   AppointmentId = ta2.Id,
                                                   Id = ta2.DocSlot.DocId,
                                                   UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == ta2.DocSlot.DocId).FirstOrDefault().UserId,
                                                   FirstName = AppDbCommonLogic.GetAppintmentUserNameforDoctor(ta2.Id, ta2.MyRole, ta2.Appoinmentcode),
                                                   AppointmentDate = ta2.DocSlot.ToTime,
                                                   AppointmentType = ta2.DocSlot.Type,
                                                   Duration = (int)ta2.DocSlot.Duration,
                                                   MaxCount = (int)ta2.DocSlot.MaxCount,
                                                   Subject = ta2.DocSlot.Subject,
                                                   SlotId = ta2.DocSlot.Id,
                                                   DoctorId = ta2.DocSlot.DocId,
                                                   AppointmentCode = ta2.Appoinmentcode,
                                                   PhotoUrl = AppDbCommonLogic.GetAppintmentUserImage(ta2.Id, ta2.MyRole),
                                                   AppointmentCount = AppDbCommonLogic.GetAppointmentCount(ta2.DocSlot.Id),
                                                   Status = ta2.DocSlot.Status,
                                                   BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == ta2.DocSlot.DocId).FirstOrDefault().UserId),
                                                   Expert = (from sp in _db.Doctorspecilization
                                                             where sp.DocId == ta2.DocSlot.DocId && sp.Status == 1 && sp.Name != null
                                                             select new Specializion
                                                             {
                                                                 Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                 HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                             }).ToList(),
                                                   Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(ta2.DocSlot.DocId),
                                                   ToTime = ta2.DocSlot.ToTime,
                                                   FromTime = ta2.DocSlot.FromTime,
                                                   Booked = 0
                                               }).ToList();

                if (weekCnsList.Count > 0)
                {
                    data.SpecializationCategory.AddRange(weekCnsList);
                    data.SpecializationCategory = data.SpecializationCategory.OrderByDescending(a => a.AppointmentDate).GroupBy(x => x.AppointmentCode).Select(x => x.First()).Take(6).ToList();
                }

                else if (data.SpecializationCategory.Count > 0)
                {
                    data.SpecializationCategory = data.SpecializationCategory.OrderByDescending(a => a.AppointmentDate).GroupBy(x => x.AppointmentCode).Select(x => x.First()).Take(6).ToList();

                    if (data.SpecializationCategory != null)
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(resp);
            }
            return Ok(data);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorRecomndedTimeslot(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            var Doclist = _db.Appoinments.Where(x => x.DoctorId == DocId && x.IsActive != 0).ToList();
            try
            {
                if (Doclist != null)
                {
                    var getList = Doclist.GroupBy(x => x.DoctorConsultationId).Select(x => x.First()).ToList();
                    foreach (var item in getList)
                    {
                        var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.DocId == item.DoctorConsultationId && a.SlotType == "other").OrderByDescending(a => a.ToTime).ToList();
                        if (doc.Count != 0)
                        {
                            data.SpecializationCategory = (from r in doc
                                                           select new doctor
                                                           {
                                                               Id = r.Id,
                                                               UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                               FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                               AppointmentDate = r.ToTime,
                                                               AppointmentType = r.Type,
                                                               Duration = (int)r.Duration,
                                                               MaxCount = (int)r.MaxCount,
                                                               Subject = r.Subject,
                                                               SlotId = r.Id,
                                                               DoctorId = r.DocId,
                                                               PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                                               AppointmentCount = AppDbCommonLogic.GetAppointmentCount(r.Id),
                                                               Status = r.Status,
                                                               BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                               Expert = (from sp in _db.Doctorspecilization
                                                                         where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                         select new Specializion
                                                                         {
                                                                             Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                             HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                         }).ToList(),
                                                               Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                                               ToTime = r.ToTime,
                                                               FromTime = r.FromTime,
                                                               Booked = 1,
                                                               bookingawailibility = AppDbCommonLogic.CheckDoctorRemainingAailability(r.DocId)
                                                           }).ToList();
                        }
                        list.AddRange(data.SpecializationCategory);
                    }
                    if (list.Count > 0)
                    {
                        data.SpecializationCategory = list.Where(a => a.bookingawailibility == false).OrderBy(m => m.AppointmentDate).GroupBy(x => x.Id).Select(x => x.First()).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> CreatetimeslotforDoctor(Bookfreeavailbility data)
        {
            ApiResponse Resp = new ApiResponse();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                var Newst = data.ToTime;
                var newed = data.ToTime.AddMinutes(data.Duration);
                Random RN = new Random();
                string AppCode = "DR-R" + "-" + RN.Next(100000, 999999) + 1;
                DateTime EndDate = data.ToTime.AddMinutes(data.Duration);
                var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == data.DocId && a.Status == 0 && (a.ToTime <= Newst && a.FromTime >= newed || a.FromTime >= Newst && a.ToTime <= newed)).FirstOrDefault();
                if (check != null)
                {
                    Resp.msg = AppResource.Slot_added;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
                else
                {
                    var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == data.ToDocId && a.IsActive != 0 && ((data.ToTime >= a.StartDate && data.ToTime <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    if (Slotbook == null)
                    {
                        var checkReferalDoctor = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorConsultationId == data.DocId && a.IsActive != 0 && ((data.ToTime >= a.StartDate && data.ToTime <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                        if (checkReferalDoctor != null)
                        {
                            AppCode = checkReferalDoctor.Appoinmentcode;
                        }
                        var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                        DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                        {
                            ToTime = data.ToTime,
                            FromTime = data.ToTime.AddMinutes(data.Duration),
                            SlotType = data.SlotType,
                            DocId = data.DocId,
                            Status = 0,
                            Docavailable = data.Docavailable,
                            Patientavailable = data.Patientavailable,
                            Nurseavailable = data.Nurseavailable,
                            Pharmaavailable = data.Pharmaavailable,
                            DoctorType = Doctor.DocCategory,
                            MaxCount = data.MaxCount,
                            Type = data.Type,
                            Subject = data.Subject,
                            Duration = data.Duration
                        };
                        _db.DoctorAvailibiltyforAppointment.Add(obj);
                        int i = await _db.SaveChangesAsync();
                        if (i > 0)
                        {

                            Appoinments obj1 = new Appoinments()
                            {
                                StartDate = data.ToTime,
                                EndDate = data.ToTime.AddMinutes(data.Duration),
                                BookingDate = DateTime.UtcNow,
                                AppointmentTitle = data.Subject,
                                Duration = data.Duration,
                                DoctorId = data.ToDocId,
                                DoctorConsultationId = data.DocId,
                                Appoinmentcode = AppCode,
                                IsActive = 1,
                                DocSlotId = obj.Id,
                                UserRole = 2,
                                MyRole = 8
                            };
                            _db.Appoinments.Add(obj1);
                            int row = _db.SaveChanges();
                            if (row > 0)
                            {
                                int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                AppDbCommonLogic.InsertDoctorDoctorActivity(AppId, data.DocId, data.ToDocId);
                                AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "Add New Time Slot", "Success");
                                var timeUtc = data.ToTime;
                                var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                                string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                                List<Specializion> Expert = new List<Specializion>();
                                Expert = AppDbCommonLogic.GetDoctorSpeciality(Doctor.Id, Lang);
                                string Specility = "";
                                if (Expert.Count() > 0)
                                {
                                    foreach (var item in Expert)
                                    {
                                        if (Specility != "")
                                        {
                                            Specility = Specility + "," + item.Specialization;
                                        }
                                        else
                                        {
                                            Specility = item.Specialization;
                                        }
                                    }
                                }
                                var users = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault();
                                AppCommonLogic.SendTimeCreationEmailToHelpdesk(Doctor.SecondName + " " + Doctor.FirstName, Doctor.DocCategory, Specility, users.Email, users.PhoneNo, formattedDate, data.Duration, data.SlotType, data.Subject);
                            }
                            Resp.msg = AppResource.user_Added;
                            Resp.code = (int)ApiCustomResponseCode.Ok;
                            return Ok(Resp);
                        }
                        else
                        {
                            Resp.msg = AppResource.appointment_failed;
                            Resp.code = (int)ApiCustomResponseCode.BadRequest;
                            return Ok(Resp);
                        }
                    }
                    else
                    {
                        Resp.msg = AppResource.user_slot;
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(Resp);
                    }
                }
            }
            catch (Exception ex)
            {
                var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorNewTimeSlotListforPatient()
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.SlotType == "patient").OrderByDescending(a => a.ToTime).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                       AppointmentDate = r.ToTime,
                                                       AppointmentType = r.Type,
                                                       Duration = (int)r.Duration,
                                                       MaxCount = (int)r.MaxCount,
                                                       Subject = r.Subject,
                                                       SlotId = r.Id,
                                                       DoctorId = r.DocId,
                                                       PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                                       AppointmentCount = AppDbCommonLogic.GetAppointmentCount(r.Id),
                                                       Status = r.Status,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckTimeawailabilityforpatient(r.DocId),
                                                       ToTime = r.ToTime,
                                                       FromTime = r.FromTime,
                                                       bookingawailibility = AppDbCommonLogic.CheckDoctorRemainingAailability(r.DocId)
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.bookingawailibility == false).OrderBy(m => m.AppointmentDate).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetIndividualDoctorTimeSlotListforPatient(int DocId)
        {
            List<doctor> list = new List<doctor>();
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            GetSpeCat data = new GetSpeCat();
            try
            {
                var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.SlotType == "patient" && a.DocId == DocId).OrderByDescending(a => a.ToTime).ToList();
                if (doc.Count != 0)
                {
                    data.SpecializationCategory = (from r in doc
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                                       FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                                       AppointmentDate = r.ToTime,
                                                       AppointmentType = r.Type,
                                                       Duration = (int)r.Duration,
                                                       MaxCount = (int)r.MaxCount,
                                                       Subject = r.Subject,
                                                       SlotId = r.Id,
                                                       DoctorId = r.DocId,
                                                       PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                                       AppointmentCount = AppDbCommonLogic.GetAppointmentCount(r.Id),
                                                       Status = r.Status,
                                                       BussinessStatus = AppDbCommonLogic.GetBussinessStatus(_db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId),
                                                       Expert = (from sp in _db.Doctorspecilization
                                                                 where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                                 select new Specializion
                                                                 {
                                                                     Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                                     HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                                 }).ToList(),
                                                       Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                                       ToTime = r.ToTime,
                                                       FromTime = r.FromTime,
                                                       bookingawailibility = AppDbCommonLogic.CheckDoctorRemainingAailability(r.DocId)
                                                   }).ToList();
                    if (data.SpecializationCategory.Count > 0)
                    {
                        data.SpecializationCategory = data.SpecializationCategory.Where(a => a.bookingawailibility == false).OrderBy(m => m.AppointmentDate).ToList();
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> CreatetimeslotforPatient(Bookfreeavailbility data)
        {
            ApiResponse Resp = new ApiResponse();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                var Newst = data.ToTime;
                var newed = data.ToTime.AddMinutes(data.Duration);
                Random RN = new Random();
                string AppCode = "DR-R" + "-" + RN.Next(100000, 999999) + 1;
                DateTime EndDate = data.ToTime.AddMinutes(data.Duration);
                var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == data.DocId && a.Status == 0 && (a.ToTime <= Newst && a.FromTime >= newed || a.FromTime >= Newst && a.ToTime <= newed)).FirstOrDefault();
                if (check != null)
                {
                    Resp.msg = AppResource.Slot_added;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
                else
                {
                    var Slotbook = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.PatientId == data.ToDocId && a.IsActive != 0 && ((data.ToTime >= a.StartDate && data.ToTime <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                    if (Slotbook == null)
                    {
                        var checkReferalDoctor = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == data.DocId && a.IsActive != 0 && ((data.ToTime >= a.StartDate && data.ToTime <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                        if (checkReferalDoctor != null)
                        {
                            Resp.msg = AppResource.user_slot;
                            Resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(Resp);
                        }
                        var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                        DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                        {
                            ToTime = data.ToTime,
                            FromTime = data.ToTime.AddMinutes(data.Duration),
                            SlotType = data.SlotType,
                            DocId = data.DocId,
                            Status = 0,
                            Docavailable = data.Docavailable,
                            Patientavailable = data.Patientavailable,
                            Nurseavailable = data.Nurseavailable,
                            Pharmaavailable = data.Pharmaavailable,
                            DoctorType = Doctor.DocCategory,
                            MaxCount = data.MaxCount,
                            Type = data.Type,
                            Subject = data.Subject,
                            Duration = data.Duration
                        };
                        _db.DoctorAvailibiltyforAppointment.Add(obj);
                        int i = await _db.SaveChangesAsync();
                        if (i > 0)
                        {

                            Appoinments obj1 = new Appoinments()
                            {
                                StartDate = data.ToTime,
                                EndDate = data.ToTime.AddMinutes(data.Duration),
                                BookingDate = DateTime.UtcNow,
                                AppointmentTitle = data.Subject,
                                Duration = data.Duration,
                                PatientId = data.ToDocId,
                                DoctorId = data.DocId,
                                Appoinmentcode = AppCode,
                                IsActive = 1,
                                DocSlotId = obj.Id,
                                UserRole = 2,
                                MyRole = 4,
                                ColorCode = getRandColor()
                            };
                            _db.Appoinments.Add(obj1);
                            int row = _db.SaveChanges();
                            if (row > 0)
                            {
                                int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                AppDbCommonLogic.InsertPatientDoctorActivity(AppId, data.ToDocId, data.DocId);
                                AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "Add New Time Slot", "Success");
                            }
                            Resp.msg = AppResource.user_Added;
                            Resp.code = (int)ApiCustomResponseCode.Ok;
                            return Ok(Resp);
                        }
                        else
                        {
                            Resp.msg = AppResource.appointment_failed;
                            Resp.code = (int)ApiCustomResponseCode.BadRequest;
                            return Ok(Resp);
                        }
                    }
                    else
                    {
                        Resp.msg = AppResource.user_slot;
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(Resp);
                    }
                }
            }
            catch (Exception ex)
            {
                var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatetimeslotforGroups(Bookfreeavailbility data)
        {
            ApiResponse Resp = new ApiResponse();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                var Newst = data.ToTime;
                var newed = data.ToTime.AddMinutes(data.Duration);
                Random RN = new Random();
                string AppCode = "DR-R" + "-" + RN.Next(100000, 999999) + 1;
                DateTime EndDate = data.ToTime.AddMinutes(data.Duration);
                var check = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == data.DocId && a.Status == 0 && (a.ToTime <= Newst && a.FromTime >= newed || a.FromTime >= Newst && a.ToTime <= newed)).FirstOrDefault();
                if (check != null)
                {
                    Resp.msg = AppResource.Slot_added;
                    Resp.code = (int)ApiCustomResponseCode.Error;
                    return Ok(Resp);
                }
                else
                {
                    var grouplist = _db.Groupchat.Where(a => a.ConnectionId == data.ConnectionId).ToList();
                    var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                    DoctorAvailibiltyforAppointment obj = new DoctorAvailibiltyforAppointment()
                    {
                        ToTime = data.ToTime,
                        FromTime = data.ToTime.AddMinutes(data.Duration),
                        SlotType = data.SlotType,
                        DocId = data.DocId,
                        Status = 0,
                        Docavailable = data.Docavailable,
                        Patientavailable = data.Patientavailable,
                        Nurseavailable = data.Nurseavailable,
                        Pharmaavailable = data.Pharmaavailable,
                        DoctorType = Doctor.DocCategory,
                        MaxCount = grouplist.Count(),
                        Type = data.Type,
                        Subject = data.Subject,
                        Duration = data.Duration
                    };
                    _db.DoctorAvailibiltyforAppointment.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        if (grouplist.Count > 0)
                        {
                            foreach(var item in grouplist)
                            {
                                if(item.ParticipateId!= Doctor.UserId)
                                {
                                    int DoctorId = _db.Doctors.Where(a => a.UserId == item.ParticipateId).FirstOrDefault().Id;
                                    Appoinments obj1 = new Appoinments()
                                    {
                                        StartDate = data.ToTime,
                                        EndDate = data.ToTime.AddMinutes(data.Duration),
                                        BookingDate = DateTime.UtcNow,
                                        AppointmentTitle = data.Subject,
                                        Duration = data.Duration,
                                        DoctorId = DoctorId,
                                        DoctorConsultationId = data.DocId,
                                        Appoinmentcode = AppCode,
                                        IsActive = 1,
                                        DocSlotId = obj.Id,
                                        UserRole = 2,
                                        MyRole = 8
                                    };
                                    _db.Appoinments.Add(obj1);
                                    int row = _db.SaveChanges();
                                    if (row > 0)
                                    {
                                        int AppId = _db.Appoinments.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                        AppDbCommonLogic.InsertDoctorDoctorActivity(AppId, data.DocId, DoctorId);
                                        AppDbCommonLogic.SendAppointmentBookingConfirmation(AppId);
                                        AppDbCommonLogic.InsertUserLog(Doctor.UserId, "Add New Time Slot", "Success");
                                        var timeUtc = data.ToTime;
                                        var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                        var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                                        string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                                        List<Specializion> Expert = new List<Specializion>();
                                        Expert = AppDbCommonLogic.GetDoctorSpeciality(Doctor.Id, Lang);
                                        string Specility = "";
                                        if (Expert.Count() > 0)
                                        {
                                            foreach (var items in Expert)
                                            {
                                                if (Specility != "")
                                                {
                                                    Specility = Specility + "," + items.Specialization;
                                                }
                                                else
                                                {
                                                    Specility = items.Specialization;
                                                }
                                            }
                                        }
                                        var users = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault();
                                        AppCommonLogic.SendTimeCreationEmailToHelpdesk(Doctor.SecondName + " " + Doctor.FirstName, Doctor.DocCategory, Specility, users.Email, users.PhoneNo, formattedDate, data.Duration, data.SlotType, data.Subject);
                                    }

                                }

                            }
                            Resp.msg = AppResource.user_Added;
                            Resp.code = (int)ApiCustomResponseCode.Ok;
                            return Ok(Resp);
                        }
                       
                    }
                    else
                    {
                        Resp.msg = AppResource.appointment_failed;
                        Resp.code = (int)ApiCustomResponseCode.BadRequest;
                        return Ok(Resp);
                    }
                }
            }
            catch (Exception ex)
            {
                var Doctor = _db.Doctors.Where(a => a.Id == data.DocId).FirstOrDefault();
                AppDbCommonLogic.InsertUserLog(Doctor.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                Resp.msg = ex.Message;
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(Resp);
            }
            return Ok(Resp);
        }
    }
}


