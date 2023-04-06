using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class ClinicDocMap
    {
        public int Id { get; set; }
        public int? DocId { get; set; }
        public int? ClinicId { get; set; }
        public int? UserId { get; set; }
        public string Image { get; set; }
    }
}
