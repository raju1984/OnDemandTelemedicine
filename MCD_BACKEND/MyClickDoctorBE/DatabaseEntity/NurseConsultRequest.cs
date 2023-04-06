using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class NurseConsultRequest
    {
        public int Id { get; set; }
        public string PatientRequest { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? Status { get; set; }
        public string DaysofWeek { get; set; }
    }
}
