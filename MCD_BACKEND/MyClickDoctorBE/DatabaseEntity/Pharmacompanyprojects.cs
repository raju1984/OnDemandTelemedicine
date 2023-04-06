using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Pharmacompanyprojects
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int PharmaCompanyId { get; set; }
    }
}
