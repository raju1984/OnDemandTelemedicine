using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PriceListMaster
    {
        public int Id { get; set; }
        public decimal? AppointmentRate { get; set; }
        public int? CurrencyId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int? Duration { get; set; }
        public int CategoryId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
