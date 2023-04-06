using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class AmamnesticDataPreAppointment
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string AmamnesticDetails { get; set; }
        public int? Type { get; set; }
    }
}
