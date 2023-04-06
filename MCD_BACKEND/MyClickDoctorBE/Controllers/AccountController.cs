using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using MyClickDoctorBE.App_Resources;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using static MyClickDoctorBE.Models.AppCommonLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AccountController : ControllerBase
    {
        private readonly MyClickDoctorV4 _db;
        private readonly ILogger<AccountController> _log;
        private readonly IHostingEnvironment _appEnvironment;
        public AccountController(MyClickDoctorV4 context, ILogger<AccountController> log, IHostingEnvironment appEnvironment)
        {
            _db = context;
            _log = log;
            _appEnvironment = appEnvironment;
        }
        private bool UsersExists(string email)
        {
            try
            {
                return _db.Users.Any(e => e.Email == email);
            }
            catch (DbUpdateException updExc)
            {
                return false;
            }

        }
        [HttpPost]
        public IActionResult Login(Login log)
        {
            Login resp = new Login();
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                if (ModelState.IsValid)
                {
                    var checkPass = _db.Users.Where(a => a.Email == log.Email.Trim()).FirstOrDefault();
                    if (checkPass != null)
                    {
                        if (checkPass.UserPassword == null)
                        {
                            resp.msg = AppResource.emailverify;
                            resp.code = (int)ApiCustomResponseCode.Error;
                            return Ok(resp);
                        }
                        else
                        {
                            string pass = new AES_ALGORITHM().Encrypt(log.Password);
                            var user = _db.Users.Where(a => a.Email == log.Email.Trim() && a.UserPassword == pass.Trim() && a.UserType == log.Usertype).FirstOrDefault();
                            if (user != null)
                            {
                                if (user.Status == (int)UserStatus.Active)
                                {
                                    resp.msg = AppResource.login_success;
                                    resp.code = (int)ApiCustomResponseCode.Ok;
                                    resp.Token = Guid.NewGuid().ToString();
                                    user.Token = resp.Token;
                                    user.BussinessStatus = 1;
                                    user.Lastlogin = DateTime.UtcNow;
                                    resp.UserId = user.Id;
                                    resp.Usertype = user.UserType;
                                    resp.Phone = user.PhoneNo;
                                    resp.Email = user.Email;
                                    resp.UserIds = AppCommonLogic.ToHexString(user.Id.ToString());
                                    resp.BussinessStatus = (int)user.BussinessStatus;
                                    if (user.UserType == (int)UserType.PharmaRep)
                                    {
                                        var ph = _db.PharmaRepresentative.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = ph.Id;
                                        resp.Name = ph.SecondName + " " + ph.FirstName;
                                        resp.ProfilePhoto = ph.ProfileUrl;
                                        var Comp = _db.PharmaCompanyRepMap.Where(a => a.PharmaRepId == resp.Id && a.Status == 0).FirstOrDefault();
                                        if (Comp != null)
                                        {
                                            resp.PharmaCompId = Comp.PharmaComId;
                                        }
                                        else
                                        {
                                            resp.PharmaCompId = 0;
                                        }
                                    }
                                    else if (user.UserType == (int)UserType.Pharma)
                                    {
                                        var ph = _db.PharmaceuticalsCompany.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = ph.Id;
                                        resp.Name = ph.ComanyName;
                                        resp.ProfilePhoto = ph.ProfileUrl;
                                    }
                                    else if (user.UserType == (int)UserType.Doctor)
                                    {
                                        var dc = _db.Doctors.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = dc.Id;
                                        resp.Name = dc.SecondName + " " + dc.FirstName;
                                        resp.ProfilePhoto = dc.PhotoUrl;
                                        resp.Doctortype = dc.DocCategory;
                                        resp.IsValidated = (bool)dc.IsValidated;
                                        resp.AllowPatient = AppDbCommonLogic.GetDoctorAllowPatient(dc.Id);
                                        resp.Project = dc.Project;
                                        AppDbCommonLogic.InsertUserLog(dc.UserId, "User Logged In", "Success");
                                    }
                                    else if (user.UserType == (int)UserType.Patient)
                                    {
                                        var pt = _db.Patient.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = pt.Id;
                                        resp.Name = pt.SecondName + " " + pt.FirstName;
                                        resp.ProfilePhoto = pt.ProfileUrl;
                                    }
                                    else if (user.UserType == (int)UserType.Admin)
                                    {
                                        var ad = _db.Administrator.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = ad.Id;
                                        resp.Name = ad.FirstName;
                                        //+ " " + ad.SecondName;
                                        //resp.ProfilePhoto = ad.p;
                                    }
                                    else if (user.UserType == (int)UserType.Nurse)
                                    {
                                        var nr = _db.Nurses.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = nr.Id;
                                        resp.Name = nr.SecondName + " " + nr.FirstName;
                                        resp.ProfilePhoto = nr.ProfileUrl;
                                    }
                                    else if (user.UserType == (int)UserType.Operator)
                                    {
                                        var ad = _db.Administrator.Where(a => a.UserId == user.Id).FirstOrDefault();
                                        resp.Id = ad.Id;
                                        resp.Name = ad.FirstName;
                                    }
                                    Random RN = new Random();
                                    int EmailSms = RN.Next(100000, 999999);
                                    resp.SMS = EmailSms;
                                    if (user.EmailAuth == true)
                                    {
                                        user.EmailOtp = EmailSms;
                                        AppCommonLogic.OtpEmail(user.Email, resp.Name, EmailSms);
                                        resp.AuthStatus = true;
                                    }
                                    else if (user.SMSAuth == true)
                                    {
                                        resp.AuthStatus = true;
                                    }
                                    else
                                    {
                                        resp.AuthStatus = false;
                                    }
                                    _db.SaveChanges();
                                }
                                else if (user.Status == (int)UserStatus.Waiting_for_Aproval)
                                {
                                    resp.msg = AppResource.user_Pending;
                                    resp.code = (int)ApiCustomResponseCode.Error;
                                    return Ok(resp);
                                }
                                else if (user.Status == (int)UserStatus.Blocked)
                                {
                                    resp.msg = AppResource.user_Blocked;
                                    resp.code = (int)ApiCustomResponseCode.Error;
                                    return Ok(resp);
                                }
                            }
                            else
                            {
                                resp.msg = AppResource.invalid_username;
                                resp.code = (int)ApiCustomResponseCode.Error;
                                return Ok(resp);
                            }
                        }
                    }
                    else
                    {
                        resp.msg = AppResource.invalid_username;
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                return Ok(resp);
            }
            return Ok(resp);
        }
        [HttpPost]
        public IActionResult GetForgetPaassword(ForgetPass obj)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.code = (int)ApiCustomResponseCode.BadRequest;
            string Name = "";
            try
            {
                if (!string.IsNullOrEmpty(obj.Email))
                {

                    Users user = _db.Users.Where(a => a.Email == obj.Email && a.Status == (int)UserStatus.Active).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Status == (int)UserStatus.Active)
                        {
                            //Random generator = new Random();
                            //string password = generator.Next(0, 999999).ToString("D6");
                            string UserIds = user.Id.ToString();
                            if (user.UserType == 2)
                            {
                                var doc = _db.Doctors.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = doc.SecondName + " " + doc.FirstName;
                            }
                            else if (user.UserType == 3)
                            {
                                var phc = _db.PharmaceuticalsCompany.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = phc.ComanyName;
                            }
                            else if (user.UserType == 4)
                            {
                                var pt = _db.Patient.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = pt.SecondName + " " + pt.FirstName;
                            }
                            else if (user.UserType == 5)
                            {
                                var ph = _db.PharmaRepresentative.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = ph.SecondName + " " + ph.FirstName;
                            }
                            else if (user.UserType == 6)
                            {
                                var nr = _db.Nurses.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = nr.SecondName + " " + nr.FirstName;
                            }
                            else if (user.UserType == 7)
                            {
                                var nr = _db.Administrator.Where(a => a.UserId == user.Id).FirstOrDefault();
                                Name = nr.SecondName + " " + nr.FirstName;
                            }
                            if (AppCommonLogic.SendForgetPassword(user.Email, Name, new AES_ALGORITHM().Encrypt(UserIds), user.UserType))
                            {
                                // user.UserPassword = new AES_ALGORITHM().Encrypt(password);
                                //_db.SaveChanges();
                                Resp.code = (int)ApiCustomResponseCode.Ok;
                                Resp.msg = AppResource.user_forget;
                                return Ok(Resp);
                            }
                            else
                            {
                                Resp.code = (int)ApiCustomResponseCode.Error;
                                Resp.msg = AppResource.user_Failed;
                                return Ok(Resp);
                            }
                        }
                        else if (user.Status == (int)UserStatus.Waiting_for_Aproval)
                        {
                            Resp.msg = AppResource.user_Pending;
                            Resp.code = (int)ApiCustomResponseCode.Error;
                        }
                        else if (user.Status == (int)UserStatus.Blocked)
                        {
                            Resp.msg = AppResource.user_Blocked;
                            Resp.code = (int)ApiCustomResponseCode.Error;
                        }
                    }
                    else
                    {
                        Resp.msg = AppResource.Mail_invalid;
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        return Ok(Resp);
                    }
                }
                else
                {
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                _log.LogInformation(ex.Message);
                string path = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                string Completefile = path + "/" + "J" + DateTime.UtcNow.Ticks + "" + "." + "json";
                System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(ex.Message));
                Resp.msg = ex.Message;
            }
            return Ok(Resp);
        }
        [HttpPost]
        public async Task<IActionResult> DoctorRegistration([FromBody] doctor data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int UserId;
            try
            {
                if (!UsersExists(data.Email))
                {
                    Users obj = new Users
                    {
                        Email = data.Email,
                        PhoneNo = data.PhoneNo,
                        //UserPassword = new AES_ALGORITHM().Encrypt(data.Password),
                        UserType = (int)UserType.Doctor,
                        Status = (int)UserStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString(),
                        BussinessStatus = 0,
                        LastMeeting = DateTime.UtcNow.AddMinutes(-3),
                        Lastlogin = DateTime.UtcNow.AddMinutes(-3),
                        EmailAuth = false,
                        SMSAuth = false
                    };
                    _db.Users.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        UserId = _db.Users.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        Address lat = AppCommonLogic.GetLatLong(data.City, data.Zipcode);
                        Doctors doc = new Doctors
                        {
                            UserId = UserId,
                            FirstName = data.FirstName,
                            SecondName = data.SecondName,
                            PhoneNo = data.PhoneNo,
                            YearsOfExperiecne = data.YearsOfExperiecne,
                            MedicalRegistrationNo = data.MedicalRegistrationNo,
                            City = data.City,
                            StreetNumber = data.StreetNumber,
                            Zipcode = data.Zipcode,
                            ShortIntroduction = data.ShortIntroduction,
                            ContractStartDate = DateTime.UtcNow,
                            Status = (int)UserStatus.Active,
                            Longitude = lat.Longitude,
                            Latitude = lat.Latitude,
                            Dob = DateTime.UtcNow,
                            Year = 0,
                            DoctorConsultation = true,
                            DoctorConslutationCount = 10000,
                            DoctorType = data.DoctorType,
                            Gander = data.Gander,
                            Country = data.Country,
                            CountryCode = data.CountryCode,
                            MSHCID = data.MSHCID,
                            DocCategory = data.DocCategory,
                            Agreement = data.Agreement,
                            IsValidated = false,
                            Project=data.Project
                        };
                        _db.Doctors.Add(doc);
                        int j = await _db.SaveChangesAsync();
                        int DocId = _db.Doctors.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        List<SpecilizationCategory> sc = new List<SpecilizationCategory>();
                        if (data.Expert != null)
                        {
                            for (int k = 0; k < data.Expert.Count(); k++)
                            {
                                Doctorspecilization ds = new Doctorspecilization()
                                {
                                    Name = data.Expert[k].Specialization,
                                    DocId = DocId,
                                    CatId = data.Expert[k].CatId,
                                    Status = 1
                                };
                                _db.Doctorspecilization.Add(ds);
                            }
                            await _db.SaveChangesAsync();
                        }
                        if (j > 0)
                        {
                            string UserIds = UserId.ToString();
                            AppCommonLogic.SendEmailToDoctor(data.Email, data.SecondName + " " + data.FirstName, new AES_ALGORITHM().Encrypt(UserIds), 2);
                            string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + new AES_ALGORITHM().Encrypt(UserIds) + "&Type=" + 2 + "&id=" + 1 + "";
                            resp.UserId = UserId.ToString();
                            resp.EncryptedId = new AES_ALGORITHM().Encrypt(UserIds);
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            resp.msg = AppResource.user_Success;
                            resp.path = Path;
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
                    resp.msg = AppResource.user_exist;
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                // resp.msg = ex.Message;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> PatientRegistration([FromBody] PatientReg data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int UserId;
            try
            {
                if (!UsersExists(data.Email))
                {
                    Users obj = new Users
                    {
                        Email = data.Email,
                        PhoneNo = data.PhoneNo,
                        // UserPassword = new AES_ALGORITHM().Encrypt(data.Password),
                        UserType = (int)UserType.Patient,
                        Status = (int)UserStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString(),
                        BussinessStatus = 0,
                        LastMeeting = DateTime.UtcNow.AddMinutes(-3),
                        Lastlogin = DateTime.UtcNow.AddMinutes(-3),
                        EmailAuth = false,
                        SMSAuth = false
                    };
                    _db.Users.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        UserId = _db.Users.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        Patient doc = new Patient
                        {
                            UserId = UserId,
                            FirstName = data.FirstName,
                            SecondName = data.SecondName,
                            ShortIntroduction = data.ShortIntroduction,
                            Dob = data.Dob,
                            Zipcode = data.Zipcode,
                            City = data.City,
                            StreetNumber = data.StreetNumber,
                            AddressLine = data.AddressLine,
                            Gender = data.Gender,
                            SocialSecurityNumber = data.SocialSecurityNumber,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow.AddDays(-1),
                            DeletedAt = DateTime.UtcNow.AddDays(-1),
                            Country = data.Country,
                            State = data.State,
                            Status = (int)UserStatus.Active,
                            WizardStatus = 0,
                            GDPR = data.GDPR,
                            CountryCode = data.CountryCode,
                            InvoiceName = data.InvoiceName,
                            InvoiceAddress = data.InvoiceAddress,
                            TaxNumber = data.TaxNumber
                        };
                        _db.Patient.Add(doc);
                        int j = await _db.SaveChangesAsync();
                        if (j > 0)
                        {
                            string UserIds = UserId.ToString();
                            AppCommonLogic.SendEmail(data.Email, data.SecondName + " " + data.FirstName, new AES_ALGORITHM().Encrypt(UserIds), 4);
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            resp.msg = AppResource.PatientSuccess;
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
                    resp.msg = AppResource.user_exist;
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
                //resp.msg = ex.Message;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> PharmaRegistration([FromBody] pharma data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int UserId;
            try
            {
                if (!UsersExists(data.Email))
                {
                    Users obj = new Users
                    {
                        Email = data.Email,
                        //PhoneNo = data.PhoneNo,
                        //UserPassword = "",
                        UserType = (int)UserType.Pharma,
                        Status = (int)UserStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString(),
                        BussinessStatus = 0,
                        LastMeeting = DateTime.UtcNow.AddMinutes(-3),
                        Lastlogin = DateTime.UtcNow.AddMinutes(-3),
                        EmailAuth = false,
                        SMSAuth = false
                    };
                    _db.Users.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        UserId = _db.Users.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        PharmaceuticalsCompany doc = new PharmaceuticalsCompany
                        {
                            UserId = UserId,
                            ComanyName = data.ComanyName,
                            Address = data.Address,
                            ContactPerson = data.ContactPerson,
                            Email = data.Email,
                            Status = (int)UserStatus.Active
                        };
                        _db.PharmaceuticalsCompany.Add(doc);
                        int j = await _db.SaveChangesAsync();
                        if (j > 0)
                        {
                            int Id = _db.PharmaceuticalsCompany.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                            if (data.GetAssignPharma.Count > 0)
                            {
                                List<AssignPharma> gtawart = new List<AssignPharma>();
                                for (int n = 0; n < data.GetAssignPharma.Count(); n++)
                                {
                                    PharmaCompanyRepMap Assph = new PharmaCompanyRepMap()
                                    {
                                        PharmaRepId = data.GetAssignPharma[n].RepId,
                                        PharmaComId = Id,
                                        Status = 0,
                                        Created = DateTime.UtcNow
                                    };
                                    _db.PharmaCompanyRepMap.Add(Assph);
                                    int p = await _db.SaveChangesAsync();
                                    if (p > 0)
                                    {
                                        var phrep = _db.PharmaRepresentative.Where(a => a.Id == data.GetAssignPharma[n].RepId).FirstOrDefault();
                                        if (phrep != null)
                                        {
                                            phrep.AssinStatus = 1;
                                            await _db.SaveChangesAsync();
                                        }
                                    }
                                }
                                string UserIds = UserId.ToString();
                                AppCommonLogic.SendEmail(data.Email, data.FirstName, new AES_ALGORITHM().Encrypt(UserIds), 3);
                            }
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            resp.msg = AppResource.user_Success;
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
                    resp.msg = AppResource.user_exist;
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
        public async Task<IActionResult> ChangePassword([FromBody] ChangePass data)
        {
            ApiResponse Resp = new ApiResponse();
            Resp.code = (int)ApiCustomResponseCode.BadRequest;
            Resp.msg = AppResource.invalid_model;
            string Name = "";
            string pass = new AES_ALGORITHM().Encrypt(data.OldPassword);
            try
            {
                if (ModelState.IsValid)
                {
                    var gtpass = _db.Users.Where(a => a.Id == data.UserId && a.UserPassword == pass).FirstOrDefault();
                    if (gtpass != null)
                    {
                        gtpass.UserPassword = new AES_ALGORITHM().Encrypt(data.NewPassword);
                        int i = await _db.SaveChangesAsync();
                        if (gtpass.UserType == 2)
                        {
                            var doc = _db.Doctors.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = doc.SecondName + " " + doc.FirstName;
                        }
                        else if (gtpass.UserType == 3)
                        {
                            var phc = _db.PharmaceuticalsCompany.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = phc.ComanyName;
                        }
                        else if (gtpass.UserType == 4)
                        {
                            var pt = _db.Patient.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = pt.SecondName + " " + pt.FirstName;
                        }
                        else if (gtpass.UserType == 5)
                        {
                            var ph = _db.PharmaRepresentative.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = ph.SecondName + " " + ph.FirstName;
                        }
                        else if (gtpass.UserType == 6)
                        {
                            var nr = _db.Nurses.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = nr.SecondName + " " + nr.FirstName;
                        }
                        else if (gtpass.UserType == 7)
                        {
                            var nr = _db.Administrator.Where(a => a.UserId == gtpass.Id).FirstOrDefault();
                            Name = nr.SecondName + " " + nr.FirstName;
                        }
                        if (i > 0)
                        {
                            AppCommonLogic.SendEmailChangePassword(gtpass.Email, data.NewPassword, Name);
                            Resp.code = (int)ApiCustomResponseCode.Ok;
                            Resp.msg = AppResource.user_password;
                            return Ok(Resp);
                        }
                        else
                        {
                            Resp.code = (int)ApiCustomResponseCode.Ok;
                            Resp.msg = AppResource.user_password;
                            return Ok(Resp);
                        }
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        Resp.msg = AppResource.Old_Password;
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
        public async Task<IActionResult> AddAdmin([FromBody] Admin data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int UserId;
            try
            {
                if (!UsersExists(data.Email))
                {
                    Users obj = new Users
                    {
                        Email = data.Email,
                        PhoneNo = data.PhoneNo,
                        UserPassword = new AES_ALGORITHM().Encrypt(data.Password),
                        UserType = (int)UserType.Admin,
                        Status = (int)UserStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString(),
                        BussinessStatus = 0,
                        LastMeeting = DateTime.UtcNow.AddMinutes(-3),
                        Lastlogin = DateTime.UtcNow.AddMinutes(-3),
                        EmailAuth = false,
                        SMSAuth = false
                    };
                    _db.Users.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        UserId = _db.Users.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        Administrator admn = new Administrator
                        {
                            UserId = UserId,
                            FirstName = data.FirstName,
                            SecondName = data.SecondName,
                            Mshcid = data.MSHCID,
                            ContractStartDate = data.ContractStartDate,
                            ContractEndDate = data.ContarctEndDate,
                            Role = 1,
                            IsActive = true
                        };
                        _db.Administrator.Add(admn);
                        int j = await _db.SaveChangesAsync();
                        if (j > 0)
                        {
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            resp.msg = AppResource.user_Success;
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
                    resp.msg = AppResource.user_exist;
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetSpecializationCategory()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            GetSpeCat data = new GetSpeCat();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                if (Lang == "en")
                {
                    data.SpecializationCategory = (from r in _db.SpecilizationCategory
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       FirstName = r.Name,
                                                       Name = r.Name,
                                                       HungName = r.HunName,
                                                       AllowPatient=r.AllowPatient
                                                   }).ToList();
                }
                else
                {
                    data.SpecializationCategory = (from r in _db.SpecilizationCategory.OrderBy(a => a.HunName)
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       FirstName = r.HunName,
                                                       Name = r.Name,
                                                       HungName = r.HunName,
                                                       AllowPatient = r.AllowPatient
                                                   }).ToList();
                }
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBussinessStatus(UpdateStatus data)
        {
            UpdateStatus Resp = new UpdateStatus();
            try
            {
                var getpro = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (getpro != null)
                {
                    getpro.BussinessStatus = data.BussinessStatus;
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
        public IActionResult RequestForProfileChange(ProfileRequest data)
        {
            ApiResponse Resp = new ApiResponse();
            string Name = "";
            try
            {
                if (data.Type == (int)UserType.Doctor)
                {
                    Name = _db.Doctors.Where(a => a.UserId == data.UserId).FirstOrDefault().FirstName;
                }
                else if (data.Type == (int)UserType.Pharma)
                {
                    Name = _db.PharmaceuticalsCompany.Where(a => a.UserId == data.UserId).FirstOrDefault().ComanyName;
                }
                else if (data.Type == (int)UserType.PharmaRep)
                {
                    Name = _db.PharmaRepresentative.Where(a => a.UserId == data.UserId).FirstOrDefault().Name;
                }
                AppCommonLogic.SendEmailProfileRequest(Name, data.Message);
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = AppResource.user_request;
                return Ok(Resp);
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.BadRequest;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(UpdatePass data)
        {
            ApiResponse Resp = new ApiResponse();
            string UserId = new AES_ALGORITHM().Decrypt(data.UserId);
            int Id = Convert.ToInt32(UserId);
            try
            {
                var gtpass = _db.Users.Where(a => a.Id == Id).FirstOrDefault();
                if (gtpass != null)
                {
                    gtpass.UserPassword = new AES_ALGORITHM().Encrypt(data.Password);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        //AppCommonLogic.SendEmailChangePassword(gtpass.Email, data.NewPassword);
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_password;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_password;
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
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public ActionResult ChackPassword(chckusr data)
        {
            ApiResponse Resp = new ApiResponse();
            string UserIds = new AES_ALGORITHM().Decrypt(data.UserId);
            int Id = Convert.ToInt32(UserIds);
            try
            {
                var gtpass = _db.Users.Where(a => a.Id == Id).FirstOrDefault();
                if (gtpass != null)
                {
                    //if (gtpass.UserPassword != null)
                    //{
                    //    Resp.code = (int)ApiCustomResponseCode.Ok;
                    //    Resp.msg = AppResource.user_password;
                    //    return Ok(Resp);
                    //}
                    //else
                    //{
                    //if (DateTime.UtcNow > gtpass.CreatedAt.AddHours(168))
                    //{
                    //    Resp.code = (int)ApiCustomResponseCode.Block;
                    //    Resp.msg = AppResource.invalid_model;
                    //    AppDbCommonLogic.DeleteUserProfile(gtpass.Id, gtpass.UserType);
                    //    return Ok(Resp);
                    //}
                    //else
                    if (gtpass.UserPassword != null)
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_password;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        Resp.msg = AppResource.invalid_model;
                        return Ok(Resp);
                    }
                    //  }
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Block;
                    Resp.msg = AppResource.invalid_model;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.Error;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public ActionResult UpdateUserTiming(Updatetime data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                var user = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (user != null)
                {
                    if (data.Type == 1)
                    {
                        user.Lastlogin = DateTime.UtcNow;

                    }
                    else if (data.Type == 2)
                    {
                        user.LastMeeting = DateTime.UtcNow;
                    }
                    else if (data.Type == 3)
                    {
                        AppDbCommonLogic.InsertUserLog(data.UserId, "User Logout", "Success");
                        user.Lastlogin = DateTime.UtcNow.AddMinutes(-3);
                    }
                }
                _db.SaveChanges();
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = AppResource.user_update;

                return Ok(Resp);
            }
            catch (Exception ex)
            {
                AppDbCommonLogic.InsertUserLog(data.UserId, "", "Failed", ex.Message + " " + ex.StackTrace);
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOS([FromBody] Admin data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            int UserId;
            try
            {
                if (!UsersExists(data.Email))
                {
                    Users obj = new Users
                    {
                        Email = data.Email,
                        PhoneNo = data.PhoneNo,
                        UserPassword = new AES_ALGORITHM().Encrypt(data.Password),
                        UserType = (int)UserType.Operator,
                        Status = (int)UserStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString(),
                        BussinessStatus = 0,
                        LastMeeting = DateTime.UtcNow.AddMinutes(-3),
                        Lastlogin = DateTime.UtcNow.AddMinutes(-3)
                    };
                    _db.Users.Add(obj);
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        UserId = _db.Users.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                        Administrator admn = new Administrator
                        {
                            UserId = UserId,
                            FirstName = data.FirstName,
                            SecondName = data.SecondName,
                            Mshcid = data.MSHCID,
                            ContractStartDate = data.ContractStartDate,
                            ContractEndDate = data.ContarctEndDate,
                            Role = (int)UserType.Operator,
                            IsActive = true
                        };
                        _db.Administrator.Add(admn);
                        int j = await _db.SaveChangesAsync();
                        if (j > 0)
                        {
                            resp.code = (int)ApiCustomResponseCode.Ok;
                            resp.msg = AppResource.user_Success;
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
                    resp.msg = AppResource.user_exist;
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetDoctorCategoryType()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            GetSpeCat data = new GetSpeCat();
            string Lang = "";
            Lang = Request.Headers["Lang"];
            try
            {
                if (Lang == "en")
                {
                    data.SpecializationCategory = (from r in _db.DoctorCategoryType
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       FirstName = r.Name
                                                   }).ToList();
                }
                else
                {
                    data.SpecializationCategory = (from r in _db.DoctorCategoryType
                                                   select new doctor
                                                   {
                                                       Id = r.Id,
                                                       FirstName = r.HunName
                                                   }).ToList();
                }
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
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpGet]
        public ActionResult<IEnumerable<doctor>> GetCountry()
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.Error;
            GetSpeCat data = new GetSpeCat();
            try
            {

                data.SpecializationCategory = (from r in _db.Country
                                               select new doctor
                                               {
                                                   Id = r.Id,
                                                   FirstName = r.Name
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
            catch (Exception ex)
            {
                resp.msg = ex.Message;
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public ActionResult UpdateNurseSuggetion(nurseSuggetion data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                var app = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).FirstOrDefault();
                if (app != null)
                {
                    app.NurseSuggestionStatus = data.Status;
                }
                _db.SaveChanges();
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = AppResource.user_update;
                return Ok(Resp);
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public ActionResult EnableDisableAuthentication(Authentication data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                var user = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (user != null)
                {
                    if (data.AuthType == "SMS")
                    {
                        user.SMSAuth = data.Status;
                    }
                    else if (data.AuthType == "Email")
                    {
                        user.EmailAuth = data.Status;
                    }
                }
                _db.SaveChanges();
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = AppResource.user_update;
                return Ok(Resp);
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResendOTP(rESEND data)
        {
            ApiResponse Resp = new ApiResponse();
            try
            {
                var gtpass = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (gtpass != null)
                {
                    Random RN = new Random();
                    int EmailSms = RN.Next(100000, 999999);
                    if (gtpass.EmailAuth == true)
                    {
                        gtpass.EmailOtp = EmailSms;
                    }
                    else if (gtpass.SMSAuth == true)
                    {
                        gtpass.MobileOtp = EmailSms;
                    }
                    int i = await _db.SaveChangesAsync();
                    if (i > 0)
                    {
                        AppCommonLogic.OtpEmail(gtpass.Email, data.Name, EmailSms);
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_password;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.user_password;
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
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public IActionResult CheckOTP(chkotp data)
        {
            Otp Resp = new Otp();
            try
            {
                var user = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (user != null)
                {
                    if (user.EmailOtp == data.OTP)
                    {
                        //  Resp.AuthStatus=gtpass.EmailAuth
                        Resp.BussinessStatus = 1;
                        Resp.Email = user.Email;
                        Resp.Phone = user.PhoneNo;
                        if (user.UserType == (int)UserType.PharmaRep)
                        {
                            var ph = _db.PharmaRepresentative.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = ph.Id;
                            Resp.Name = ph.SecondName + " " + ph.FirstName;
                            Resp.ProfilePhoto = ph.ProfileUrl;
                            var Comp = _db.PharmaCompanyRepMap.Where(a => a.PharmaRepId == Resp.Id && a.Status == 0).FirstOrDefault();
                            if (Comp != null)
                            {
                                Resp.PharmaCompId = Comp.PharmaComId;
                            }
                            else
                            {
                                Resp.PharmaCompId = 0;
                            }
                        }
                        else if (user.UserType == (int)UserType.Pharma)
                        {
                            var ph = _db.PharmaceuticalsCompany.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = ph.Id;
                            Resp.Name = ph.ComanyName;
                            Resp.ProfilePhoto = ph.ProfileUrl;
                        }
                        else if (user.UserType == (int)UserType.Doctor)
                        {
                            var dc = _db.Doctors.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = dc.Id;
                            Resp.Name = dc.SecondName + " " + dc.FirstName;
                            Resp.ProfilePhoto = dc.PhotoUrl;
                        }
                        else if (user.UserType == (int)UserType.Patient)
                        {
                            var pt = _db.Patient.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = pt.Id;
                            Resp.Name = pt.SecondName + " " + pt.FirstName;
                            Resp.ProfilePhoto = pt.ProfileUrl;
                            Resp.WizardStatus = AppDbCommonLogic.GetwizardStatusforpt(pt.Id);
                        }
                        else if (user.UserType == (int)UserType.Admin)
                        {
                            var ad = _db.Administrator.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = ad.Id;
                            Resp.Name = ad.FirstName;
                        }
                        else if (user.UserType == (int)UserType.Nurse)
                        {
                            var nr = _db.Nurses.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = nr.Id;
                            Resp.Name = nr.SecondName + " " + nr.FirstName;
                            Resp.ProfilePhoto = nr.ProfileUrl;
                        }
                        else if (user.UserType == (int)UserType.Operator)
                        {
                            var ad = _db.Administrator.Where(a => a.UserId == user.Id).FirstOrDefault();
                            Resp.Id = ad.Id;
                            Resp.Name = ad.FirstName;
                        }
                        Resp.Token = user.Token;
                        Resp.BussinessStatus = 1;
                        //  Resp.Lastlogin = DateTime.UtcNow;
                        Resp.UserId = user.Id;
                        Resp.Usertype = user.UserType;
                        Resp.UserIds = AppCommonLogic.ToHexString(user.Id.ToString());
                        Resp.code = (int)ApiCustomResponseCode.Ok;
                        Resp.msg = AppResource.login_success;
                        return Ok(Resp);
                    }
                    else
                    {
                        Resp.code = (int)ApiCustomResponseCode.Error;
                        Resp.msg = AppResource.invalid_model;
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
                Resp.code = (int)ApiCustomResponseCode.Ok;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpPost]
        public ActionResult SendEmailToDoctor(send data)
        {
            ApiResponse resp = new ApiResponse();
            resp.msg = AppResource.invalid_model;
            resp.code = (int)ApiCustomResponseCode.BadRequest;
            try
            {
                string UserIds = data.UserId.ToString();
                var user = _db.Users.Where(a => a.Id == data.UserId).FirstOrDefault();
                if (user != null)
                {
                    user.CreatedAt = DateTime.UtcNow;
                    _db.SaveChanges();
                    if (AppCommonLogic.SendEmailToDoctor(user.Email, data.FirstName, new AES_ALGORITHM().Encrypt(UserIds), 2))
                    {
                        resp.code = (int)ApiCustomResponseCode.Ok;
                        resp.msg = AppResource.user_Success;
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
                    resp.msg = AppResource.NotRefund;
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.BadRequest;
            }
            return Ok(resp);
        }
        [HttpPost]
        public ActionResult UnSubscribe(chckusr data)
        {
            ApiResponse Resp = new ApiResponse();
            string UserIds = new AES_ALGORITHM().Decrypt(data.UserId);
            int Id = Convert.ToInt32(UserIds);
            try
            {
                var gtpass = _db.Users.Where(a => a.Id == Id).FirstOrDefault();
                if (gtpass != null)
                {
                    gtpass.Unscubscribe = 1;
                    _db.SaveChanges();
                    Resp.code = (int)ApiCustomResponseCode.Ok;
                    Resp.msg = AppResource.user_password;
                    return Ok(Resp);
                }
                else
                {
                    Resp.code = (int)ApiCustomResponseCode.Block;
                    Resp.msg = AppResource.invalid_model;
                    return Ok(Resp);
                }
            }
            catch (Exception ex)
            {
                Resp.code = (int)ApiCustomResponseCode.Error;
                Resp.msg = ex.Message;
                return Ok(Resp);
            }
        }
        [HttpGet]
        public IActionResult CheckDoctor(string Email)
        {
            Login resp = new Login();
            try
            {
                var user = _db.Users.Where(a => a.Email == Email).FirstOrDefault();
                if (user != null)
                {
                    if (user.UserPassword != null)
                    {
                        var dc = _db.Doctors.Where(a => a.UserId == user.Id).FirstOrDefault();
                        resp.Id = dc.Id;
                        resp.Name = dc.SecondName + " " + dc.FirstName;
                        resp.ProfilePhoto = dc.PhotoUrl;
                        resp.Doctortype = dc.DocCategory;
                        resp.IsValidated = (bool)dc.IsValidated;
                        resp.msg = AppResource.login_success;
                        resp.code = (int)ApiCustomResponseCode.Ok;
                        resp.Token = Guid.NewGuid().ToString();
                        user.Token = resp.Token;
                        user.BussinessStatus = 1;
                        user.Lastlogin = DateTime.UtcNow;
                        resp.UserId = user.Id;
                        resp.Usertype = user.UserType;
                        resp.Phone = user.PhoneNo;
                        resp.Email = user.Email;
                        resp.UserIds = AppCommonLogic.ToHexString(user.Id.ToString());
                        resp.BussinessStatus = (int)user.BussinessStatus;
                        resp.DoctorUserId = new AES_ALGORITHM().Encrypt(user.Id.ToString());
                        return Ok(resp);
                    }
                    else
                    {
                        resp.code = 201;
                        resp.DoctorUserId = new AES_ALGORITHM().Encrypt(user.Id.ToString());
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
            catch (Exception ex)
            {
                resp.code = (int)ApiCustomResponseCode.Ok;
                resp.msg = ex.Message;
                return Ok(resp);
            }
        }
    }
}