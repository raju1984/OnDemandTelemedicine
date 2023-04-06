using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class BannerRecord
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        public int? BannerId { get; set; }
        public DateTime? ShowDate { get; set; }
    }
}
