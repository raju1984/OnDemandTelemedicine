using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Doctorspecilization
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public string Name { get; set; }
        public int DocId { get; set; }
        public int? Status { get; set; }

        public Doctors Doc { get; set; }
    }
}
