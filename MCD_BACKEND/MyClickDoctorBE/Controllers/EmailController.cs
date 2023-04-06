using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using MyClickDoctorBE.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyClickDoctorBE.Models.AppCommonLogic;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class EmailController : ControllerBase
    {
        private readonly MyClickDoctorV4 _db;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ILogger<EmailController> _log;
        private readonly IRazorViewToStringRenderer _renderer;
        private readonly IEmailService _emailService;
        public EmailController(MyClickDoctorV4 context, IHostingEnvironment appEnvironment, ILogger<EmailController> log, IRazorViewToStringRenderer renderer, IEmailService emailService)
        {
            _db = context;
            _appEnvironment = appEnvironment;
            _log = log;
            _renderer = renderer;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorToSendEmail()
        {
            List<doctor> list = new List<doctor>();
            int count = 0;
            try
            {
                var DocList = _db.Users.Where(a => a.UserType == 2 && a.Unscubscribe != 1 && Convert.ToDateTime(a.EmailUpdatedAt).Date != DateTime.UtcNow.Date && a.Status == 1).ToList();
                count = DocList.Count();
                if (DocList.Count() > 0)
                {
                    foreach (var item in DocList)
                    {
                        var doctor = _db.Doctors.Where(a => a.UserId == item.Id).FirstOrDefault();
                        if (doctor != null)
                        {
                            var doc = _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.ToTime > DateTime.UtcNow && a.DocId != doctor.Id && a.SlotType == "other").OrderByDescending(a => a.ToTime).ToList();
                            if (doc.Count() > 0)
                            {
                                list = (from r in doc
                                        select new doctor
                                        {
                                            Id = r.Id,
                                            UserId = _db.Doctors.Where(a => a.Status == 1 && a.Id == r.DocId).FirstOrDefault().UserId,
                                            FirstName = AppDbCommonLogic.GetDoctorName(r.DocId),
                                            AppointmentDate = r.ToTime,
                                            Date = AppDbCommonLogic.CovertDatetoHungaryTime(r.ToTime),
                                            Duration = (int)r.Duration,
                                            SlotId = r.Id,
                                            DoctorId = r.DocId,
                                            PhotoUrl = AppDbCommonLogic.GetDoctorImage(r.DocId),
                                            Expert = (from sp in _db.Doctorspecilization
                                                      where sp.DocId == r.DocId && sp.Status == 1 && sp.Name != null
                                                      select new Specializion
                                                      {
                                                          Specialization = AppDbCommonLogic.GetEnglishDoctorSpecilization(sp.CatId),
                                                          HungName = AppDbCommonLogic.GetHungryDoctorSpecilization(sp.CatId)
                                                      }).ToList(),
                                            Awailability = AppDbCommonLogic.CheckDoctortoDoctorAvailability(r.DocId),
                                        }).ToList();
                                string UserIds = item.Id.ToString();
                                string MyId = new AES_ALGORITHM().Encrypt(UserIds);
                                var model = new EmailCampaignModel(doctor.SecondName + " " + doctor.FirstName, list, MyId);
                                const string view = "/Templates/_KOLEmail";
                                var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                _emailService.SendEmail("Új konzultációs időpontok a myclickdoctorban", htmlBody, item.Email, true, _appEnvironment.WebRootPath);
                            }
                            item.EmailUpdatedAt = DateTime.UtcNow;
                            _db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(count);
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointmentFifteenMinEmail()
        {
            int Count = 0;
            try
            {
                var status = _db.PharmaMaxConsultationCount.FirstOrDefault();
                if (status.Fifteenmin == true)
                {
                    var todayDate = DateTime.UtcNow;
                    var appointments = _db.Appoinments.Where(a => a.StartDate.Date == todayDate.Date && a.IsFifteenEmail != true).ToList();
                    Count = appointments.Count();
                    appointments = appointments.GroupBy(a => a.Appoinmentcode).Select(x => x.First()).ToList();
                    if (appointments.Count() > 0)
                    {
                        foreach (var item in appointments)
                        {
                            TimeSpan span = item.StartDate.TimeOfDay - todayDate.TimeOfDay;
                            int SendEmailMin = Math.Abs((int)span.TotalMinutes);
                            if (SendEmailMin <= 15 && SendEmailMin >= 10)
                            {
                                if (item.IsFifteenEmail != true)
                                {
                                    AppDbCommonLogic.UpdatemeetingFifteenmintusstatus(item.Appoinmentcode);
                                    string appointmentId = item.Id.ToString();
                                    string AppId = new AES_ALGORITHM().Encrypt(appointmentId);
                                    var timeUtc = item.StartDate;
                                    var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                    var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                                    string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                                    if (item.UserRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.UserRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, SendEmailMin + " Minutes", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        //   AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath, user.Email);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                    if (item.MyRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.MyRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, SendEmailMin + " Minutes", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        //   AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath, user.Email);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath, ex.Message);
                return Ok(Count);
            }
            return Ok(Count);
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointmentOneHoursEmail()
        {
            //   AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath,"Method Call");
            int Count = 0;
            try
            {
                var status = _db.PharmaMaxConsultationCount.FirstOrDefault();
                if (status.Onehour == true)
                {
                    var todayDate = DateTime.UtcNow;
                    var appointments = _db.Appoinments.Where(a => a.StartDate.Date == todayDate.Date && a.IshourEmail != true).ToList();
                    Count = appointments.Count();
                    appointments = appointments.GroupBy(a => a.Appoinmentcode).Select(x => x.First()).ToList();
                    if (appointments.Count() > 0)
                    {
                        foreach (var item in appointments)
                        {
                            TimeSpan span = item.StartDate.TimeOfDay - todayDate.TimeOfDay;
                            int SendEmailMin = Math.Abs((int)span.TotalMinutes);
                            if (SendEmailMin <= 60 && SendEmailMin >= 55)
                            {
                                if (item.IshourEmail != true)
                                {
                                    AppDbCommonLogic.Updatemeetinghourstatus(item.Appoinmentcode);
                                    string appointmentId = item.Id.ToString();
                                    string AppId = new AES_ALGORITHM().Encrypt(appointmentId);
                                    var timeUtc = item.StartDate;
                                    var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                    var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                                    string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                                    if (item.UserRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.UserRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, "1 hours", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        //  AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath, user.Email);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                    if (item.MyRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.MyRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, "1 hours", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        //  AppDbCommonLogic.LogEmail(_appEnvironment.WebRootPath, user.Email);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(Count);
            }
            return Ok(Count);
        }
        [HttpGet]
        public async Task<IActionResult> GetAppointmenttwentyfourHoursEmail()
        {
            int Count = 0;
            try
            {
                var status = _db.PharmaMaxConsultationCount.FirstOrDefault();
                if (status.Twentyfourhour == true)
                {
                    var todayDate = DateTime.UtcNow;
                    var appointments = _db.Appoinments.Where(a => a.StartDate.Date == todayDate.Date && a.IsdayEmail != true).ToList();
                    Count = appointments.Count();
                    appointments = appointments.GroupBy(a => a.Appoinmentcode).Select(x => x.First()).ToList();
                    if (appointments.Count() > 0)
                    {
                        foreach (var item in appointments)
                        {
                            TimeSpan span = item.StartDate.TimeOfDay - todayDate.TimeOfDay;
                            int SendEmailMin = Math.Abs((int)span.TotalMinutes);
                            if (SendEmailMin <= 1440 && SendEmailMin >= 1435)
                            {
                                if (item.IsdayEmail != true)
                                {
                                    AppDbCommonLogic.Updatemeetingtwentyfourstatus(item.Appoinmentcode);
                                    string appointmentId = item.Id.ToString();
                                    string AppId = new AES_ALGORITHM().Encrypt(appointmentId);
                                    var timeUtc = item.StartDate;
                                    var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                    var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                                    string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
                                    if (item.UserRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.UserRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, "24 hours", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                    if (item.MyRole > 0)
                                    {
                                        int UserId = AppDbCommonLogic.GetAppintmentUserId(item.Id, item.MyRole);
                                        string AppointmentToName = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.MyRole);
                                        string AppointmentWith = AppDbCommonLogic.GetAppintmentUserName(item.Id, item.UserRole);
                                        var doctor = _db.Doctors.Where(a => a.Id == UserId).FirstOrDefault();
                                        var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                                        var Slottype = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == item.DocSlotId).FirstOrDefault();
                                        var model = new RemainderEmail(AppointmentToName, AppId, "24 hours", AppointmentWith, formattedDate, (int)item.Duration, Slottype.SlotType);
                                        const string view = "/Templates/Appointment_Reminder_Email";
                                        var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                                        _emailService.SendEmail("Emlékeztető konzultációról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                                    }
                                }
                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(Count);
            }
            return Ok(Count);
        }
        [HttpGet]
        public async Task<IActionResult> SendEmailtoKOLnobodybookAppoinment()
        {
            try
            {
                var todayDate = DateTime.UtcNow;
                var KOl = _db.DoctorAvailibiltyforAppointment.Where(a => a.ToTime.Date == DateTime.UtcNow.Date && a.Isemail == false).ToList();
                foreach (var item in KOl)
                {
                    var checkapp = _db.Appoinments.Where(a => a.DocSlotId == item.Id && a.IsActive!=0).ToList();
                    if (checkapp.Count == 0)
                    {
                        TimeSpan span = item.ToTime.TimeOfDay - todayDate.TimeOfDay;
                        int SendEmailMin = Math.Abs((int)span.TotalMinutes);
                        if (SendEmailMin <= 122 && SendEmailMin >= 120)
                        {
                            item.Isemail = true;
                            _db.SaveChanges();
                            var timeUtc = item.ToTime;
                            var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                            var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
                            var doctor = _db.Doctors.Where(a => a.Id == item.DocId).FirstOrDefault();
                            var user = _db.Users.Where(a => a.Id == doctor.UserId).FirstOrDefault();
                            var model = new reminderKOl(doctor.SecondName + " " + doctor.FirstName, today, (int)item.Duration,item.Type);
                            const string view = "/Templates/_KolBeforeReminder";
                            var htmlBody = await _renderer.RenderViewToStringAsync($"{view}.cshtml", model);
                            _emailService.SendEmail("Tájékoztatás az Ön által megadott konzultációs időpontról", htmlBody, user.Email, true, _appEnvironment.WebRootPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok();
            }
            return Ok();
        }
    }
}
