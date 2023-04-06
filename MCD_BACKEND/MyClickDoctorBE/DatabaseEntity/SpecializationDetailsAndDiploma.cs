using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class SpecializationDetailsAndDiploma
    {
        public int Id { get; set; }
        public int? YearOfDiploma { get; set; }
        public string NameofDegree { get; set; }
        public string University { get; set; }
        public string Specialization { get; set; }
        public string Remark { get; set; }
        public int? UserId { get; set; }
        public int? SpecializationType { get; set; }
    }
}
