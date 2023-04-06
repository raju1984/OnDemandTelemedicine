using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyClickDoctorBE.DatabaseEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyClickDoctorBE.Models.AppCommonLogic;

namespace MyClickDoctorBE.Models
{
    public class AppDbCommonLogic
    {
        public bool Isvalid(string token)
        {
            try
            {
                using (MyClickDoctorV4 _db = new MyClickDoctorV4())
                {
                    return _db.Users.Any(e => e.Token == token.Trim());
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string GetUserName(int UserId, int UserType)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (UserType == 5)
            {
                Name = _db.PharmaRepresentative.Where(a => a.UserId == UserId).FirstOrDefault().Name;
            }
            else if (UserType == 3)
            {
                Name = _db.PharmaceuticalsCompany.Where(a => a.UserId == UserId).FirstOrDefault().ComanyName;
            }
            else if (UserType == 2)
            {
                var docnam = _db.Doctors.Where(a => a.UserId == UserId).FirstOrDefault();
                Name = docnam.SecondName + " " + docnam.FirstName;
            }
            else if (UserType == 4)
            {
                var docnam = _db.Patient.Where(a => a.UserId == UserId).FirstOrDefault();
                Name = docnam.SecondName + " " + docnam.FirstName;
            }
            else if (UserType == 6)
            {
                var docnam = _db.Nurses.Where(a => a.UserId == UserId).FirstOrDefault();
                Name = docnam.SecondName + " " + docnam.FirstName;
            }
            return Name;
        }
        public static string SendCancelMeetingEmailToUser(int AppointmentId, int Type)
        {
            string CancelledBy = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            var doctr = _db.Doctors.Where(a => a.Id == meetdata.DoctorId).FirstOrDefault();
            var pharm = _db.PharmaRepresentative.Where(a => a.Id == meetdata.PharmaRepId).FirstOrDefault();
            string Email = _db.Users.Where(a => a.Id == doctr.UserId).FirstOrDefault().Email;
            if (Type == 1)
            {
                CancelledBy = "Administrator";
            }
            else if (Type == 2)
            {
                CancelledBy = doctr.SecondName + " " + doctr.FirstName;
            }
            else if (Type == 5)
            {
                CancelledBy = pharm.Name;
            }
            AppCommonLogic.SendEmailCancelMeeting(Email, doctr.SecondName + " " + doctr.FirstName, pharm.Name, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            var doctr1 = _db.Doctors.Where(a => a.Id == meetdata.DoctorId).FirstOrDefault();
            var pharm1 = _db.PharmaRepresentative.Where(a => a.Id == meetdata.PharmaRepId).FirstOrDefault();
            string Email1 = _db.Users.Where(a => a.Id == pharm1.UserId).FirstOrDefault().Email;
            AppCommonLogic.SendEmailCancelMeeting(Email1, pharm1.Name, doctr1.SecondName + " " + doctr1.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            return CancelledBy;
        }
        public static string SendCancelPatientMeetingEmailToUser(int AppointmentId, int Type)
        {
            string CancelledBy = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            var pat = _db.Patient.Where(a => a.Id == meetdata.PatientId).FirstOrDefault();
            string patEmail = _db.Users.Where(a => a.Id == pat.UserId).FirstOrDefault().Email;
            if (Type == 4)
            {
                if (meetdata.UserRole == 2)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 4);
                    int DoctorId = GetAppintmentUserId(meetdata.Id, 2);
                    var DocUserId = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == DocUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, DocUserId.SecondName + " " + DocUserId.FirstName, pat.SecondName + " " + pat.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(patEmail, pat.SecondName + " " + pat.FirstName, DocUserId.SecondName + " " + DocUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
                else if (meetdata.UserRole == 6)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 4);
                    int NurseId = GetAppintmentUserId(meetdata.Id, 6);
                    var NurseUserId = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == NurseUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, NurseUserId.SecondName + " " + NurseUserId.FirstName, pat.SecondName + " " + pat.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(patEmail, pat.SecondName + " " + pat.FirstName, NurseUserId.SecondName + " " + NurseUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
            }
            return CancelledBy;
        }
        public static string SendCancelNurseMeetingEmailToUser(int AppointmentId, int Type)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            var nur = _db.Nurses.Where(a => a.Id == meetdata.NurseId).FirstOrDefault();
            string NurseEmail = _db.Users.Where(a => a.Id == nur.UserId).FirstOrDefault().Email;
            if (Type == 6)
            {
                string CancelledBy = nur.SecondName + " " + nur.FirstName;
                if (meetdata.UserRole == 2 && meetdata.MyRole == 6)
                {
                    int DoctorId = GetAppintmentUserId(meetdata.Id, 2);
                    var DocConst = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                    string DocConsEmail = _db.Users.Where(a => a.Id == DocConst.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(DocConsEmail, DocConst.SecondName + " " + DocConst.FirstName, nur.SecondName + " " + nur.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(NurseEmail, CancelledBy, DocConst.SecondName + " " + DocConst.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
                else if (meetdata.UserRole == 6 && meetdata.MyRole == 4)
                {
                    int PatientId = GetAppintmentUserId(meetdata.Id, 4);
                    var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                    string patEmail = _db.Users.Where(a => a.Id == pat.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(patEmail, pat.SecondName + " " + pat.FirstName, CancelledBy, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(NurseEmail, CancelledBy, pat.SecondName + " " + pat.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
            }
            return nur.FirstName;
        }
        public static string SendCancelDoctorMeetingEmailToUser(int AppointmentId, int Type)
        {
            string CancelledBy = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            var Doct = _db.Doctors.Where(a => a.Id == meetdata.DoctorId).FirstOrDefault();
            string DoctEmail = _db.Users.Where(a => a.Id == Doct.UserId).FirstOrDefault().Email;
            if (Type == 2)
            {
                CancelledBy = GetAppintmentUserName(meetdata.Id, Type);
                if (meetdata.MyRole == 8)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 2);
                    var DocUserId = _db.Doctors.Where(a => a.Id == meetdata.DoctorConsultationId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == DocUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, DocUserId.SecondName + " " + DocUserId.FirstName, Doct.SecondName + " " + Doct.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(DoctEmail, Doct.SecondName + " " + Doct.FirstName, DocUserId.SecondName + " " + DocUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    InsertUserLog(Doct.UserId, "Cancel Appointment", "Success");
                }
                else if (meetdata.MyRole == 4)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 2);
                    int PatientId = GetAppintmentUserId(meetdata.Id, 4);
                    var PatUserId = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == PatUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, PatUserId.SecondName + " " + PatUserId.FirstName, Doct.SecondName + " " + Doct.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(DoctEmail, Doct.SecondName + " " + Doct.FirstName, PatUserId.SecondName + " " + PatUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
                else if (meetdata.MyRole == 6)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 2);
                    int NurseId = GetAppintmentUserId(meetdata.Id, 6);
                    var NurseUserId = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == NurseUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, NurseUserId.SecondName + " " + NurseUserId.FirstName, Doct.SecondName + " " + Doct.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(DoctEmail, Doct.SecondName + " " + Doct.FirstName, NurseUserId.SecondName + " " + NurseUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
                else if (meetdata.MyRole == 5)
                {
                    CancelledBy = GetAppintmentUserName(meetdata.Id, 2);
                    int PharmaId = GetAppintmentUserId(meetdata.Id, 5);
                    var PharmaUserId = _db.PharmaRepresentative.Where(a => a.Id == PharmaId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == PharmaUserId.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendEmailCancelMeeting(Email, PharmaUserId.SecondName + " " + PharmaUserId.FirstName, Doct.SecondName + " " + Doct.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                    AppCommonLogic.SendEmailCancelMeeting(DoctEmail, Doct.SecondName + " " + Doct.FirstName, PharmaUserId.SecondName + " " + PharmaUserId.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                }
            }
            return CancelledBy;
        }
        public static string SendCancelAdminMeetingEmailToUser(int AppointmentId, int Type)
        {
            string CancelledBy = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            CancelledBy = "Administrator";
            if (meetdata.UserRole == 2 && meetdata.MyRole == 8)
            {
                int DoctorId = GetAppintmentUserId(meetdata.Id, 8);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int DoctorConsId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var DocConst = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string DocConsEmail = _db.Users.Where(a => a.Id == DocConst.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendEmailCancelMeeting(DocConsEmail, DocConst.SecondName + " " + DocConst.FirstName, Doctor.SecondName + " " + Doctor.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                AppCommonLogic.SendEmailCancelMeeting(Email, Doctor.SecondName + " " + Doctor.FirstName, DocConst.SecondName + " " + DocConst.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 5)
            {
                int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int Pharmaid = GetAppintmentUserId(meetdata.Id, 5);
                var phrma = _db.PharmaRepresentative.Where(a => a.Id == Pharmaid).FirstOrDefault();
                string PhrmaEmail = _db.Users.Where(a => a.Id == phrma.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendEmailCancelMeeting(Email, Doctor.SecondName + " " + Doctor.FirstName, phrma.SecondName + " " + phrma.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                AppCommonLogic.SendEmailCancelMeeting(PhrmaEmail, phrma.SecondName + " " + phrma.FirstName, Doctor.SecondName + " " + Doctor.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 4)
            {
                int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int Patid = GetAppintmentUserId(meetdata.Id, 4);
                var pt = _db.Patient.Where(a => a.Id == Patid).FirstOrDefault();
                string ptEmail = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendEmailCancelMeeting(Email, Doctor.SecondName + " " + Doctor.FirstName, pt.SecondName + " " + pt.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                AppCommonLogic.SendEmailCancelMeeting(ptEmail, pt.SecondName + " " + pt.FirstName, Doctor.SecondName + " " + Doctor.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 6)
            {
                int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int nurid = GetAppintmentUserId(meetdata.Id, 6);
                var nr = _db.Nurses.Where(a => a.Id == nurid).FirstOrDefault();
                string nrEmail = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendEmailCancelMeeting(Email, Doctor.SecondName + " " + Doctor.FirstName, nr.SecondName + " " + nr.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                AppCommonLogic.SendEmailCancelMeeting(nrEmail, nr.SecondName + " " + nr.FirstName, Doctor.SecondName + " " + Doctor.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            }
            else if (meetdata.UserRole == 6 && meetdata.MyRole == 4)
            {
                int ptId = GetAppintmentUserId(meetdata.Id, 4);
                var pt = _db.Patient.Where(a => a.Id == ptId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault().Email;

                int nurid = GetAppintmentUserId(meetdata.Id, 6);
                var nr = _db.Nurses.Where(a => a.Id == nurid).FirstOrDefault();
                string nrEmail = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendEmailCancelMeeting(Email, pt.SecondName + " " + pt.FirstName, nr.SecondName + " " + nr.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
                AppCommonLogic.SendEmailCancelMeeting(nrEmail, nr.SecondName + " " + nr.FirstName, pt.SecondName + " " + pt.FirstName, CancelledBy, meetdata.BookingDate, meetdata.Duration, meetdata.AppointmentTitle);
            }
            return CancelledBy;
        }
        public static int GetMeetingStatus(DateTime start, DateTime end, int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            // DateTime EndDate = end.AddMinutes(10);
            DateTime crnttime = DateTime.UtcNow;
            int status = 0;
            try
            {
                var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId && (crnttime >= a.StartDate.AddMinutes(-3) && a.EndDate >= crnttime)).FirstOrDefault();
                if (appdetail != null)
                {
                    status = 0;
                }
                else
                {
                    status = 1;
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }
        public static int GetBussinessStatus(int UserId)
        {
            int BussinessStatus = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var userdata = _db.Users.Where(a => a.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    TimeSpan Total = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                    int Time = Math.Abs((int)Total.TotalMinutes);
                    if (Time <= 2)
                    {
                        BussinessStatus = 2;
                        userdata.BussinessStatus = 2;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.Lastlogin.TimeOfDay;
                    int mnts = Math.Abs((int)mint.TotalMinutes);
                    if (mnts <= 2)
                    {
                        BussinessStatus = 1;
                        userdata.BussinessStatus = 1;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    else
                    {
                        BussinessStatus = 0;
                        userdata.BussinessStatus = 0;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                }
                else
                {
                    BussinessStatus = 0;
                    return BussinessStatus = 0;
                }

            }
            catch (Exception ex)
            {
                BussinessStatus = 0;
                return BussinessStatus = 0;
            }
        }
        public static int GetMyTarityDoctor(int RepId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int DoctorCount = 0;
            List<doctor> list = new List<doctor>();
            GetSpeCat data = new GetSpeCat();
            List<doctor> getlist = new List<doctor>();
            try
            {
                var cat = _db.DoctorPhramaRepMap.Where(a => a.Status == 1 && a.RepId == RepId).ToList();
                foreach (var item in cat)
                {
                    var getDoc = _db.Doctorspecilization.Include(y => y.Doc).Where(a => a.CatId == item.CatId && a.Doc.Status == 1).ToList();
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
                                                       }).ToList();

                    }
                    getlist.AddRange(data.SpecializationCategory);
                }
                DataResponse dataResponse = new DataResponse();
                dataResponse.getList = getlist.GroupBy(x => x.UserId).Select(x => x.First()).ToList();
                DoctorCount = dataResponse.getList.Count();
            }
            catch (Exception ex)
            {
                DoctorCount = 0;
            }
            return DoctorCount;
        }
        public static int GetCancelStatus(int AppointmentId, DateTime end)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime crnttime = DateTime.UtcNow;
            int status = 0;
            try
            {
                //var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId && crnttime >= a.EndDate).FirstOrDefault();
                if (crnttime >= end)
                {
                    status = 1;
                }
                else
                {
                    status = 0;
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }
        public static int DeletePharmaRep(int CompanyId)
        {
            int status = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var getpharma = _db.PharmaCompanyRepMap.Where(a => a.PharmaComId == CompanyId).ToList();
            if (getpharma.Count > 0)
            {
                foreach (var item in getpharma)
                {
                    _db.PharmaCompanyRepMap.Remove(item);
                    var phrep = _db.PharmaRepresentative.Where(a => a.Id == item.PharmaRepId).FirstOrDefault();
                    if (phrep != null)
                    {
                        phrep.AssinStatus = 0;
                    }
                }
                _db.SaveChangesAsync();
            }
            return status;
        }
        public static int InsertPaymentWithPendingStatus(decimal Amount, int Currency, int AppointmentId, int UserId, string PaymentId, string Remark, string TransactionId, string POSTransactionId)
        {
            int row = 0;
            string modifiedString = PaymentId.Replace("@", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace("'", "").Replace("-", "");
            string modifiedString1 = TransactionId.Replace("@", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace("'", "").Replace("-", "");
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var paymnt = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (paymnt != null)
                {
                    paymnt.PaymentId = modifiedString;
                    paymnt.TransactionId = modifiedString1;
                    paymnt.POSTransactionId = POSTransactionId;
                    paymnt.Status = 2;
                    _db.SaveChanges();
                }
                else
                {
                    Payments pay = new Payments()
                    {
                        Amount = Amount,
                        Currency = Currency,
                        AppoinmentId = AppointmentId,
                        UserId = UserId,
                        TransactionId = modifiedString1,
                        Remark = Remark,
                        Status = 2,
                        PaymentDate = DateTime.UtcNow,
                        InvoiceStatus = 0,
                        PaymentId = modifiedString,
                        POSTransactionId = POSTransactionId,
                        HalfRefund = false,
                    };
                    _db.Payments.Add(pay);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertPaymentWithSuccessStatus(string PaymentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay = _db.Payments.Where(a => a.PaymentId == PaymentId).FirstOrDefault();
                if (getpay != null)
                {
                    getpay.Status = 1;
                    row = 1;
                }
                var ptlg = _db.PaymentsLog.Where(a => a.TransactionId == PaymentId).FirstOrDefault();
                if (ptlg != null)
                {
                    ptlg.Status = 1;
                    ptlg.Updated = DateTime.UtcNow;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertPaymentWithFailedStatus(string PaymentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay = _db.Payments.Where(a => a.PaymentId == PaymentId).FirstOrDefault();
                if (getpay != null)
                {
                    getpay.Status = 0;
                    row = 1;
                }
                var ptlg = _db.PaymentsLog.Where(a => a.TransactionId == PaymentId).FirstOrDefault();
                if (ptlg != null)
                {
                    ptlg.Status = 0;
                    ptlg.Updated = DateTime.UtcNow;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static List<Specializion> GetDoctorSpecility(int AppointmentId)
        {
            List<Specializion> list = new List<Specializion>();
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var appdet = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            list = (from sp in _db.Doctorspecilization
                    where sp.DocId == appdet.DoctorId && sp.Status == 1 && sp.Name != null
                    select new Specializion
                    {
                        Specialization = sp.Name
                    }).ToList();
            return list;
        }
        public static List<DocCliMap> GetDoctorWorkPlace(int? DoctorId)
        {
            List<DocCliMap> list = new List<DocCliMap>();
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var appdet = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
            list = (from dc in _db.DoctorClinicMap
                    where dc.UserId == appdet.UserId
                    select new DocCliMap
                    {
                        ClinicName = dc.ClinicName,
                        ClinicAddress = dc.ClinicAddress
                    }).ToList();
            return list;
        }
        public static string GetDoctorName(int? DoctorId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
            Name = docnam.SecondName + " " + docnam.FirstName;
            return Name;
        }
        public static string GetDoctorImage(int? DoctorId)
        {
            string Image = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
            Image = docnam.PhotoUrl;
            return Image;
        }
        public static string GetPatientName(int? PatId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Patient.Where(a => a.Id == PatId).FirstOrDefault();
            Name = docnam.SecondName + " " + docnam.FirstName;
            return Name;
        }
        public static int GetAppintmentUserId(int AppointmentId, int? Role)
        {
            int AppointmentUserId = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Role == 4)//Patient
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
            }
            else if (Role == 5)//PharmaRep
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
            }
            else if (Role == 6)//Nurse
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
            }
            else if (Role == 8)//Doctortodoctor
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
            }
            else if (Role == 2)//Doctortodoctor
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
            }
            return AppointmentUserId;
        }
        public static string GetAppintmentUserName(int AppointmentId, int? Role)
        {
            string AppointmentUserName = "";
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Role == 4)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (Role == 5)//PharmaRep
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
                var app = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = app.SecondName + " " + app.FirstName;
            }
            else if (Role == 6)//Nurse
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
                var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (Role == 8)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (Role == 2)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            return AppointmentUserName;
        }
        public static string GetAppintmentUserImage(int AppointmentId, int? Role)
        {
            string AppointmentUserImage = "";
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Role == 4)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.ProfileUrl;
            }
            else if (Role == 5)//PharmaRep
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
                AppointmentUserImage = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().ProfileUrl;
            }
            else if (Role == 6)//Nurse
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
                var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.ProfileUrl;
            }
            else if (Role == 8)//Doctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.PhotoUrl;
            }
            else if (Role == 2)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.PhotoUrl;
            }
            return AppointmentUserImage;
        }
        public static int GetAppintmentDoctorUserId(int AppointmentId, int? Role)
        {
            int AppointmentUserId = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Role == 2)//Doctor
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
            }
            return AppointmentUserId;
        }
        public static int InsertPaymentLogsWithPendingStatus(int Amount, int AppointmentId, string TransactionId)
        {
            int row = 0;
            string modifiedString = TransactionId.Replace("@", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace("'", "").Replace("-", "");
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var appdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            try
            {
                PaymentsLog pay = new PaymentsLog()
                {
                    PatientId = appdata.PatientId,
                    DoctorId = appdata.DoctorId,
                    Amount = Amount,
                    TransactionId = modifiedString,
                    Status = 2,
                    Created = DateTime.UtcNow,
                    PaymentMethod = "",
                    AppointmentId = AppointmentId
                };
                _db.PaymentsLog.Add(pay);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static decimal TotalAmount(int DoctorId)
        {
            decimal TotalAmt = 0;
            decimal finalamount = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 4 && a.IsActive != 0 && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                    decimal discount = 0.00m;
                    discount = TotalAmt * 20 / 100;
                    finalamount = TotalAmt - discount;
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return finalamount;
        }
        public static decimal MonthTotalAmount(int DoctorId, int Month, int Year)
        {
            decimal TotalAmt = 0;
            decimal final = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 4 && a.IsActive != 0 && a.BookingDate.Month == Month && a.BookingDate.Year == Year && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay != null)
                        {
                            if (pay.Amount != 0)
                            {
                                TotalAmt += pay.Amount;
                            }
                        }
                    }
                    decimal discount = 0.00m;
                    discount = TotalAmt * 20 / 100;
                    final = TotalAmt - discount;
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return final;
        }
        public static decimal MonthTotalAmountDoctor(int DoctorId, int Month)
        {
            decimal TotalAmt = 0;
            decimal final = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 4 && a.IsActive != 0 && a.BookingDate.Month == Month && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                    decimal discount = 0.00m;
                    discount = TotalAmt * 20 / 100;
                    final = TotalAmt - discount;
                }
                else
                {
                    final = 0;
                }
            }
            catch (Exception ex)
            {
                final = 0;
            }
            return final;
        }
        public static string GetPayDate(int Month, int Year)
        {
            string Moname = "";
            string Pattern = "";
            if (Month == 1)
            {
                Moname = "Jan";
            }
            else if (Month == 2)
            {
                Moname = "Feb";
            }
            else if (Month == 3)
            {
                Moname = "Mar";
            }
            else if (Month == 4)
            {
                Moname = "Apr";
            }
            else if (Month == 5)
            {
                Moname = "May";
            }
            else if (Month == 6)
            {
                Moname = "June";
            }
            else if (Month == 7)
            {
                Moname = "July";
            }
            else if (Month == 8)
            {
                Moname = "Aug";
            }
            else if (Month == 9)
            {
                Moname = "Sep";
            }
            else if (Month == 10)
            {

                Moname = "Oct";
            }
            else if (Month == 11)
            {
                Moname = "Nov";
            }
            else if (Month == 12)
            {
                Moname = "Dec";
            }
            Pattern = Moname + "-" + Year;
            return Pattern;
        }
        public static int TotalAmountForAdmin()
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(r => r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.IsActive != 3 && r.PatientId != 0).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay != null)
                        {
                            if (pay.Amount != 0)
                            {
                                TotalAmt += (int)pay.Amount;
                            }
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static int TotalAmountForAdminMonthYear(int Month, int Year)
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(r => r.MyRole == 4 && r.UserRole == 2 && r.IsActive != 0 && r.IsActive != 3 && r.PatientId != 0 && r.BookingDate.Month == Month && r.BookingDate.Year == Year).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay != null)
                        {
                            if (pay.Amount != 0)
                            {
                                TotalAmt += (int)pay.Amount;
                            }
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static int PatientTotalAmount(int PatientId)
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.PatientId == PatientId && a.MyRole == 4 && a.IsActive != 0 && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static int PatientMonthTotalAmount(int PatientId, int Month)
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.PatientId == PatientId && a.MyRole == 4 && a.IsActive != 0 && a.BookingDate.Month == Month && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static int PatientYearTotalAmount(int PatientId, int Year)
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.PatientId == PatientId && a.MyRole == 4 && a.IsActive != 0 && a.BookingDate.Year == Year && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static int PatientMonthyaerTotalAmount(int PatientId, int Month, int Year)
        {
            int TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.PatientId == PatientId && a.MyRole == 4 && a.IsActive != 0 && a.BookingDate.Month == Month && a.BookingDate.Year == Year && a.IsActive != 3).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                }
                else
                {
                    TotalAmt = 0;
                }
            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static string GetInvoice(int Id)
        {
            string invoice = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var inv = _db.Payments.Where(a => a.AppoinmentId == Id).FirstOrDefault();
            if (inv != null)
            {
                if (inv.Invoice != null)
                {
                    invoice = inv.Invoice;
                }
            }
            return invoice;
        }
        public static decimal DoctorYearTotalAmount(int DoctorId, int Year)
        {
            decimal TotalAmt = 0;
            decimal final = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 4 && a.IsActive != 3 && a.IsActive != 0 && a.BookingDate.Year == Year).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                    decimal discount = 0.00m;
                    discount = TotalAmt * 20 / 100;
                    final = TotalAmt - discount;
                }
                else
                {
                    final = 0;
                }
            }
            catch (Exception ex)
            {
                final = 0;
            }
            return final;
        }
        //public static int GetDAppintmentUserId(int AppointmentId, int? Role,int? DoctorId)
        //{


        //    int AppointmentUserId = 0;
        //    MyClickDoctorV4 _db = new MyClickDoctorV4();
        //    if(DoctorId==0)
        //    {
        //        if (Role == 4)//Patient
        //        {
        //            AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
        //        }
        //        else if (Role == 5)//PharmaRep
        //        {
        //            AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
        //        }
        //        else if (Role == 6)//Nurse
        //        {
        //            AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
        //        }
        //        else if (Role == 8)//Doctortodoctor
        //        {
        //            AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
        //        }
        //        else if (Role == 2)//Doctortodoctor
        //        {
        //            AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
        //        }
        //    }
        //    else
        //    {
        //        AppointmentUserId = (int)DoctorId;
        //    }

        //    return AppointmentUserId;
        //}
        //public static string GetDAppintmentUserName(int AppointmentId, int? Role, int? DoctorId)
        //{
        //    string AppointmentUserName = "";
        //    int Id = 0;
        //    MyClickDoctorV4 _db = new MyClickDoctorV4();
        //    if (DoctorId == 0)
        //    {
        //        if (Role == 4)//Patient
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
        //            var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserName = data.FirstName + " " + data.SecondName;
        //        }
        //        else if (Role == 5)//PharmaRep
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
        //            AppointmentUserName = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().Name;
        //        }
        //        else if (Role == 6)//Nurse
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
        //            var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserName = data.FirstName + " " + data.SecondName;
        //        }
        //        else if (Role == 8)//Doctortodoctor
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
        //            var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserName = data.FirstName + " " + data.SecondName;
        //        }
        //        else if (Role == 2)//Doctortodoctor
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
        //            var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserName = data.FirstName + " " + data.SecondName;
        //        }
        //    }
        //    else
        //    {
        //        Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
        //        var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //        AppointmentUserName = data.FirstName + " " + data.SecondName;
        //    }

        //    return AppointmentUserName;
        //}
        //public static string GetDAppintmentUserImage(int AppointmentId, int? Role, int? DoctorId)
        //{
        //    string AppointmentUserImage = "";
        //    int Id = 0;
        //    MyClickDoctorV4 _db = new MyClickDoctorV4();
        //    if (DoctorId == 0)
        //    {
        //        if (Role == 4)//Patient
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
        //            var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserImage = data.ProfileUrl;
        //        }
        //        else if (Role == 5)//PharmaRep
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
        //            AppointmentUserImage = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().ProfileUrl;
        //        }
        //        else if (Role == 6)//Nurse
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
        //            var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserImage = data.ProfileUrl;
        //        }
        //        else if (Role == 8)//Doctor
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
        //            var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserImage = data.PhotoUrl;
        //        }
        //        else if (Role == 2)//Doctortodoctor
        //        {
        //            Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
        //            var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //            AppointmentUserImage = data.PhotoUrl;
        //        }
        //    }
        //    else
        //    {
        //        Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
        //        var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
        //        AppointmentUserImage = data.PhotoUrl;
        //    } 
        //    return AppointmentUserImage;
        //}
        public static List<Specializion> GetDoctorSpeciality(int DocId, string Lang)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            List<Specializion> getlist = new List<Specializion>();
            try
            {
                if (Lang == "en")
                {
                    getlist = (from sp in _db.Doctorspecilization
                               where sp.DocId == DocId && sp.Status == 1 && sp.Name != null
                               select new Specializion
                               {
                                   // Specialization = sp.Name
                                   Specialization = _db.SpecilizationCategory.Where(a => a.Id == sp.CatId).FirstOrDefault().Name
                               }).ToList();
                }
                else
                {
                    getlist = (from sp in _db.Doctorspecilization
                               where sp.DocId == DocId && sp.Status == 1 && sp.Name != null
                               select new Specializion
                               {
                                   Specialization = _db.SpecilizationCategory.Where(a => a.Id == sp.CatId).FirstOrDefault().HunName
                               }).ToList();
                }
            }
            catch (Exception ex)
            {
                return getlist;
            }
            return getlist;
        }
        public static List<SpecialCategory> GetDoctorProfikeSpeciality(int DocId, string Lang)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            List<SpecialCategory> getlist = new List<SpecialCategory>();
            try
            {
                if (Lang == "en")
                {
                    getlist = (from sp in _db.Doctorspecilization
                               where sp.DocId == DocId && sp.Status == 1 && sp.Name != null
                               select new SpecialCategory
                               {
                                   Id = sp.Id,
                                   CatId = sp.CatId,
                                   FirstName = sp.Name
                               }).ToList();
                }
                else
                {
                    getlist = (from sp in _db.Doctorspecilization
                               where sp.DocId == DocId && sp.Status == 1 && sp.Name != null
                               select new SpecialCategory
                               {
                                   Id = sp.Id,
                                   CatId = sp.CatId,
                                   FirstName = _db.SpecilizationCategory.Where(a => a.Id == sp.CatId).FirstOrDefault().HunName
                               }).ToList();
                }

            }
            catch (Exception ex)
            {
                return getlist;
            }
            return getlist;
        }
        public static int UpdateAppointmentStatus(int AppointmentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (getpay != null)
                {
                    if (getpay.CancelBy == 1)
                    {
                        getpay.IsActive = 0;
                    }
                    else
                    {
                        getpay.IsActive = 1;
                        var updatedocslot = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == getpay.DocSlotId).FirstOrDefault();
                        if (updatedocslot != null)
                        {
                            updatedocslot.Status = 1;
                        }
                    }
                    row = 1;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int GetAppointmentId(string PaymentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay = _db.Payments.Where(a => a.PaymentId == PaymentId).FirstOrDefault();
                if (getpay != null)
                {
                    row = (int)getpay.AppoinmentId;
                }
                //  _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int CheckDoctorAwailablity(int Awialabilty, int Id)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int row = 0;
            if (Awialabilty == 1)
            {
                var doct = _db.DoctorAvailibiltyforAppointment.Where(a => a.ToTime.Date == DateTime.UtcNow.Date && a.DocId == Id).FirstOrDefault();
                if (doct != null)
                {
                    row = 1;
                }
            }
            else if (Awialabilty == 2)
            {
                var doct = _db.DoctorAvailibiltyforAppointment.Where(a => a.ToTime.Date == DateTime.UtcNow.Date.AddDays(1) && a.DocId == Id).FirstOrDefault();
                if (doct != null)
                {
                    row = 1;
                }
            }
            else if (Awialabilty == 7)
            {
                var doct = _db.DoctorAvailibiltyforAppointment.Where(week => week.ToTime.Date >= DateTime.UtcNow.Date && week.ToTime <= DateTime.UtcNow.Date.AddDays(7) && week.DocId == Id).FirstOrDefault();
                if (doct != null)
                {
                    row = 1;
                }
            }
            else if (Awialabilty == 0)
            {
                row = 0;
            }
            return row;
        }
        public static int CheckDoctortoDoctorAvailability(int DoctorId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                int GetDoctorWeekCount = (int)_db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault().DoctorConslutationCount;
                DateTime Monday = DateTime.Now;
                DateTime Saturday = DateTime.Now;
                DayOfWeek day = DateTime.Now.DayOfWeek;
                Monday = DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek - DayOfWeek.Monday));
                Saturday = DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek - DayOfWeek.Saturday));
                var Doctor = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 8 && a.StartDate.Date >= Monday.Date && Saturday.Date <= a.StartDate).ToList();
                if (Doctor.Count > 0)
                {
                    row = GetDoctorWeekCount - Doctor.Count();
                }
                else
                {
                    row = GetDoctorWeekCount;
                }
            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static int CancelMeetingActivity(int AppointmentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var actlst = _db.ActivityList.Where(a => a.AppointmentId == AppointmentId).FirstOrDefault();
                if (actlst != null)
                {
                    var checkactivity = _db.ActivityList.Where(a => a.AppointmentId == AppointmentId && a.Status == 0).FirstOrDefault();
                    if (checkactivity != null)
                    {
                        row = 0;
                    }
                    else
                    {
                        ActivityList lst = new ActivityList()
                        {
                            AppointmentId = AppointmentId,
                            PharmaId = actlst.PharmaId,
                            DocId = actlst.DocId,
                            UserType = actlst.UserType,
                            Status = 0,
                            NurseId = actlst.NurseId,
                            PatientId = actlst.PatientId,
                            DocToDocId = actlst.DocToDocId
                        };
                        _db.ActivityList.Add(lst);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static int DoneMeetingActivity(int AppointmentId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var actlst = _db.ActivityList.Where(a => a.AppointmentId == AppointmentId).FirstOrDefault();
                if (actlst != null)
                {
                    var checkactivity = _db.ActivityList.Where(a => a.AppointmentId == AppointmentId && a.Status == 2).FirstOrDefault();
                    if (checkactivity != null)
                    {
                        row = 0;
                    }
                    else
                    {
                        ActivityList lst = new ActivityList()
                        {
                            AppointmentId = AppointmentId,
                            PharmaId = actlst.PharmaId,
                            DocId = actlst.DocId,
                            UserType = actlst.UserType,
                            Status = 2,
                            NurseId = actlst.NurseId,
                            PatientId = actlst.PatientId,
                            DocToDocId = actlst.DocToDocId
                        };
                        _db.ActivityList.Add(lst);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static string GetDoctorTypeName(int? TypeId, string Lang)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay = _db.DoctorCategoryType.Where(a => a.Id == TypeId).FirstOrDefault();
                if (getpay != null)
                {
                    if (Lang == "en")
                    {
                        Name = getpay.Name;
                    }
                    else
                    {
                        Name = getpay.HunName;
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Name;
            }
            return Name;
        }
        public static string GetDoctorPrice(int? TypeId)
        {
            string Price = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var getpay15 = _db.PriceListMaster.Where(a => a.CategoryId == TypeId && a.Duration == 15).OrderByDescending(a => a.Id).FirstOrDefault();
                var getpay60 = _db.PriceListMaster.Where(a => a.CategoryId == TypeId && a.Duration == 60).OrderByDescending(a => a.Id).FirstOrDefault();
                if (getpay15 != null && getpay60 != null)
                {
                    Price = getpay15.AppointmentRate.ToString() + "-" + getpay60.AppointmentRate.ToString();
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Price;
            }
            return Price;
        }
        public static int GetDoctorAppointmentId(int AppointmentId)
        {
            int AppointmentUserId = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var data = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            if (data != null)
            {
                if (data.DoctorId != 0 && data.DoctorId != null)
                {
                    AppointmentUserId = (int)data.DoctorId;
                }
                else
                {
                    AppointmentUserId = 0;
                }
            }
            return AppointmentUserId;
        }
        public static int CheckDoctorAvalabilityForDoctorPharmaAppointment(int DoctorId, DateTime StartDate, DateTime EndDate)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var CheckDocotorAppointment = _db.Appoinments.OrderByDescending(a => a.Id).Where(a => a.DoctorId == DoctorId && a.IsActive != 0 && ((StartDate >= a.StartDate && StartDate <= a.EndDate) || (EndDate >= a.StartDate && EndDate <= a.EndDate))).FirstOrDefault();
                if (CheckDocotorAppointment != null)
                {
                    row = 0;
                }
                else
                {
                    var CheckAvailablity = _db.DoctorAvailibiltyforAppointment.Where(a => a.DocId == DoctorId && a.FromTime >= StartDate && EndDate <= a.FromTime).FirstOrDefault();
                    if (CheckAvailablity != null)
                    {
                        row = 1;
                    }
                    else
                    {
                        row = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static string GetMultiDoctorName(string AppId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            string Name = "";
            var GetAppp = _db.Appoinments.Where(a => a.Appoinmentcode == AppId).ToList();
            if (GetAppp.Count > 0)
            {
                foreach (var item in GetAppp)
                {
                    if (Name == "")
                    {
                        var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                        Name = doc.SecondName + " " + doc.FirstName;
                    }
                    else
                    {
                        var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                        Name = Name + "," + doc.SecondName + " " + doc.FirstName;
                    }
                }
            }
            return Name;
        }
        public static int GetNurseAppintmentUserId(int AppointmentId, int? Role, int? Id)
        {
            int AppointmentUserId = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Id != 0 && Id != null)//Patient
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
            }
            else if (Id == 0 || Id == null)//Doctor
            {
                AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
            }
            //else if (Role == 5)//PharmaRep
            //{
            //    AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
            //}
            //else if (Role == 8)//Doctortodoctor
            //{
            //    AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
            //}
            //else if (Role == 6)//Nurse
            //{
            //    AppointmentUserId = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
            //}


            return AppointmentUserId;
        }
        public static string GetNurseAppintmentUserName(int AppointmentId, int? Role, int? PatId)
        {
            string AppointmentUserName = "";
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (PatId != 0 && PatId != null)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (PatId == 0 || PatId == null)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            //else if (Role == 5)//PharmaRep
            //{
            //    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
            //    AppointmentUserName = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().Name;
            //}
            //else if (Role == 6)//Nurse
            //{
            //    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
            //    var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
            //    AppointmentUserName = data.FirstName + " " + data.SecondName;
            //}
            //else if (Role == 8)//Doctortodoctor
            //{
            //    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
            //    var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
            //    AppointmentUserName = data.FirstName + " " + data.SecondName;
            //}

            return AppointmentUserName;
        }
        public static string GetNurseAppintmentUserImage(int AppointmentId, int? Role, int? PatId)
        {
            string AppointmentUserImage = "";
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (PatId != 0 && PatId != null)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.ProfileUrl;
            }
            else if (PatId == 0 || PatId == null)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserImage = data.PhotoUrl;
            }
            ////else if (Role == 5)//PharmaRep
            ////{
            ////    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
            ////    AppointmentUserImage = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().ProfileUrl;
            ////}
            ////else if (Role == 6)//Nurse
            ////{
            ////    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
            ////    var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
            ////    AppointmentUserImage = data.ProfileUrl;
            ////}
            ////else if (Role == 8)//Doctor
            ////{
            ////    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
            ////    var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
            ////    AppointmentUserImage = data.PhotoUrl;
            ////}

            return AppointmentUserImage;
        }
        public static string UpdateNotificationStatus()
        {
            string AppointmentUserImage = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var list = _db.NotificationBox.Where(a => a.Status == 0 || a.Status == 1).ToList();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var check = _db.Appoinments.Where(a => a.Appoinmentcode == item.MeetingId).FirstOrDefault();
                    if (check != null)
                    {
                        if (DateTime.UtcNow >= check.EndDate)
                        {
                            item.Status = 3;
                            _db.SaveChanges();
                        }
                    }
                }
            }
            return AppointmentUserImage;
        }
        public static int InsertPatientNurseActivity(int AppId, int NurseId, int PatientId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var data = _db.ActivityList.Where(a => a.AppointmentId == AppId && a.Status == 1).FirstOrDefault();
                if (data == null)
                {
                    ActivityList acl = new ActivityList()
                    {
                        AppointmentId = AppId,
                        NurseId = NurseId,
                        PatientId = PatientId,
                        UserType = 4,
                        Status = 1
                    };
                    _db.ActivityList.Add(acl);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertNurseDoctorActivity(int AppId, int NurseId, int DoctorId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var data = _db.ActivityList.Where(a => a.AppointmentId == AppId && a.Status == 1).FirstOrDefault();
                if (data == null)
                {
                    ActivityList acl = new ActivityList()
                    {
                        AppointmentId = AppId,
                        NurseId = NurseId,
                        DocId = DoctorId,
                        UserType = 6,
                        Status = 1
                    };
                    _db.ActivityList.Add(acl);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertPatientDoctorActivity(int AppId, int PatientId, int DoctorId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var data = _db.ActivityList.Where(a => a.AppointmentId == AppId && a.Status == 1).FirstOrDefault();
                if (data == null)
                {
                    ActivityList acl = new ActivityList()
                    {
                        AppointmentId = AppId,
                        PatientId = PatientId,
                        DocId = DoctorId,
                        UserType = 4,
                        Status = 1
                    };
                    _db.ActivityList.Add(acl);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertDoctorDoctorActivity(int AppId, int DoctorToId, int DoctorId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var data = _db.ActivityList.Where(a => a.AppointmentId == AppId && a.Status == 1).FirstOrDefault();
                if (data == null)
                {
                    ActivityList acl = new ActivityList()
                    {
                        AppointmentId = AppId,
                        DocToDocId = DoctorToId,
                        DocId = DoctorId,
                        UserType = 2,
                        Status = 1
                    };
                    _db.ActivityList.Add(acl);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int InsertPharmaDoctorActivity(int AppId, int PharmaRepId, int DoctorId)
        {
            int row = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var data = _db.ActivityList.Where(a => a.AppointmentId == AppId && a.Status == 1).FirstOrDefault();
                if (data == null)
                {
                    ActivityList acl = new ActivityList()
                    {
                        AppointmentId = AppId,
                        PharmaId = PharmaRepId,
                        DocId = DoctorId,
                        UserType = 5,
                        Status = 1
                    };
                    _db.ActivityList.Add(acl);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return row;
            }
            return row;
        }
        public static int GetBussinessStatusForMeeting(int Role, int AppointmentId, string MeetingId = "")
        {
            int BussinessStatus = 0;
            int UserId = 0;
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (MeetingId != null && MeetingId != "")
            {
                var app = _db.Appoinments.Where(a => a.Appoinmentcode == MeetingId).ToList();
                if (app.Count > 1)
                {
                    BussinessStatus = 3;
                    return BussinessStatus;
                }
            }
            if (Role == 4)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                UserId = _db.Patient.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 5)//PharmaRep
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
                UserId = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 6)//Nurse
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
                UserId = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 8)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
                UserId = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 2)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                UserId = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            try
            {
                var userdata = _db.Users.Where(a => a.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    TimeSpan Total = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                    int Time = Math.Abs((int)Total.TotalMinutes);
                    if (Time <= 2)
                    {
                        BussinessStatus = 2;
                        userdata.BussinessStatus = 2;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.Lastlogin.TimeOfDay;
                    int mnts = Math.Abs((int)mint.TotalMinutes);
                    if (mnts <= 2)
                    {
                        BussinessStatus = 1;
                        userdata.BussinessStatus = 1;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    else
                    {
                        BussinessStatus = 0;
                        userdata.BussinessStatus = 0;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                }
                else
                {
                    BussinessStatus = 0;
                    return BussinessStatus = 0;
                }
            }
            catch (Exception ex)
            {
                BussinessStatus = 0;
                return BussinessStatus = 0;
            }
        }
        public static int GetDoctorBussinessStatusForMeeting(int Role, int AppointmentId, string MeetingId = "", int DoctorId = 0)
        {
            int BussinessStatus = 0;
            int UserId = 0;
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (MeetingId != null && MeetingId != "")
            {
                var app = _db.Appoinments.Where(a => a.Appoinmentcode == MeetingId).ToList();
                if (app.Count > 1)
                {
                    BussinessStatus = 3;
                    return BussinessStatus;
                }
            }
            if (Role == 4)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                UserId = _db.Patient.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 5)//PharmaRep
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
                UserId = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 6)//Nurse
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
                UserId = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            else if (Role == 8)//Doctortodoctor
            {
                var Appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (DoctorId == Appdetail.DoctorId)
                {
                    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
                    UserId = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault().UserId;
                }
                else
                {
                    Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                    UserId = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault().UserId;
                }
            }
            else if (Role == 2)//Doctortodoctor
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorId;
                UserId = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault().UserId;
            }
            try
            {
                var userdata = _db.Users.Where(a => a.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    TimeSpan Total = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                    int Time = Math.Abs((int)Total.TotalMinutes);
                    if (Time <= 2)
                    {
                        BussinessStatus = 2;
                        userdata.BussinessStatus = 2;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.Lastlogin.TimeOfDay;
                    int mnts = Math.Abs((int)mint.TotalMinutes);
                    if (mnts <= 2)
                    {
                        BussinessStatus = 1;
                        userdata.BussinessStatus = 1;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                    else
                    {
                        BussinessStatus = 0;
                        userdata.BussinessStatus = 0;
                        _db.SaveChanges();
                        return BussinessStatus;
                    }
                }
                else
                {
                    BussinessStatus = 0;
                    return BussinessStatus = 0;
                }
            }
            catch (Exception ex)
            {
                BussinessStatus = 0;
                return BussinessStatus = 0;
            }
        }
        public static int GetSlotId(int AppointmentId, int Type)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime crnttime = DateTime.UtcNow;
            int slotid = 0;
            try
            {
                var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (Type == 2)
                {
                    slotid = (int)appdetail.DocSlotId;
                }
                else if (Type == 6)
                {
                    slotid = (int)appdetail.NurseSlotId;
                }
            }
            catch (Exception ex)
            {
                slotid = 0;
            }
            return slotid;
        }
        public static decimal GetAmount(int AppointmentId, int Type)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime crnttime = DateTime.UtcNow;
            decimal amt = 0;
            try
            {
                if (Type == 2)
                {
                    var pay = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                    if (pay != null)
                    {
                        amt = pay.Amount;
                    }
                    else
                    {
                        amt = 0.00M;
                    }

                }
            }
            catch (Exception ex)
            {
                amt = 0.00M;
            }
            return amt;
        }
        public static string SendAppointmentBookingConfirmation(int AppointmentId)
        {
            string CancelledBy = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var meetdata = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
            // DateTime startTimeFormate = meetdata.StartDate; // This  is utc date time
            //TimeZoneInfo systemTimeZone = ;
            //DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
            //string formattedDate = localDateTime.ToString("yyyy-MM-dd HH:mm");
            var timeUtc = meetdata.StartDate;
            var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
            string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
            if (meetdata.UserRole == 2 && meetdata.MyRole == 8)
            {
                int DoctorId = GetAppintmentUserId(meetdata.Id, 8);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int DoctorConsId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var DocConst = _db.Doctors.Where(a => a.Id == DoctorConsId).FirstOrDefault();
                string DocConsEmail = _db.Users.Where(a => a.Id == DocConst.UserId).FirstOrDefault().Email;

                //   AppCommonLogic.SendAppointmentBookingConfirmation(DocConsEmail, DocConst.SecondName + " " + DocConst.FirstName, Doctor.SecondName + " " + Doctor.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                AppCommonLogic.SendAppointmentBookingConfirmation(Email, Doctor.SecondName + " " + Doctor.FirstName, DocConst.SecondName + " " + DocConst.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 5)
            {
                int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int Pharmaid = GetAppintmentUserId(meetdata.Id, 5);
                var phrma = _db.PharmaRepresentative.Where(a => a.Id == Pharmaid).FirstOrDefault();
                string PhrmaEmail = _db.Users.Where(a => a.Id == phrma.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendAppointmentBookingConfirmation(Email, Doctor.SecondName + " " + Doctor.FirstName, phrma.SecondName + " " + phrma.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                AppCommonLogic.SendAppointmentBookingConfirmation(PhrmaEmail, phrma.SecondName + " " + phrma.FirstName, Doctor.SecondName + " " + Doctor.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 4)
            {
                if (meetdata.IsPaymentEmail != true)
                {
                    int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                    var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                    string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                    int Patid = GetAppintmentUserId(meetdata.Id, 4);
                    var pt = _db.Patient.Where(a => a.Id == Patid).FirstOrDefault();
                    string ptEmail = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault().Email;
                    AppCommonLogic.SendAppointmentBookingConfirmation(Email, Doctor.SecondName + " " + Doctor.FirstName, pt.SecondName + " " + pt.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                    AppCommonLogic.SendAppointmentBookingConfirmation(ptEmail, pt.SecondName + " " + pt.FirstName, Doctor.SecondName + " " + Doctor.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                    meetdata.IsPaymentEmail = true;
                    _db.SaveChanges();
                }
            }
            else if (meetdata.UserRole == 2 && meetdata.MyRole == 6)
            {
                int DoctorId = GetAppintmentDoctorUserId(meetdata.Id, 2);
                var Doctor = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == Doctor.UserId).FirstOrDefault().Email;

                int nurid = GetAppintmentUserId(meetdata.Id, 6);
                var nr = _db.Nurses.Where(a => a.Id == nurid).FirstOrDefault();
                string nrEmail = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendAppointmentBookingConfirmation(Email, Doctor.SecondName + " " + Doctor.FirstName, nr.SecondName + " " + nr.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                AppCommonLogic.SendAppointmentBookingConfirmation(nrEmail, nr.SecondName + " " + nr.FirstName, Doctor.SecondName + " " + Doctor.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
            }
            else if (meetdata.UserRole == 6 && meetdata.MyRole == 4)
            {
                int ptId = GetAppintmentUserId(meetdata.Id, 4);
                var pt = _db.Patient.Where(a => a.Id == ptId).FirstOrDefault();
                string Email = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault().Email;

                int nurid = GetAppintmentUserId(meetdata.Id, 6);
                var nr = _db.Nurses.Where(a => a.Id == nurid).FirstOrDefault();
                string nrEmail = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault().Email;
                AppCommonLogic.SendAppointmentBookingConfirmation(Email, pt.SecondName + " " + pt.FirstName, nr.SecondName + " " + nr.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
                AppCommonLogic.SendAppointmentBookingConfirmation(nrEmail, nr.SecondName + " " + nr.FirstName, pt.SecondName + " " + pt.FirstName, meetdata.AppointmentTitle, (int)meetdata.Duration, formattedDate);
            }
            return CancelledBy;
        }
        public static int GetNurseSuggestionStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            try
            {
                var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (appdetail != null)
                {
                    if (appdetail.NurseSuggestionStatus != null)
                    {
                        Status = (int)appdetail.NurseSuggestionStatus;
                    }
                    else
                    {
                        Status = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 2;
            }
            return Status;
        }
        public static int GetJoinPopUp(int AppointmentId, int usertype)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            try
            {
                var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (appdetail != null)
                {
                    if (usertype == 4)
                    {
                        if (appdetail.PatientPopup != null)
                        {
                            Status = (int)appdetail.PatientPopup;
                        }
                    }
                    else if (usertype == 5)
                    {
                        if (appdetail.PharmaPopup != null)
                        {
                            Status = (int)appdetail.PharmaPopup;
                        }
                    }
                    else if (usertype == 6)
                    {
                        if (appdetail.NursePopup != null)
                        {
                            Status = (int)appdetail.NursePopup;
                        }
                    }
                    else if (usertype == 2)
                    {
                        if (appdetail.DoctorPopup != null)
                        {
                            Status = (int)appdetail.DoctorPopup;
                        }
                    }
                    else if (usertype == 8)
                    {
                        if (appdetail.DoctortoPopup != null)
                        {
                            Status = (int)appdetail.DoctortoPopup;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 0;
            }
            return Status;
        }
        public static int GetDoctorJoinPopUp(int AppointmentId, int DocId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            try
            {
                var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (appdetail != null)
                {
                    if (DocId == appdetail.DoctorId)
                    {
                        if (appdetail.DoctorPopup != null)
                        {
                            Status = (int)appdetail.DoctorPopup;
                        }
                    }
                    else
                    {
                        if (appdetail.DoctortoPopup != null)
                        {
                            Status = (int)appdetail.DoctortoPopup;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 0;
            }
            return Status;
        }
        public static string GetRefundStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            string RefundStatus = "";
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.RefundStatus != null)
                    {
                        Status = (int)refund.RefundStatus;
                        if (Status == 0)
                        {
                            RefundStatus = App_Resources.AppResource.NotRefund;
                        }
                        else if (Status == 1)
                        {
                            RefundStatus = App_Resources.AppResource.Refund_status;
                        }
                    }
                    else
                    {
                        RefundStatus = App_Resources.AppResource.NotRefund;
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 0;
            }
            return RefundStatus;
        }
        public static int GetRefundStatusforPayment(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).OrderByDescending(a => a.Id).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.RefundStatus != null)
                    {
                        Status = (int)refund.RefundStatus;
                    }
                    else
                    {
                        Status = 0;
                    }
                }
                else
                {
                    Status = 0;
                }
            }
            catch (Exception ex)
            {
                Status = 0;
            }
            return Status;
        }
        public static int GetAmount(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Amount = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.Amount != null && refund.Amount != 0)
                    {
                        if (refund.HalfRefund == true)
                        {
                            Amount = (int)refund.Amount / 2;
                        }
                        else
                        {
                            Amount = (int)refund.Amount;
                        }
                    }
                    else
                    {
                        Amount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Amount = 0;
            }
            return Amount;
        }
        public static DateTime GetRefundDate(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime dt = new DateTime();
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.RefundDate != null)
                    {
                        dt = (DateTime)refund.RefundDate;
                    }
                    else
                    {
                        dt = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                dt = DateTime.UtcNow;
            }
            return dt;
        }
        public static int UpdatePaymentRefundStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int row = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    refund.RefundStatus = 1;
                    refund.RefundDate = DateTime.UtcNow;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static int UpdateDoctorStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int total = 0;
            try
            {
                var refund = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    refund.DoctorCancelStatus = 1;
                    refund.IsActive = 0;
                    _db.SaveChanges();
                }
                var dc = _db.Appoinments.Where(a => a.DoctorId == refund.DoctorId).ToList();
                if (dc.Count > 0)
                {
                    foreach (var item in dc)
                    {
                        if (item.DoctorCancelStatus != 0 && item.DoctorCancelStatus != null)
                        {
                            total += 1;
                        }
                    }
                    if (total == 1)
                    {
                        var dcprofile = _db.Doctors.Where(a => a.Id == refund.DoctorId).FirstOrDefault();
                        string Email = _db.Users.Where(a => a.Id == dcprofile.UserId).FirstOrDefault().Email;
                        AppCommonLogic.sendwarningmailtodoctor(Email, dcprofile.SecondName + " " + dcprofile.FirstName);
                    }
                    if (total >= 2)
                    {
                        var profile = _db.Doctors.Where(a => a.Id == refund.DoctorId).FirstOrDefault();
                        profile.Status = 0;
                        _db.SaveChanges();
                        var user = _db.Users.Where(a => a.Id == profile.UserId).FirstOrDefault();
                        user.Status = 0;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                total = 0;
            }
            return total;
        }
        public static int CancelPaymenttStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int total = 0;
            try
            {
                var refund = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    refund.CancelBy = 1;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                total = 0;
            }
            return total;
        }
        public static decimal Get80percentAmount(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime crnttime = DateTime.UtcNow;
            decimal amt = 0;
            try
            {

                var pay = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (pay != null)
                {
                    decimal discount = 0.00m;
                    discount = pay.Amount * 20 / 100;
                    amt = pay.Amount - discount;
                }
                else
                {
                    amt = 0.00M;
                }
            }
            catch (Exception ex)
            {
                amt = 0.00M;
            }
            return amt;
        }
        public static int UpdatePaymentHalfRefundStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int row = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    refund.HalfRefund = true;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                row = 0;
            }
            return row;
        }
        public static int DeleteUserProfile(int Id, int usertype)
        {
            int i = 0;
            try
            {
                MyClickDoctorV4 _db = new MyClickDoctorV4();
                var use = _db.Users.Where(a => a.Id == Id).FirstOrDefault();
                if (use != null)
                {
                    use.Status = 0;
                    _db.SaveChanges();
                }
                if (usertype == 2)
                {

                    var doc = _db.Doctors.Where(a => a.UserId == Id).FirstOrDefault();
                    if (doc != null)
                    {
                        doc.Status = 0;
                        _db.SaveChanges();
                    }
                }
                else if (usertype == 4)
                {
                    var pat = _db.Patient.Where(a => a.UserId == Id).FirstOrDefault();
                    if (pat != null)
                    {
                        pat.Status = 0;
                        _db.SaveChanges();
                    }
                }
                else if (usertype == 6)
                {
                    var nur = _db.Nurses.Where(a => a.UserId == Id).FirstOrDefault();
                    if (nur != null)
                    {
                        nur.IsAvailable = 0;
                        _db.SaveChanges();
                    }
                }
                else if (usertype == 5)
                {
                    var pha = _db.PharmaRepresentative.Where(a => a.UserId == Id).FirstOrDefault();
                    if (pha != null)
                    {
                        pha.Status = 0;
                        _db.SaveChanges();
                    }
                }
                return i;
            }
            catch (Exception ex)
            {
                return i;
            }
        }
        public static string GetUserAvailable(string MeetingId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var Appointmentlist = _db.Appoinments.Where(a => a.Appoinmentcode == MeetingId).ToList();
                if (Appointmentlist.Count > 0)
                {
                    foreach (var item in Appointmentlist)
                    {
                        if (item.UserRole == 2 && item.MyRole == 4)
                        {
                            int PatientId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PatientId;
                            var Pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == Pat.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Pat.SecondName + " " + Pat.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Pat.SecondName + " " + Pat.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 6)
                        {
                            int NurseId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().NurseId;
                            var Nur = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == Nur.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Nur.SecondName + " " + Nur.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Nur.SecondName + " " + Nur.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 5)
                        {
                            int PharmaRepId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PharmaRepId;
                            var pharma = _db.PharmaRepresentative.Where(a => a.Id == PharmaRepId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == pharma.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = pharma.SecondName + " " + pharma.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + pharma.SecondName + " " + pharma.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 8)
                        {
                            int DoctorConsultationId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorConsultationId;
                            var dc = _db.Doctors.Where(a => a.Id == DoctorConsultationId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == dc.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = dc.SecondName + " " + dc.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + dc.SecondName + " " + dc.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 6 && item.MyRole == 4)
                        {
                            int NurseId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().NurseId;
                            var nr = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = nr.SecondName + " " + nr.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + nr.SecondName + " " + nr.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int PatientId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PatientId;
                            var pt = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                            var ptuserdata = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - ptuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts <= 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = pt.SecondName + " " + pt.FirstName + " " + "(" + ptuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + pt.SecondName + " " + pt.FirstName + " " + "(" + ptuserdata.PhoneNo + ")";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Name = "";
            }
            return Name;
        }
        public static string GetUserNotAvailable(string MeetingId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var Appointmentlist = _db.Appoinments.Where(a => a.Appoinmentcode == MeetingId).ToList();
                if (Appointmentlist.Count > 0)
                {
                    foreach (var item in Appointmentlist)
                    {
                        if (item.UserRole == 2 && item.MyRole == 4)
                        {
                            int PatientId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PatientId;
                            var Pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == Pat.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Pat.SecondName + " " + Pat.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Pat.SecondName + " " + Pat.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 6)
                        {
                            int NurseId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().NurseId;
                            var Nur = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == Nur.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Nur.SecondName + " " + Nur.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Nur.SecondName + " " + Nur.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 5)
                        {
                            int PharmaRepId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PharmaRepId;
                            var pharma = _db.PharmaRepresentative.Where(a => a.Id == PharmaRepId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == pharma.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = pharma.SecondName + " " + pharma.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + pharma.SecondName + " " + pharma.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 2 && item.MyRole == 8)
                        {
                            int DoctorConsultationId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorConsultationId;
                            var dc = _db.Doctors.Where(a => a.Id == DoctorConsultationId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == dc.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = dc.SecondName + " " + dc.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + dc.SecondName + " " + dc.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int DoctorId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().DoctorId;
                            var Doc = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
                            var Docuserdata = _db.Users.Where(a => a.Id == Doc.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - Docuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + Doc.SecondName + " " + Doc.FirstName + " " + "(" + Docuserdata.PhoneNo + ")";
                                }
                            }
                        }
                        else if (item.UserRole == 6 && item.MyRole == 4)
                        {
                            int NurseId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().NurseId;
                            var nr = _db.Nurses.Where(a => a.Id == NurseId).FirstOrDefault();
                            var userdata = _db.Users.Where(a => a.Id == nr.UserId).FirstOrDefault();
                            TimeSpan mint = DateTime.UtcNow.TimeOfDay - userdata.LastMeeting.TimeOfDay;
                            int mnts = Math.Abs((int)mint.TotalMinutes);
                            if (mnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = nr.SecondName + " " + nr.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + nr.SecondName + " " + nr.FirstName + " " + "(" + userdata.PhoneNo + ")";
                                }
                            }
                            int PatientId = (int)_db.Appoinments.Where(a => a.Id == item.Id).FirstOrDefault().PatientId;
                            var pt = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                            var ptuserdata = _db.Users.Where(a => a.Id == pt.UserId).FirstOrDefault();
                            TimeSpan docmint = DateTime.UtcNow.TimeOfDay - ptuserdata.LastMeeting.TimeOfDay;
                            int dcmnts = Math.Abs((int)docmint.TotalMinutes);
                            if (dcmnts > 2)
                            {
                                if (Name == null && Name == "")
                                {
                                    Name = pt.SecondName + " " + pt.FirstName + " " + "(" + ptuserdata.PhoneNo + ")";
                                }
                                else
                                {
                                    Name = Name + "," + pt.SecondName + " " + pt.FirstName + " " + "(" + ptuserdata.PhoneNo + ")";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Name = "";
            }
            return Name;
        }
        public static int geimgno(int no)
        {
            int nm = 0;
            if (no % 2 == 0)
            {
                nm = 2;
            }
            else
            {
                nm = 1;
            }
            return nm;
        }
        public static int GetIssuePopUp(int AppointmentId, int Usertype, int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pop = _db.InvitationMaster.Where(a => a.AppointmentId == AppointmentId).FirstOrDefault();
                if (pop != null)
                {
                    if (pop.UserId != UserId)
                    {
                        if (Usertype == 2)
                        {
                            status = (int)pop.DoctorPopupStatus;
                        }
                        else if (Usertype == 4)
                        {
                            status = (int)pop.PatientPopupStatus;
                        }
                        else if (Usertype == 5)
                        {
                            status = (int)pop.PharmaPopupStatus;
                        }
                        else if (Usertype == 6)
                        {
                            status = (int)pop.NursePopupStatus;
                        }
                    }
                    else
                    {
                        status = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 1;
            }
            return status;
        }
        public static int GetPaymentStatusforPayment(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Status = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).OrderByDescending(a => a.Id).FirstOrDefault();
                if (refund != null)
                {
                    Status = 1;
                }
                else
                {
                    Status = 0;
                }
            }
            catch (Exception ex)
            {
                Status = 0;
            }
            return Status;
        }
        public static int GetPatientAmount(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int Amount = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.Amount != null && refund.Amount != 0)
                    {
                        Amount = (int)refund.Amount;
                    }
                    else
                    {
                        Amount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Amount = 0;
            }
            return Amount;
        }
        public static DateTime GetPatientAmountDate(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            DateTime date = new DateTime();
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.PaymentDate != null)
                    {
                        date = (DateTime)refund.PaymentDate;
                    }
                }
            }
            catch (Exception ex)
            {
                date = DateTime.UtcNow;
            }
            return date;
        }
        public static int GetInvoiceStatus(int AppointmentId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var refund = _db.Payments.Where(a => a.AppoinmentId == AppointmentId).FirstOrDefault();
                if (refund != null)
                {
                    if (refund.InvoiceStatus != null && refund.InvoiceStatus != 0)
                    {
                        status = (int)refund.InvoiceStatus;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }
        public static int GetwizardStatus(int PatientId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                if (pat != null)
                {
                    if (pat.UpdatedAt.Date == DateTime.UtcNow.Date)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            updatewizardStatus(PatientId);
            return status;
        }
        public static int GetwizardStatusforpt(int PatientId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                if (pat != null)
                {
                    if (pat.UpdatedAt.Date == DateTime.UtcNow.Date)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            // updatewizardStatus(PatientId);
            return status;
        }
        public static int updatewizardStatus(int PatientId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                if (pat != null)
                {
                    pat.UpdatedAt = DateTime.UtcNow;
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }
        public static int GetwizardStatusformydoctor(int PatientId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                if (pat != null)
                {
                    if (pat.UpdatedAt.Date == DateTime.UtcNow.Date)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }
            updatewizardformydoctorStatus(PatientId);
            return status;
        }
        public static int updatewizardformydoctorStatus(int PatientId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int status = 0;
            try
            {
                var pat = _db.Patient.Where(a => a.Id == PatientId).FirstOrDefault();
                if (pat != null)
                {
                    pat.UpdatedAt = DateTime.UtcNow;
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }
        public static string getPatientDocument(int AppointmentId, int id, int usertype)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            string status = "";
            try
            {
                if (usertype == 2)
                {
                    var pat = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                    if (pat != null)
                    {
                        var patname = _db.Doctors.Where(a => a.Id == pat.DoctorId).FirstOrDefault();
                        if (patname != null)
                        {
                            status = patname.SecondName + " " + patname.FirstName + "Document " + id;
                        }
                    }
                }
                else if (usertype == 4)
                {
                    var pat = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                    if (pat != null)
                    {
                        var patname = _db.Patient.Where(a => a.Id == pat.PatientId).FirstOrDefault();
                        status = patname.SecondName + " " + patname.FirstName + "Document " + id;
                    }
                }
                else if (usertype == 8)
                {
                    var pat = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                    if (pat != null)
                    {
                        var patname = _db.Doctors.Where(a => a.Id == pat.DoctorConsultationId).FirstOrDefault();
                        if (patname != null)
                        {
                            status = patname.SecondName + " " + patname.FirstName + "Document " + id;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                status = "";
            }
            return status;
        }
        public static int getuploaddocumentstatus(DateTime end)
        {
            int status = 0;
            if (end >= DateTime.UtcNow)
            {
                status = 1;
            }
            else
            {
                status = 0;
            }
            return status;
        }
        public static int GetDoctorMCSHID(int? DoctorId)
        {
            int MSCHID = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Doctors.Where(a => a.Id == DoctorId).FirstOrDefault();
            MSCHID = (int)docnam.MSHCID;
            return MSCHID;
        }
        public static string GetAppointmenttype(int? Id)
        {
            string Appointmenttype = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docavailbe = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == Id).FirstOrDefault();
            Appointmenttype = docavailbe.Type;
            return Appointmenttype;
        }
        public static decimal MonthTotalAmountFORAdmin(int AppoinmentId)
        {
            decimal TotalAmt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {

                var pay = _db.Payments.Where(a => a.AppoinmentId == AppoinmentId).FirstOrDefault();
                if (pay != null)
                {
                    if (pay.Amount != 0)
                    {
                        decimal discount = 0.00m;
                        discount = pay.Amount * 20 / 100;
                        TotalAmt = pay.Amount - discount;
                    }
                }

            }
            catch (Exception ex)
            {
                TotalAmt = 0;
            }
            return TotalAmt;
        }
        public static decimal DoctorMonthyaerTotalAmount(int DoctorId, int Month, int Year)
        {
            decimal TotalAmt = 0;
            decimal final = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            try
            {
                var AppList = _db.Appoinments.Where(a => a.DoctorId == DoctorId && a.MyRole == 4 && a.IsActive != 3 && a.IsActive != 0 && a.BookingDate.Month == Month && a.BookingDate.Year == Year).ToList();
                if (AppList.Count > 0)
                {
                    foreach (var item in AppList)
                    {
                        var pay = _db.Payments.Where(a => a.AppoinmentId == item.Id).FirstOrDefault();
                        if (pay.Amount != 0)
                        {
                            TotalAmt += (int)pay.Amount;
                        }
                    }
                    decimal discount = 0.00m;
                    discount = TotalAmt * 20 / 100;
                    final = TotalAmt - discount;
                }
                else
                {
                    final = 0;
                }
            }
            catch (Exception ex)
            {
                final = 0;
            }
            return final;
        }
        public static string GetByteToImage(string mybyte, string type, string path, string Name)
        {
            string Image = "";
            Image = new AES_ALGORITHM().Decrypt(mybyte);
            if (type == ".pdf")
            {
                string Completefile = path + "/TempImages/" + Name + ".pdf";
                if (!System.IO.File.Exists(Completefile))
                {
                    byte[] byteArray = Convert.FromBase64String(Image);
                    System.IO.FileStream stream = new FileStream(path + "/" + Name + ".pdf", FileMode.CreateNew);
                    System.IO.BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(byteArray, 0, byteArray.Length);
                    writer.Close();
                    Image = "/TempImages/" + Name + ".pdf";
                }
                return Image;
            }
            else if (type == ".docx")
            {
                string Completefile = path + "/TempImages/" + Name + ".docx";
                if (!System.IO.File.Exists(Completefile))
                {
                    byte[] byteArray = Convert.FromBase64String(Image);
                    System.IO.FileStream stream = new FileStream(path + "/" + Name + ".docx", FileMode.CreateNew);
                    System.IO.BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(byteArray, 0, byteArray.Length);
                    writer.Close();
                    Image = "/TempImages/" + Name + ".docx";
                }
                return Image;
            }
            else if (type == ".pptx")
            {
                string Completefile = path + "/TempImages/" + Name + ".pptx";
                if (!System.IO.File.Exists(Completefile))
                {
                    byte[] byteArray = Convert.FromBase64String(Image);
                    System.IO.FileStream stream = new FileStream(path + "/" + Name + ".pptx", FileMode.CreateNew);
                    System.IO.BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(byteArray, 0, byteArray.Length);
                    writer.Close();
                    Image = "/TempImages/" + Name + ".pptx";

                }
                return Image;
            }
            else
            {
                string Completefile = path + "/TempImages/" + Name + "." + type;
                if (!System.IO.File.Exists(Completefile))
                {
                    byte[] byteArray = Convert.FromBase64String(Image);
                    System.IO.FileStream stream = new FileStream(path + "/" + Name + "." + type, FileMode.CreateNew);
                    System.IO.BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(byteArray, 0, byteArray.Length);
                    writer.Close();
                    Image = "/TempImages/" + Name + "." + type;
                }
            }
            return Image;
        }
        public static string GetCategory(int CategoryId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            string status = "";
            try
            {
                var cat = _db.DoctorCategoryType.Where(a => a.Id == CategoryId).FirstOrDefault();
                if (cat != null)
                {
                    status = cat.Name;
                }
            }
            catch (Exception ex)
            {
                status = "";
            }
            return status;
        }
        public static int AddBannerRecord(int AppointmentId)
        {
            int status = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var checkstatus = _db.BannerStatus.FirstOrDefault();
            if (checkstatus != null)
            {
                if (checkstatus.Stataus == true)
                {
                    var appdetail = _db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault();
                    BannerRecord obj = new BannerRecord
                    {
                        DoctorId = appdetail.DoctorId,
                        PatientId = appdetail.PatientId,
                        ShowDate = DateTime.UtcNow,
                        AppointmentId = AppointmentId,
                        BannerId = 0
                    };
                    _db.BannerRecord.Add(obj);
                    status = _db.SaveChanges();
                }
            }
            return status;
        }
        public static string GetAppintmentUserNameforDoctor(int AppointmentId, int? Role, string AppCode)
        {
            string AppointmentUserName = "";
            int Id = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            if (Role == 4)//Patient
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PatientId;
                var data = _db.Patient.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (Role == 5)//PharmaRep
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().PharmaRepId;
                var app = _db.PharmaRepresentative.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = app.SecondName + " " + app.FirstName;
            }
            else if (Role == 6)//Nurse
            {
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().NurseId;
                var data = _db.Nurses.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            else if (Role == 2)//Doctortodoctor
            {
                var GetAppp = _db.Appoinments.Where(a => a.Appoinmentcode == AppCode).ToList();
                if (GetAppp.Count > 0)
                {
                    foreach (var item in GetAppp)
                    {
                        if (AppointmentUserName == "")
                        {
                            var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                            AppointmentUserName = doc.SecondName + " " + doc.FirstName;
                        }
                        else
                        {
                            var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                            AppointmentUserName = AppointmentUserName + "," + doc.SecondName + " " + doc.FirstName;
                        }
                    }
                }
            }
            else if (Role == 8)//Doctortodoctor
            {
                //var GetAppp = _db.Appoinments.Where(a => a.Appoinmentcode == AppCode).ToList();
                //if (GetAppp.Count > 0)
                //{
                //    foreach (var item in GetAppp)
                //    {
                //        if (AppointmentUserName == "")
                //        {
                //            var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                //            AppointmentUserName = doc.SecondName + " " + doc.FirstName;
                //        }
                //        else
                //        {
                //            var doc = _db.Doctors.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                //            AppointmentUserName = AppointmentUserName + "," + doc.SecondName + " " + doc.FirstName;
                //        }
                //    }
                //}
                Id = (int)_db.Appoinments.Where(a => a.Id == AppointmentId).FirstOrDefault().DoctorConsultationId;
                var data = _db.Doctors.Where(a => a.Id == Id).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
            }
            return AppointmentUserName;
        }
        public static string GetUserChatName(int? UserId, int? Role)
        {
            string AppointmentUserName = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var user = _db.Users.Where(a => a.Id == UserId).FirstOrDefault();
            if (Role == 4)//Patient
            {
                var data = _db.Patient.Where(a => a.UserId == UserId).FirstOrDefault();

                AppointmentUserName = data.SecondName + " " + data.FirstName + "(" + user.PhoneNo + ")";
            }
            else if (Role == 5)//PharmaRep
            {
                var app = _db.PharmaRepresentative.Where(a => a.UserId == UserId).FirstOrDefault();
                AppointmentUserName = app.SecondName + " " + app.FirstName + "(" + user.PhoneNo + ")";
            }
            else if (Role == 6)//Nurse
            {
                var data = _db.Nurses.Where(a => a.UserId == UserId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName + "(" + user.PhoneNo + ")";
            }
            else if (Role == 8)//Doctortodoctor
            {
                var data = _db.Doctors.Where(a => a.UserId == UserId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName + "(" + user.PhoneNo + ")";
            }
            else if (Role == 2)//Doctortodoctor
            {
                var data = _db.Doctors.Where(a => a.UserId == UserId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName + "(" + user.PhoneNo + ")";
            }
            return AppointmentUserName;
        }
        public static int GetUserCount(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.RecieverId == UserId && r.Userseen == false && r.RequestType == "admin").ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int GetUserTotalCount(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.RecieverId == UserId && r.RequestType == "admin").ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int GetUserTotalAdminCount(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.SenderId == UserId && r.RequestType == "user").ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int GetAdminCount(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.SenderId == UserId && r.Isseen == false).ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int GetAdminCountForDashboard()
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.Isseen == false).ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int checkStatusofSupport()
        {
            int Status = 0;
            DateTime date = DateTime.UtcNow;
            string dateToday = date.ToString("d");
            DayOfWeek day = DateTime.Now.DayOfWeek;
            string dayToday = day.ToString();
            if ((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
            {
                Status = 0;
            }
            else if (date.TimeOfDay.Hours >= 6 && date.TimeOfDay.Hours <= 16)
            {
                Status = 1;
            }
            return Status;
        }
        public static int ExceptionLogs()
        {
            int i = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            ChatLog obj = new ChatLog
            {

            };
            _db.ChatLog.Add(obj);
            _db.SaveChanges();
            return i;
        }
        public static string GetEnglishDoctorSpecilization(int Id)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            string Name = "";
            var cat = _db.SpecilizationCategory.Where(r => r.Id == Id).FirstOrDefault();
            if (cat != null)
            {
                Name = cat.Name;
            }
            return Name;
        }
        public static string GetHungryDoctorSpecilization(int Id)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            string Name = "";
            var cat = _db.SpecilizationCategory.Where(r => r.Id == Id).FirstOrDefault();
            if (cat != null)
            {
                Name = cat.HunName;
            }
            return Name;
        }
        public static int SendSecondEmailtoUser()
        {
            int i = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var GetUser = _db.Users.Where(r => r.Isemail != 1 && r.CreatedAt.Date == DateTime.UtcNow.AddHours(-24) && r.UserType == 2).ToList();
            if (GetUser != null)
            {
                foreach (var item in GetUser)
                {
                    item.Isemail = 1;
                    _db.SaveChanges();
                    string UserIds = item.Id.ToString();
                    string Name = GetDocname(item.Id);
                    AppCommonLogic.SendEmailToDoctorAgain(item.Email, Name, new AES_ALGORITHM().Encrypt(UserIds), 2);
                }
            }
            return i;
        }
        private static string GetDocname(int? UserId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Doctors.Where(a => a.UserId == UserId).FirstOrDefault();
            Name = docnam.SecondName + " " + docnam.FirstName;
            return Name;
        }
        public static int GetAppointmentCount(int? SlotId)
        {
            int cnt = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            cnt = _db.Appoinments.Where(a => a.DocSlotId == SlotId && a.IsActive != 0).Count();
            return cnt;
        }
        public static string CovertDatetoHungaryTime(DateTime StartDate)
        {
            var timeUtc = StartDate;
            var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, centraltimezone);
            string formattedDate = today.ToString("yyyy-MM-dd HH:mm");
            return formattedDate;
        }
        public string getRandColor()
        {
            Random rnd = new Random();
            string hexOutput = String.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }
        public static string LogEmail(string Path = "", string stage = "")
        {
            string path = System.IO.Path.Combine(Path, @"Clinic");
            string Completefile = path + "/" + "Email-" + DateTime.Now.ToShortDateString() + "" + "." + "json";
            if (!System.IO.File.Exists(Completefile))
            {
                System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(stage) + Environment.NewLine + "LogTime:- " + DateTime.UtcNow + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                System.IO.File.AppendAllText(Completefile, JsonConvert.SerializeObject(stage) + Environment.NewLine + "LogTime:- " + DateTime.UtcNow + Environment.NewLine + Environment.NewLine);
            }
            return path;
        }
        public static int InsertUserLog(int UserId, string ActivityName, string Status = "", string Msg = "")
        {
            int i = 0;
            try
            {
                MyClickDoctorV4 _db = new MyClickDoctorV4();
                IssueDescription issue = new IssueDescription()
                {
                    UserId = UserId,
                    ActivityName = ActivityName,
                    Created = DateTime.UtcNow,
                    Status = Status,
                    ErrorMessage = Msg
                };
                _db.IssueDescription.Add(issue);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return i;
        }
        public static string UserConvertedUserId(string AppId)
        {
            string Appointmentid = "";
            Appointmentid = new AES_ALGORITHM().Encrypt(AppId);
            return Appointmentid;
        }
        public static string GetDoctorSubject(int DocSlotId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.DoctorAvailibiltyforAppointment.Where(a => a.Id == DocSlotId).FirstOrDefault();
            if (docnam != null)
            {
                Name = docnam.Subject;
            }
            return Name;
        }
        public static int UpdatemeetingFifteenmintusstatus(string meetingId)
        {
            int i = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var list = _db.Appoinments.Where(a => a.Appoinmentcode == meetingId).ToList();
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.IsFifteenEmail = true;
                }
                _db.SaveChanges();
            }
            return i;
        }
        public static int Updatemeetinghourstatus(string meetingId)
        {
            int i = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var list = _db.Appoinments.Where(a => a.Appoinmentcode == meetingId).ToList();
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.IshourEmail = true;
                }
                _db.SaveChanges();
            }
            return i;
        }
        public static int Updatemeetingtwentyfourstatus(string meetingId)
        {
            int i = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var list = _db.Appoinments.Where(a => a.Appoinmentcode == meetingId).ToList();
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.IsdayEmail = true;
                }
                _db.SaveChanges();
            }
            return i;
        }
        public static string GetDoctorChatName(int? SenderId, int? RecieverId, int UserId)
        {
            string AppointmentUserName = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();

            if (SenderId != UserId)
            {
                var data = _db.Doctors.Where(a => a.UserId == SenderId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
                return AppointmentUserName;
            }
            else if (RecieverId != UserId)
            {
                var data = _db.Doctors.Where(a => a.UserId == RecieverId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
                return AppointmentUserName;
            }

            return "";
        }
        public static string GetPatientChatName(int? SenderId, int? RecieverId, int UserId)
        {
            string AppointmentUserName = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();

            if (SenderId == UserId)
            {
                var data = _db.Patient.Where(a => a.UserId == RecieverId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
                return AppointmentUserName;
            }
            else
            {
                var data = _db.Patient.Where(a => a.UserId == SenderId).FirstOrDefault();
                AppointmentUserName = data.SecondName + " " + data.FirstName;
                return AppointmentUserName;
            }
        }
        public static int GetPatientId(int? SenderId, int? RecieverId, int UserId)
        {
            int PatientId = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();

            if (SenderId == UserId)
            {
                var data = _db.Patient.Where(a => a.UserId == RecieverId).FirstOrDefault();
                PatientId = data.Id;
                return PatientId;
            }
            else
            {
                var data = _db.Patient.Where(a => a.UserId == SenderId).FirstOrDefault();
                PatientId = data.Id;
                return PatientId;
            }
        }
        public static int GetDoctorSeenCount(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.SenderId == UserId && r.Isseen == false && r.Message!="" && r.Message!=null).ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int GetsenderId(int? SenderId, int UserId, int? RecieverId)
        {
            int Id = 0;
            if (SenderId == UserId)
            {
                Id = (int)SenderId;
            }
            else
            {
                Id = (int)RecieverId;
            }
            return Id;
        }
        public static int GetRecieverId(int? RecieverId, int UserId, int? SenderId)
        {
            int Id = 0;
            if (RecieverId == UserId)
            {
                Id = (int)SenderId;
            }
            else
            {
                Id = (int)RecieverId;
            }
            return Id;
        }
        public static string GetRequesttype(int? SenderId, int UserId)
        {
            string Request = "";
            if (SenderId == UserId)
            {
                Request = "Send";
            }
            else
            {
                Request = "Recieve";
            }
            return Request;
        }
        public static int GetDoctorChatHistoryCount(string connectionId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.ConnectionId == connectionId).ToList();
            if (cht.Count > 0)
            {
                count = cht.Count();
            }
            return count;
        }
        public static int CheckTimeawailability(int DocId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            return _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.FromTime >= DateTime.UtcNow && a.DocId == DocId && a.DoctorType == "KeyOpinionLeader").Count();
        }
        public static string GetDoctorchatName(int? DoctorId)
        {
            string Name = "";
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var docnam = _db.Doctors.Where(a => a.UserId == DoctorId).FirstOrDefault();
            Name = docnam.SecondName + " " + docnam.FirstName;
            return Name;
        }
        public static int CheckMonthAVAILABLEaPPOINTMENT(int DocId)
        {
            int res = 0;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var doctor = _db.Doctors.Where(a => a.Id == DocId).FirstOrDefault();
            if (doctor.MaxCallCount != null)
            {
                var DocTimeSlot = _db.DoctorAvailibiltyforAppointment.Where(a => a.FromTime.Month == DateTime.UtcNow.Month && a.Status == 0 && a.DocId == doctor.Id).ToList();
                if (DocTimeSlot.Count > 0)
                {
                    foreach (var item in DocTimeSlot)
                    {
                        int app = _db.Appoinments.Where(a => a.DocSlotId == item.Id && (a.IsActive == 2 || a.IsActive == 1)).Count();
                        if (app > 0)
                        {
                            res++;
                        }
                    }
                }
                //  int app= _db.Appoinments.Where(a => a.DoctorId == DocId && a.StartDate.Month == DateTime.UtcNow.Month && (a.IsActive == 2 || a.IsActive == 1)).Count();
                if (res > (int)doctor.MaxCallCount)
                {
                    res = 0;
                }
                else
                {
                    res = (int)doctor.MaxCallCount - res;
                }
            }
            return res;
        }
        public static int GetDoctorNotificationstatus(int UserId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            int count = 0;
            var cht = _db.Chat.Where(r => r.SenderId == UserId && r.Isseen == false && r.Message!=null && r.Message!="").ToList();
            if (cht.Count > 0)
            {
                foreach (var not in cht)
                {
                    TimeSpan Total = DateTime.UtcNow.TimeOfDay - Convert.ToDateTime(not.CreatedAt).TimeOfDay;
                    int Time = Math.Abs((int)Total.TotalSeconds);
                    if (Time <= 2)
                    {
                        count = 1;
                        return count;
                    }
                }

            }
            return count;
        }
        public static bool CheckDoctorRemainingAailability(int DocId)
        {
            int res = 0;
            bool response = false;
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            var doctor = _db.Doctors.Where(a => a.Id == DocId).FirstOrDefault();
            if (doctor.MaxCallCount != null)
            {
                var DocTimeSlot = _db.DoctorAvailibiltyforAppointment.Where(a => a.FromTime.Month == DateTime.UtcNow.Month && a.Status == 0 && a.DocId == doctor.Id).ToList();
                if (DocTimeSlot.Count > 0)
                {
                    foreach (var item in DocTimeSlot)
                    {
                        int app = _db.Appoinments.Where(a => a.DocSlotId == item.Id && (a.IsActive == 2 || a.IsActive == 1)).Count();
                        if (app > 0)
                        {
                            res++;
                        }
                    }
                    if (res >= (int)doctor.MaxCallCount)
                    {
                        response = true;
                    }
                    else
                    {
                        res = (int)doctor.MaxCallCount - res;
                    }
                }
            }
            return response;
        }
        public static int CheckTimeawailabilityforpatient(int DocId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            return _db.DoctorAvailibiltyforAppointment.Where(a => a.Status == 0 && a.FromTime >= DateTime.UtcNow && a.DocId == DocId && a.SlotType == "patient").Count();
        }
        public static bool GetDoctorAllowPatient(int DocId)
        {
            MyClickDoctorV4 _db = new MyClickDoctorV4();
            bool status = false;
            List<Doctorspecilization> getlist = new List<Doctorspecilization>();
            try
            {
                getlist = _db.Doctorspecilization.Where(a => a.DocId == DocId && a.Status == 1 && a.Name != null).ToList();
                if (getlist.Count() > 0)
                {
                    foreach (var item in getlist)
                    {
                        var category = _db.SpecilizationCategory.Where(a => a.Id == item.CatId).FirstOrDefault();
                        if (category.AllowPatient == true)
                        {
                            return status = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return status;
            }
            return status;
        }
   
    }
}
