using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PharmaMaxConsultationCount
    {
        public int Id { get; set; }
        public int ConsulationCount { get; set; }
        public int? RefCount { get; set; }
        public int? CallCount { get; set; }
        public bool? Fifteenmin { get; set; }
        public bool? Onehour { get; set; }
        public bool? Twentyfourhour { get; set; }
    }
}
