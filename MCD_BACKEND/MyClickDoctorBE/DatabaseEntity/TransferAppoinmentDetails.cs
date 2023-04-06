using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class TransferAppoinmentDetails
    {
        public int Id { get; set; }
        public int? TransferId { get; set; }
        public int? AppointmentId { get; set; }
    }
}
