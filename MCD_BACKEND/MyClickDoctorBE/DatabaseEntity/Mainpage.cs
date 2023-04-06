using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Mainpage
    {
        public int Id { get; set; }
        public string BannerHeaderName { get; set; }
        public string BannerHeaderPoint { get; set; }
        public string Points { get; set; }
        public string BannerHeaderNameHu { get; set; }
        public string BannerHeaderPointHu { get; set; }
        public string PointsHu { get; set; }
        public string UserComment { get; set; }
    }
}
