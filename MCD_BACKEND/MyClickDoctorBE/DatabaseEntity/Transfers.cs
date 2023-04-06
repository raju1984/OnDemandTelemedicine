using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Transfers
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public int? ToUserId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
