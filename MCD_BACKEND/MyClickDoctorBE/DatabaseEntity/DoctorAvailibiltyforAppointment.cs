using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DoctorAvailibiltyforAppointment
    {
        public DoctorAvailibiltyforAppointment()
        {
            Appoinments = new HashSet<Appoinments>();
        }
        public int Id { get; set; }
        public int DocId { get; set; }
        public DateTime ToTime { get; set; }
        public DateTime FromTime { get; set; }
        public int Status { get; set; }
        public string SlotType { get; set; }
        public bool? Docavailable { get; set; }
        public bool? Nurseavailable { get; set; }
        public bool? Patientavailable { get; set; }
        public bool? Pharmaavailable { get; set; }
        public string DoctorType { get; set; }
        public string Type { get; set; }
        public int? MaxCount { get; set; }
        public string Subject { get; set; }
        public int? Duration { get; set; }
        public bool? Isemail { get; set; }
        public ICollection<Appoinments> Appoinments { get; set; }
    }
}
