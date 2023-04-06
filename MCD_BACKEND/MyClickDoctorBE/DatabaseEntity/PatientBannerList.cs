using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PatientBannerList
    {
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string BannerHungry { get; set; }
        public string BannerEng { get; set; }
        public int? Time { get; set; }
        public bool BannerStatus { get; set; }
    }
}
