using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DoctorClinicMap
    {
        public int Id { get; set; }
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
        public int? DocumnetId { get; set; }
        public int? UserId { get; set; }
    }
}
