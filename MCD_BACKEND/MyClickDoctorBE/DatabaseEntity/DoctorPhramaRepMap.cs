using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DoctorPhramaRepMap
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int RepId { get; set; }
        public int Status { get; set; }
        public DateTime? Created { get; set; }

        public SpecilizationCategory Cat { get; set; }
    }
}
