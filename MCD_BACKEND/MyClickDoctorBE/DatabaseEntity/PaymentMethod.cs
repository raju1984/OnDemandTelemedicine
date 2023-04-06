using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PaymentMethod
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PaymentType { get; set; }
        public string Ccno { get; set; }
        public string Cvvno { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public bool? IsPrimary { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
    }
}
