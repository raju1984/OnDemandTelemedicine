using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class ActivityList
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int Status { get; set; }
        public int UserType { get; set; }
        public int? DocId { get; set; }
        public int? PharmaId { get; set; }
        public int? NurseId { get; set; }
        public int? PatientId { get; set; }
        public int? DocToDocId { get; set; }

        public Appoinments Appointment { get; set; }
    }
}
