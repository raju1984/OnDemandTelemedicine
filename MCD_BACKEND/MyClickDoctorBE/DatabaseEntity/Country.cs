using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string AccessCode { get; set; }
        public int? Status { get; set; }
        public int? Zone { get; set; }
    }
}
