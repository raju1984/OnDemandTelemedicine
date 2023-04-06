using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class DoctorReview
    {
        public int Id { get; set; }
        public int DocId { get; set; }
        public int FromUserId { get; set; }
        public int Type { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public DateTime Created { get; set; }
        public int ParentId { get; set; }
        public int AppointmentId { get; set; }

        public Doctors Doc { get; set; }
        public DoctorReview IdNavigation { get; set; }
        public DoctorReview InverseIdNavigation { get; set; }
    }
}
