using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class InvitationMaster
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int? DoctorPopupStatus { get; set; }
        public string Status { get; set; }
        public int? DoctortoDoctorPopupStatus { get; set; }
        public int? PatientPopupStatus { get; set; }
        public int? NursePopupStatus { get; set; }
        public int? PharmaPopupStatus { get; set; }
        public int? UserType { get; set; }
        public int? UserId { get; set; }
    }
}
