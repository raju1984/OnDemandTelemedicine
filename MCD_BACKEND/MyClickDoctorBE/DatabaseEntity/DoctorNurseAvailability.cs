using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DoctorNurseAvailability
    {
        public int Id { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string DayofWeek { get; set; }
        public int? UserId { get; set; }
    }
}
