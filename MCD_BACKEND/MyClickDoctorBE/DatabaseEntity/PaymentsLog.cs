using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PaymentsLog
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? Amount { get; set; }
        public string TransactionId { get; set; }
        public int? Status { get; set; }
        public DateTime? Created { get; set; }
        public string PaymentMethod { get; set; }
        public int AppointmentId { get; set; }
        public DateTime? Updated { get; set; }
    }
}
