using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Currency
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
    }
}
