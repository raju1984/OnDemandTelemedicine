using MyClickDoctorBE.DatabaseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
    public class ApiResponse
    {
        public string msg { get; set; }
        public int code { get; set; }
        public string path { get; set; }
        public string UserId { get; set; }
        public string EncryptedId { get; set; }
    }
    public class ptapp : ApiResponse
    {
        public int AppointmentId { get; set; }
    }
    public class doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime? ContarctEndDate { get; set; }
        [Required]
        public int YearsOfExperiecne { get; set; }
        [Required]
        public string MedicalRegistrationNo { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string ContractualDoctorsCompany { get; set; }
        public string ShortIntroduction { get; set; }
        public int? PriceId { get; set; }
        public int? UserId { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNo { get; set; }

        public string Password { get; set; }

        public int UserType { get; set; }
        public int? Status { get; set; }
        public string Token { get; set; }
        public string Gander { get; set; }
        public DateTime DOB { get; set; }
        public List<Specializion> Expert { get; set; }
        public List<DoctorAwalaibility> DaysWeek { get; set; }
        public List<SpecialCategory> DocSpeciality { get; set; }
        public List<experinace> DocExperiance { get; set; }
        public List<Award> DocAward { get; set; }
        public List<DocCliMap> DocClinic { get; set; }
        public string AboutMe { get; set; }
        public string RegistrationNo { get; set; }
        public int Year { get; set; }
        public int BussinessStatus { get; set; }
        public string AppointmentTitle { get; set; }
        public int AppointmentId { get; set; }
        public DateTime LastVisitDate { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? DoctorConslutationCount { get; set; }
        public bool? DoctorConsultation { get; set; }
        public string PatientName { get; set; }
        public string Address { get; set; }
        public string PatImage { get; set; }
        public int InvoiceStatus { get; set; }
        public string Invoice { get; set; }
        public int Awailability { get; set; }
        public int DoctorType { get; set; }
        public string DoctorTypeName { get; set; }
        public List<Bookfreeavailbility> DoctorAvailability { get; set; }
        public string Price { get; set; }
        public int MaxCount { get; set; }
        public string RefundStatus { get; set; }
        public string ssnno { get; set; }
        public string Name { get; set; }
        public string HungName { get; set; }
        public string CountryCode { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceAddress { get; set; }
        public string TaxNumber { get; set; }
        public int MSHCID { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
        public int CheckCount { get; set; }
        public string DocCategory { get; set; }
        public bool Agreement { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? ToTime { get; set; }
        public DateTime? FromTime { get; set; }
        public string AppointmentType { get; set; }
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public string Subject { get; set; }
        public int AppointmentCount { get; set; }
        public string Date { get; set; }
        public bool IsValidated { get; set; }
        public string AppointmentCode { get; set; }
        public int Booked { get; set; }
        public int? MaxCallCount { get; set; }
        public int awilablecallcountmonth { get; set; }
        public bool bookingawailibility { get; set; }
        public string Project { get; set; }
        public bool? AllowPatient { get; set; }
    }
    public class Login : ApiResponse
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public int Usertype { get; set; }
        public string Phone { get; set; }
        public int PharmaCompId { get; set; }
        public int Id { get; set; }
        public int BussinessStatus { get; set; }
        public string UserIds { get; set; }
        public string Name { get; set; }
        public string ProfilePhoto { get; set; }
        public int SMS { get; set; }
        public bool AuthStatus { get; set; }
        public string Doctortype { get; set; }
        public bool IsValidated { get; set; }
        public string DoctorUserId { get; set; }
        public bool AllowPatient { get; set; }
        public string Project { get; set; }
    }
    public class PatientReg
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortIntroduction { get; set; }
        public DateTime Dob { get; set; }
        public string City { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public string Zipcode { get; set; }
        public string Gender { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ProfileUrl { get; set; }
        public int BussinessStatus { get; set; }
        public bool GDPR { get; set; }
        public string CountryCode { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceAddress { get; set; }
        public string TaxNumber { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
    }
    public class pharma
    {

        public string Email { get; set; }

        public string PhoneNo { get; set; }

        public string Password { get; set; }

        public int UserType { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }
        public int Id { get; set; }
        public string ComanyName { get; set; }
        public string ShortDescription { get; set; }
        public string RegNumber { get; set; }
        public string Address { get; set; }
        public int? UserId { get; set; }
        public int ServicesHours { get; set; }
        public string ProfilePic { get; set; }
        public List<PharmaDoc> Pharmadocument { get; set; }
        public int DoctorCount { get; set; }
        public int TodayVisit { get; set; }
        public int WeekVisit { get; set; }
        public List<BookPharmaApp> TodayAppList { get; set; }
        public List<BookPharmaApp> WeekAppList { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TerityDefination { get; set; }
        public string ContactPerson { get; set; }
        public List<AssignPharma> GetAssignPharma { get; set; }
        public int CompanyId { get; set; }
    }
    public class PharmaDoc
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
    public class UpdateStatus : ApiResponse
    {
        [Required]
        public int UserId { get; set; }

        public int Status { get; set; }
        public int BussinessStatus { get; set; }
        public bool IsValidated { get; set; }
    }
    public class ChangePass
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public int UserId { get; set; }
    }
    public class Admin : pharma
    {
        public string MSHCID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContarctEndDate { get; set; }
    }
    public class ForgetPass : ApiResponse
    {
        public string Email { get; set; }
    }
    public class Address
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
    public class AppDoctor
    {
        public int DoctorId { get; set; }
    }
    public class BookPharmaApp
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BookingDate { get; set; }
        // [Required]
        public string AppointmentTitle { get; set; }
        // [Required]
        public int DoctorId { get; set; }
        // [Required]
        public int PharmaRepId { get; set; }
        public string TimeZone { get; set; }
        public int Duration { get; set; }
        public int PharmaCompanyId { get; set; }
        public string DoctorName { get; set; }
        public string RepName { get; set; }
        public string AppointmentId { get; set; }
        public string Attachment { get; set; }
        public string DocImage { get; set; }
        public int DocuId { get; set; }
        public string Date { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public List<Specializion> Expert { get; set; }
        public List<DoctorAvailibiltyforAppointment> DocFreeSlot { get; set; }
        //   [Required]
        public int SlotId { get; set; }
        public string Appointmenttype { get; set; }
        public int Status { get; set; }
        public string UserId { get; set; }
        public int Type { get; set; }
        public string PharmaRepImage { get; set; }
        public int MeetingJoinStatus { get; set; }
        public int CancelStatus { get; set; }
        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public string AppoinmentId { get; set; }
        public string TransactionId { get; set; }
        public string Remark { get; set; }
        public List<DocCliMap> DocClinic { get; set; }
        public string PatImage { get; set; }
        public string PatName { get; set; }
        public int NurseId { get; set; }
        public string NurseImage { get; set; }
        public string NurseName { get; set; }
        public int Role { get; set; }
        public int AppointmentUserId { get; set; }
        public string AppointmentUserName { get; set; }
        public string AppointmentUserImage { get; set; }
        public int AppointmentSUserId { get; set; }
        public string AppointmentSUserName { get; set; }
        public string AppointmentSUserImage { get; set; }
        public int DoctorConsultationId { get; set; }
        public string Description { get; set; }
        public string DaysWeek { get; set; }
        public int UserRole { get; set; }
        public int MyRole { get; set; }
        public List<AppDoctor> DoctorsId { get; set; }
        public int NurseSuggestion { get; set; }
        public int Ispopup { get; set; }
        public int RefundStatus { get; set; }
        public string MeetingId { get; set; }
        public int PaymentStatus { get; set; }
        public int UploadStatus { get; set; }
        public string Colorcode { get; set; }
        public string AppId { get; set; }
        public int ConsultationType { get; set; }
    }
    public class Specializion
    {
        public int Id { get; set; }
        public int YearOfDiploma { get; set; }
        public string NameofDegree { get; set; }
        public string University { get; set; }
        public string Specialization { get; set; }
        public string HungName { get; set; }
        public string Remark { get; set; }
        public int UserId { get; set; }
        public int CatId { get; set; }
    }
    public class DoctorAwalaibility
    {
        public int Id { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string DayofWeek { get; set; }
        public int? UserId { get; set; }
    }
    public class PharmaRep
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int PharmaCompanyId { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? ContractExpiryDate { get; set; }
        public string AboutUs { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TerityDefination { get; set; }
        public string CompanyName { get; set; }
        public string ShortDescription { get; set; }
        public string RegNumber { get; set; }
        public string CompleteAddress { get; set; }
        public string ServicesHours { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public string ProfileUrl { get; set; }
        public List<AssignDoctors> AsignCat { get; set; }
        public int BussinessStatus { get; set; }
        public string CountryCode { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
    }
    public class SpecialCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CatId { get; set; }
        public string FirstName { get; set; }
    }
    public class GetSpeCat : ApiResponse
    {
        public int CallCount { get; set; }
        public int DoctorCount { get; set; }
        public int WizardStatus { get; set; }
        public List<doctor> SpecializationCategory { get; set; }

    }
    public class DataResponse : ApiResponse
    {
        public int DoctorCount { get; set; }
        public List<doctor> getList { get; set; }

    }
    public class DoctorProfile : ApiResponse
    {
        public doctor DocProfile { get; set; }
    }
    public class PharmaProfile : ApiResponse
    {
        public pharma GetPhrma { get; set; }
    }
    public class PharmaApp : ApiResponse
    {
        public pharma Dashboard { get; set; }
    }
    public class GetApp : ApiResponse
    {
        public List<BookPharmaApp> Appdata { get; set; }
    }
    public class GetPharma : ApiResponse
    {
        public List<pharma> getList { get; set; }
    }
    public class Upload : ApiResponse
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
    public class MultiUpload : ApiResponse
    {
        public string[] Path { get; set; }
    }
    public class Profile : ApiResponse
    {
        public int UserId { get; set; }
        public string ShortDescription { get; set; }
        public string ImagePath { get; set; }
        public string DocPath { get; set; }
    }
    public class meeting : ApiResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int UserType { get; set; }
        public string AppId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentId { get; set; }
    }
    public class experinace
    {
        public int Id { get; set; }
        public string WorkplaceTitle { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AddressLine { get; set; }
    }
    public class Award
    {
        public int Id { get; set; }
        public string AwardTitle { get; set; }
        public int? Year { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
    }
    public class DocCliMap
    {
        public int Id { get; set; }
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
        public int? DocumnetId { get; set; }
        public int? UserId { get; set; }
        public List<Upload> Image { get; set; }
    }
    public class invite
    {
        public int Id { get; set; }
        public int SentTo { get; set; }
        public int? FromUserId { get; set; }
        public string Email { get; set; }
        public string MessageWithInvitation { get; set; }
    }
    public class Bokk
    {
        public string Title { get; set; }
        public string Date { get; set; }

    }
    public class pattern
    {
        public string Date { get; set; }
        public string Title { get; set; }
    }
    public class NewDoctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specility { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public int ReqId { get; set; }
        public int CompanyId { get; set; }
    }
    public class Bookfreeavailbility
    {
        public int Id { get; set; }
        public int DocId { get; set; }
        public DateTime ToTime { get; set; }
        public DateTime FromTime { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string SlotType { get; set; }
        public bool? Docavailable { get; set; }
        public bool? Nurseavailable { get; set; }
        public bool? Patientavailable { get; set; }
        public bool? Pharmaavailable { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; }
        public int MaxCount { get; set; }
        public string Subject { get; set; }
        public int ToDocId { get; set; }
        public string ConnectionId { get; set; }
    }
    public class NurseAvailable
    {
        public int Id { get; set; }
        public int NurseId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    }
    public class freeslot : ApiResponse
    {
        public List<Bookfreeavailbility> getList { get; set; }
        public int DoctorCallCount { get; set; }
    }
    public class invitedoclist : ApiResponse
    {
        public List<NewDoctor> GetInviteDocList { get; set; }
    }
    public class DashBoard
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string MSHCID { get; set; }
        public int UserId { get; set; }
        public int PharmaCount { get; set; }
        public int PharmaCompanyCount { get; set; }
        public int DocCount { get; set; }
        public int PatientCount { get; set; }
        public int AppCount { get; set; }
        public int ReqCount { get; set; }
        public int NurseCount { get; set; }
        public List<doctor> DocList { get; set; }
        public List<Patient> PatList { get; set; }
        // public List<PharmaRep> PhaList { get; set; }
        public List<BookPharmaApp> AppList { get; set; }
        public List<Notify> NotList { get; set; }
        public List<AssignPharma> PhaList { get; set; }
    }
    public class AdminDashBoard : ApiResponse
    {
        public List<DashBoard> data { get; set; }
    }
    public class Pateint : ApiResponse
    {
        public List<PatientReg> getList { get; set; }
    }
    public class Notify
    {
        public int Id { get; set; }
        public string ReqId { get; set; }
        public int DoctorId { get; set; }
        public int PharmaRepId { get; set; }
        public string Message { get; set; }
        public int AdminId { get; set; }
        public int? ParentId { get; set; }
        public string DoctorName { get; set; }
        public string PharmaRepName { get; set; }
        public string AdminName { get; set; }
        public int Duration { get; set; }
        public int NotCount { get; set; }
        public int Isseen { get; set; }
        public int Status { get; set; }
        public string AdminComment { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public string UserName { get; set; }
        public string MeetingId { get; set; }
        public string UserIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string activeuser { get; set; }
        public string notactiveuser { get; set; }
        public List<NotifyIssue> DescriptionList { get; set; }
    }
    public class NotReq : ApiResponse
    {
        public List<Notify> GetList { get; set; }
    }
    public class AssignPharma
    {
        public int RepId { get; set; }
        public int CompId { get; set; }
        public string CompanyName { get; set; }
        public string RepName { get; set; }
    }
    public class PharmaRepresentativedetail : ApiResponse
    {
        public List<PharmaRep> GetList { get; set; }
    }
    public class AssignDoctors
    {
        public int RepId { get; set; }
        public int DocId { get; set; }
        public int CatId { get; set; }
        public string CompanyName { get; set; }
        public string RepName { get; set; }
        public string FirstName { get; set; }
    }
    public class AssignDoctopharm : ApiResponse
    {
        public List<AssignDoctors> AssignDoc { get; set; }
        public int RepId { get; set; }
    }
    public class PharmaRepresentativedetails : ApiResponse
    {
        public PharmaRep GetList { get; set; }

    }
    public class AdminAPPOINT : ApiResponse
    {
        public List<BookPharmaApp> AppList { get; set; }
    }
    public class DocSpecilty
    {
        public List<SpecialCategory> DocSpeciality { get; set; }
        public int UserId { get; set; }
    }
    public class pharmadetail : ApiResponse
    {
        public pharma data { get; set; }

    }
    public class GetSpecility : ApiResponse
    {
        public List<Specializion> Specility { get; set; }

    }
    public class DocClinics
    {
        public List<DocCliMap> DocClinic { get; set; }
        public int UserId { get; set; }
    }
    public class DocAwailbility
    {
        public List<DoctorAwalaibility> DaysWeek { get; set; }
        public int UserId { get; set; }
    }
    public class DoctorDiploamspecilization
    {
        public List<Specializion> Expert { get; set; }
        public int UserId { get; set; }
    }
    public class WorkPlace
    {
        public List<experinace> DocExperiance { get; set; }
        public int UserId { get; set; }
    }
    public class awards
    {
        public List<Award> DocAward { get; set; }
        public int UserId { get; set; }
    }
    public class DocDashboard
    {
        public int TotalPatient { get; set; }
        public int TodayPatient { get; set; }
        public int Appointments { get; set; }
        public int TotalPharma { get; set; }
        public string Name { get; set; }
        public List<BookPharmaApp> TodayAppList { get; set; }
        public List<BookPharmaApp> WeekAppList { get; set; }
    }
    public class GetAdminDash : ApiResponse
    {
        public DocDashboard getdata { get; set; }
    }
    public class ProfileRequest
    {
        public int UserId { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
    }
    public class AdminAPPOINTDetail : ApiResponse
    {
        public BookPharmaApp data { get; set; }
    }
    public class PhrmDoc
    {
        public int CatId { get; set; }
        public doctor doc { get; set; }
        public int DocId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
    }
    public class Notifiction : ApiResponse
    {
        public List<Notify> getlist { get; set; }
        public int ChatCount { get; set; }
    }
    public class UpdatePass
    {
        public string Password { get; set; }
        public string UserId { get; set; }
    }
    public class Notificationstatus : ApiResponse
    {
        public string MeetingId { get; set; }
        public int Id { get; set; }

        public int Status { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public int UserId { get; set; }
    }
    public class NotifyIssue
    {
        public int? IssueId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
    public class Updatetime
    {
        public int UserId { get; set; }
        public int Type { get; set; }
    }
    public class chckusr
    {
        public string UserId { get; set; }
    }
    public class SendNotify
    {
        public notifydata notification { get; set; }
        public string to { get; set; }
    }
    public class notifydata
    {
        public string title { get; set; }
        public string body { get; set; }
    }
    public class GetProfile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Profile { get; set; }
        public BookPharmaApp Appointment { get; set; }
    }
    public class AncPro : ApiResponse
    {
        public GetProfile Getdata { get; set; }
    }
    public class NotifyToken
    {
        // public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
    }
    public class Docslot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
    }
    public class Getslotlist : ApiResponse
    {
        public List<Docslot> getlist { get; set; }
    }
    public class PatientProfile : ApiResponse
    {
        public PatientReg detail { get; set; }
    }
    public class Feedback
    {
        public int Id { get; set; }
        public int DocId { get; set; }
        public int FromUserId { get; set; }
        public int Type { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public DateTime Created { get; set; }
        public int ParentId { get; set; }
        public string DocName { get; set; }
        public string DocImage { get; set; }
        public string MeetingId { get; set; }
        public List<Feedback> reply { get; set; }
        public int AppointmentId { get; set; }
    }
    public class getreview : ApiResponse
    {
        public List<Feedback> getlist { get; set; }
    }
    public class PatMobidy
    {
        public int Id { get; set; }
        public string anamnesis { get; set; }
        public string diagnosis { get; set; }
        public string therapy { get; set; }
        public int AppointmentId { get; set; }
        public DateTime Created { get; set; }
        public List<Specializion> Expert { get; set; }
        public List<DocCliMap> DocClinic { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string MeetingId { get; set; }
    }
    public class getpatmobidy : ApiResponse
    {
        public PatMobidy getlist { get; set; }
    }
    public class getreviewdetail : ApiResponse
    {
        public Feedback getlist { get; set; }
    }
    public class PatientDashBoard
    {
        public int Appointments { get; set; }
        public int AllAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int TotalDoctor { get; set; }
        public int TotalPatient { get; set; }
        public int MedicalReport { get; set; }
        public string Name { get; set; }
        public int WizardStatus { get; set; }
        public List<BookPharmaApp> TodayAppList { get; set; }
        public List<BookPharmaApp> WeekAppList { get; set; }
    }
    public class GetPatDash : ApiResponse
    {
        public PatientDashBoard GetData { get; set; }
    }
    public class Package
    {
        public int Id { get; set; }
        public decimal? AppointmentRate { get; set; }
        public int? CurrencyId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int? Duration { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Date { get; set; }
        public string DoctorName { get; set; }
    }
    public class GetPackage : ApiResponse
    {
        public List<Package> GetList { get; set; }
    }
    public class singlePackage : ApiResponse
    {
        public Package GetList { get; set; }
    }
    public class paystatus : ApiResponse
    {
        public string TransactionId { get; set; }
        public int Status { get; set; }
    }
    public class nurse
    {
        public int Id { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int IsAvailable { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortDescription { get; set; }
        public int UserId { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ProfileUrl { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int BussinessStatus { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
    }
    public class getNurse : ApiResponse
    {
        public nurse getdata { get; set; }
    }
    public class getNurselist : ApiResponse
    {
        public List<nurse> getlist { get; set; }
    }
    public class DoctorProfileRequest : ApiResponse
    {
        public List<DoctorRequest> RequestList { get; set; }
        public int RequestCount { get; set; }
    }
    public class doctorProfile
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int YearsOfExperiecne { get; set; }
        public string MedicalRegistrationNo { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
        public string StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string ShortIntroduction { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Gander { get; set; }
        public DateTime DOB { get; set; }
        public string AboutMe { get; set; }
        public string Country { get; set; }
        public string OldFirstName { get; set; }
        public string OldSecondName { get; set; }
        public int OldYearsOfExperiecne { get; set; }
        public string OldMedicalRegistrationNo { get; set; }
        public string OldPhotoUrl { get; set; }
        public string OldCity { get; set; }
        public string OldStreetNumber { get; set; }
        public string OldZipcode { get; set; }
        public string OldShortIntroduction { get; set; }
        public string OldEmail { get; set; }
        public string OldPhoneNo { get; set; }
        public string OldGander { get; set; }
        public DateTime OldDOB { get; set; }
        public string OldAboutMe { get; set; }
        public string OldCountry { get; set; }
        public int DoctorType { get; set; }
        public int Status { get; set; }
        public string CountryCode { get; set; }
        public int MSHCID { get; set; }
    }
    public class DoctorProfileRequestDetail : ApiResponse
    {
        public doctorProfile Requestdata { get; set; }
    }
    public class UpdateDoctorPrfileStatus
    {
        public int Status { get; set; }
        public int UserId { get; set; }
    }
    public class NurseAvalibility
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DaysOfWeek { get; set; }
    }
    public class NurseSlot : ApiResponse
    {
        public List<NurseAvailable> getList { get; set; }
    }
    public class Nurseslot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }
    }
    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
    public class UpdateDoctorCounsultation
    {
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }
    }
    public class NurseProfileRequest
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortDescription { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ProfileUrl { get; set; }
        public string MobileNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

    }
    public class PharmaProfileRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AboutUs { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TerityDefination { get; set; }
        public string CompanyName { get; set; }
        public string ShortDescription { get; set; }
        public string RegNumber { get; set; }
        public string CompleteAddress { get; set; }
        public string ServicesHours { get; set; }
        public int? Status { get; set; }
        public string ProfileUrl { get; set; }
        public string CountryCode { get; set; }

    }
    public class PharmaProfileRequestDetail
    {
        public int UserId { get; set; }
        public string AboutUs { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TerityDefination { get; set; }
        public string CompanyName { get; set; }
        public string ShortDescription { get; set; }
        public string RegNumber { get; set; }
        public string CompleteAddress { get; set; }
        public string ServicesHours { get; set; }
        public string ProfileUrl { get; set; }
        public string OldAboutUs { get; set; }
        public string OldFirstName { get; set; }
        public string OldSecondName { get; set; }
        public string OldTerityDefination { get; set; }
        public string OldCompanyName { get; set; }
        public string OldShortDescription { get; set; }
        public string OldRegNumber { get; set; }
        public string OldCompleteAddress { get; set; }
        public string OldServicesHours { get; set; }
        public string OldProfileUrl { get; set; }
        public int? Status { get; set; }
    }
    public class getPhrmadetail : ApiResponse
    {
        public PharmaProfileRequestDetail Requestdata { get; set; }
    }
    public class Getnurserequest : ApiResponse
    {
        public List<NurseProfileRequest> RequestList { get; set; }
        public int RequestCount { get; set; }
    }
    public class NurseProfile
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortDescription { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ProfileUrl { get; set; }
        public string MobileNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string OldFirstName { get; set; }
        public string OldSecondName { get; set; }
        public string OldShortDescription { get; set; }
        public string OldGender { get; set; }
        public string OldAddress { get; set; }
        public string OldProfileUrl { get; set; }
        public string OldMobileNo { get; set; }
        public string OldZipCode { get; set; }
        public string OldCity { get; set; }
        public string OldCountry { get; set; }
        public int Status { get; set; }
    }
    public class NurseProfileRequestDetail : ApiResponse
    {
        public NurseProfile Requestdata { get; set; }
    }
    public class DocConsult : ApiResponse
    {
        public List<doctor> ConsultList { get; set; }
        public int Amount { get; set; }
    }
    public class DoctorPayActivity
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public int Amount { get; set; }
        public string Paid { get; set; }
        public int MSHCID { get; set; }
        public int Duration { get; set; }
    }
    public class payhistory : ApiResponse
    {
        public List<DoctorPayActivity> consultList { get; set; }
        public int Amount { get; set; }
    }
    public class DoctorRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int UserType { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string ProfileURL { get; set; }
    }
    public class faqs
    {
        public int Id { get; set; }
        public string Ques { get; set; }
        public string Ans { get; set; }
        public int? UserType { get; set; }
    }
    public class getfaq : ApiResponse
    {
        public List<faqs> getlist { get; set; }
    }
    public class PharmaCount
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
    public class phmaxcount : ApiResponse
    {
        public PharmaCount Appdata { get; set; }
    }
    public class nurseSuggetion
    {
        public int AppointmentId { get; set; }
        public string MeetingId { get; set; }
        public int Status { get; set; }
        public string PatientName { get; set; }
        public string NurseName { get; set; }
        public string Symptoms { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class nursedoctorsuggestion : ApiResponse
    {
        public List<nurseSuggetion> list { get; set; }
    }
    public class Updatemeeting : ApiResponse
    {
        public int Usertype { get; set; }
        public int AppointmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int PatientId { get; set; }
        public int NurseId { get; set; }
        public int SlotId { get; set; }
    }
    public class reminder : ApiResponse
    {
        public List<BookPharmaApp> TodayAppList { get; set; }
    }
    public class Updatepopup : ApiResponse
    {
        public int usertype { get; set; }
        public int DocId { get; set; }
        public int Status { get; set; }
        public int Id { get; set; }
    }
    public class mkrefund : ApiResponse
    {
        public int Id { get; set; }
    }
    public class AddSpecility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HungName { get; set; }
    }
    public class MainpageBanner
    {
        public int Id { get; set; }
        public string BannerHeaderName { get; set; }
        public string BannerHeaderPoint { get; set; }
        public string BannerHeaderNameHu { get; set; }
        public string BannerHeaderPointHu { get; set; }
        public int sno { get; set; }
        public int imgno { get; set; }
    }
    public class Addmangaebanner
    {
        public List<MainpageBanner> Mainpage { get; set; }
    }
    public class Addmangaebannerpoint
    {
        public List<MainpagePoint> Mainpage { get; set; }
    }
    public class MainpagePoint
    {
        public int Id { get; set; }
        public string Points { get; set; }
        public string PointsHu { get; set; }
    }
    public class welcome : ApiResponse
    {
        public List<MainpageBanner> bannerlist { get; set; }
        public List<MainpagePoint> pointlist { get; set; }
        public string BannerHeaderName { get; set; }
        public string BannerHeaderPoint { get; set; }
    }
    public class mnpageedit : ApiResponse
    {
        public List<Mainpage> bannerlist { get; set; }
        public List<Mainpage> pointlist { get; set; }
    }
    public class about
    {
        public int Id { get; set; }
        public string Points { get; set; }
        public string PointsHu { get; set; }
    }
    public class aboutus : ApiResponse
    {
        public List<about> list { get; set; }
    }
    public class addIssue
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PopupStatus { get; set; }
        public int Usertype { get; set; }
        public int UserId { get; set; }
    }
    public class Getissue : ApiResponse
    {
        public addIssue data { get; set; }
    }
    public class appdoclist
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string filetype { get; set; }
        public int DocId { get; set; }
    }
    public class appdoc : ApiResponse
    {
        public List<appdoclist> list { get; set; }
    }
    public class codes
    {
        public string CountryCode { get; set; }
    }
    public class codess : ApiResponse
    {
        public codes data { get; set; }
    }
    public class baronid
    {
        public string PixelId { get; set; }
    }
    public class share
    {
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
    }
    public class usersuggestion
    {
        public string Comment { get; set; }
    }
    public class Authentication
    {
        public string AuthType { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
    }
    public class AdminProfile
    {
        public string Email { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
    }
    public class rESEND
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
    public class chkotp
    {
        public int UserId { get; set; }
        public int OTP { get; set; }
    }
    public class banner
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string BannerHungry { get; set; }
        public string BannerEng { get; set; }
        public int Time { get; set; }
        public bool Status { get; set; }
        public string Url { get; set; }
    }
    public class getbanner : ApiResponse
    {
        public List<banner> getList { get; set; }
    }
    public class bannerview : ApiResponse
    {
        public banner data { get; set; }
    }
    public class Otp : ApiResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public int Usertype { get; set; }
        public string Phone { get; set; }
        public int PharmaCompId { get; set; }
        public string UserIds { get; set; }

        public string ProfilePhoto { get; set; }
        public int SMS { get; set; }
        public bool AuthStatus { get; set; }
        public int BussinessStatus { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int WizardStatus { get; set; }
    }
    public class bannerstatus
    {
        public bool Status { get; set; }
    }
    public class getBannerstatus : ApiResponse
    {
        public bannerstatus data { get; set; }
    }
    public class updateBannerstatus : ApiResponse
    {
        public bool Status { get; set; }
    }
    public class Bannerrecrd
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        public int? BannerId { get; set; }
        public DateTime? ShowDate { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
    }
    public class showbannerrecord : ApiResponse
    {
        public List<Bannerrecrd> data { get; set; }
    }
    public class myversion : ApiResponse
    {
        public string Version { get; set; }
        public int Count { get; set; }
        public bool Agreement { get; set; }
    }
    public class Messge
    {
        public int? SenderId { get; set; }
        public int? RecieverId { get; set; }
        public int? SenderUserType { get; set; }
        public int? RecieverUserType { get; set; }
        public string ConnectionId { get; set; }
        public string Messages { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Isseen { get; set; }
        public string RequestType { get; set; }
        public string ChatUserName { get; set; }
        public int? MyId { get; set; }
        public int? YourId { get; set; }
        public int SeenCount { get; set; }
        public string DocCategory { get; set; }
        public int DocId { get; set; }
        public int NotificationStatus { get; set; }
    }
    public class msglist : ApiResponse
    {
        public List<Messge> data { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int AdminCount { get; set; }
    }
    public class send
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }

    }
    public class calljoin : ApiResponse
    {
        public string MeetingId { get; set; }
        public int Status { get; set; }
    }
    public class EmailCampaignModel
    {
        public string Name { get; set; }
        public List<doctor> Doclist { get; set; }
        public string ApiPath { get; set; }
        public string UserId { get; set; }
        public string Errorpath { get; set; }

        public EmailCampaignModel(string name, List<doctor> list, string userId)
        {
            Name = name;
            Doclist = list;
            ApiPath = "https://testapi.myclickdoctor.com/";
            UserId = userId;
            Errorpath = "https://testapi.myclickdoctor.com/TempImages/doctor-icon.png";
        }
    }
    public class emailhandelstatus : ApiResponse
    {
        public bool? Fifteenmin { get; set; }
        public bool? Onehour { get; set; }
        public bool? Twentyfourhour { get; set; }
    }
    public class RemainderEmail
    {
        public string Name { get; set; }
        public string AppointmentId { get; set; }
        public string ApiPath { get; set; }
        public string RemainingTime { get; set; }
        public string Appointmentwith { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string AppointmentType { get; set; }
        public RemainderEmail(string name, string AppId, string remainingtime, string appointmentwith, string startTime, int duration, string appointmentType)
        {
            Name = name;
            AppointmentId = AppId;
            ApiPath = "https://testapi.myclickdoctor.com/";
            RemainingTime = remainingtime;
            Appointmentwith = appointmentwith;
            StartTime = startTime;
            Duration = duration;
            AppointmentType = appointmentType;
        }
    }
    public class reminderKOl
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public string ApiPath { get; set; }
        public int Duration { get; set; }
        public string AppointmentType { get; set; }
        public reminderKOl(string name, DateTime date, int duration, string appointmenttype)
        {
            Name = name;
            StartTime = date;
            ApiPath = "https://testapi.myclickdoctor.com/";
            Duration = duration;
            AppointmentType = appointmenttype;
        }
    }
    public class appdetails
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MeetingId { get; set; }
        public int MyRole { get; set; }
        public int MeetingJoinStatus { get; set; }
        public string Subject { get; set; }
        public int MyId { get; set; }
        public int YourId { get; set; }
        public string MyName { get; set; }
        public string YourName { get; set; }
    }
    public class countdown
    {
        public string AppId { get; set; }
        public int DocId { get; set; }
    }
    public class updateseen
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
    }
    public class Appointmentscall : ApiResponse
    {
        public string MeetingId { get; set; }
        public int Calllength { get; set; }
        public int DocId { get; set; }
    }
    public class Info : ApiResponse
    {
        public string Infomation { get; set; }
    }
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ProjectListModel : ApiResponse
    {
        public List<ProjectModel> list { get; set; }
    }
    public class ReportModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Specialty { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int Calllength { get; set; }
        public string Calltype { get; set; }
        public int Amount { get; set; }
    }
    public class ReportListModel : ApiResponse
    {
        public List<ReportModel> list { get; set; }
    }
    public class AssignProjectModel
    {
        public int ProjectId { get; set; }
        public int PharmaCompanyId { get; set; }
    }
    public class GroupchatViewModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int? ParticipateId { get; set; }
        public string ConnectionId { get; set; }
        public int? CreatedBy { get; set; }
        public int Count { get; set; }
    }
    public class Listofgroup:ApiResponse
    {
        public List<GroupchatViewModel> list { get; set; }
        
    }
}

