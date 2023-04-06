using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class SpecilizationCategory
    {
        public SpecilizationCategory()
        {
            DoctorPhramaRepMap = new HashSet<DoctorPhramaRepMap>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string HunName { get; set; }
        public bool? AllowPatient { get; set; }

        public ICollection<DoctorPhramaRepMap> DoctorPhramaRepMap { get; set; }
    }
}
