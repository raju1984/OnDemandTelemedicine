using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Reports
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string Specialty { get; set; }
        public string Month { get; set; }
        public int? Year { get; set; }
        public int? Calllength { get; set; }
        public string Calltype { get; set; }
        public int? Amount { get; set; }

        public Projects Project { get; set; }
    }
}
