using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DocAward
    {
        public int Id { get; set; }
        public string AwardTitle { get; set; }
        public int? Year { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
    }
}
