using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Payments
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public int? Pgcurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? AppoinmentId { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }
        public string TransactionId { get; set; }
        public string Remark { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Invoice { get; set; }
        public int? InvoiceStatus { get; set; }
        public string PaymentId { get; set; }
        public string POSTransactionId { get; set; }
        public int? RefundStatus { get; set; }
        public DateTime? RefundDate { get; set; }
        public bool? HalfRefund { get; set; }

        public Appoinments Appoinment { get; set; }
    }
}
