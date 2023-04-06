using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Appoinments
    {
        public Appoinments()
        {
            ActivityList = new HashSet<ActivityList>();
            Payments = new HashSet<Payments>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Duration { get; set; }
        public DateTime BookingDate { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? NurseId { get; set; }
        public int? PharmaRepId { get; set; }
        public string ShortSummaryWrittenByPatient { get; set; }
        public DateTime? LastVisitedDate { get; set; }
        public string AppointmentTitle { get; set; }
        public string VideoCallKey { get; set; }
        public string Description { get; set; }
        public int? RatingGivenByPatient { get; set; }
        public string DoctorAdviseAfterConsultation { get; set; }
        public string PatientAamnesticData { get; set; }
        public DateTime? NurseStartDate { get; set; }
        public string NurseFeedbackAboutDoctor { get; set; }
        public bool? IsNextConsultationAdvise { get; set; }
        public DateTime? NextConsultationDate { get; set; }
        public bool? IsDoctorShowedUp { get; set; }
        public int? CallQualityFeedbackPatient { get; set; }
        public int? CallQualityFeedbackDoctor { get; set; }
        public string TimeZone { get; set; }
        public int? CompanyId { get; set; }
        public string Appoinmentcode { get; set; }
        public int? DocId { get; set; }
        public int? IsActive { get; set; }
        public int? DocSlotId { get; set; }
        public int? NurseSlotId { get; set; }
        public int? UserRole { get; set; }
        public int? DoctorConsultationId { get; set; }
        public int? MyRole { get; set; }
        public int? NurseSuggestionStatus { get; set; }
        public int? CancelBy { get; set; }
        public int? DoctorPopup { get; set; }
        public int? NursePopup { get; set; }
        public int? PatientPopup { get; set; }
        public int? PharmaPopup { get; set; }
        public int? DoctortoPopup { get; set; }
        public int? DoctorCancelStatus { get; set; }
        public int? BeforeCall { get; set; }
        public int? EndCall { get; set; }
        public bool? IsFifteenEmail { get; set; }
        public bool? IshourEmail { get; set; }
        public bool? IsdayEmail { get; set; }
        public string ColorCode { get; set; }
        public int? Calllength { get; set; }
        public bool? IsPaymentEmail { get; set; }
        public DoctorAvailibiltyforAppointment DocSlot { get; set; }
        public Doctors Doctor { get; set; }
        public Nurses Nurse { get; set; }
        public Patient Patient { get; set; }
        public PharmaRepresentative PharmaRep { get; set; }
        public ICollection<ActivityList> ActivityList { get; set; }
        public ICollection<Payments> Payments { get; set; }
    }
}
