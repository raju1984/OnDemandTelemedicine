using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PatientMedicalHistory
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int IllnessesType { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
    }
}
