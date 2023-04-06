using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Morbidity
    {
        public int Id { get; set; }
        public string Anamnesis { get; set; }
        public string Diagnosis { get; set; }
        public string Therapy { get; set; }
        public int AppointmentId { get; set; }
        public DateTime? Created { get; set; }
    }
}
