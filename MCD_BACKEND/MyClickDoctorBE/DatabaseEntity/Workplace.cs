using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Workplace
    {
        public int Id { get; set; }
        public string WorkplaceTitle { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AddressLine { get; set; }
    }
}
