using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class TempPharmaRepresentative
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AboutUs { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TerityDefination { get; set; }
        public string CompanyName { get; set; }
        public string ShortDescription { get; set; }
        public string RegNumber { get; set; }
        public string CompleteAddress { get; set; }
        public string ServicesHours { get; set; }
        public int? Status { get; set; }
        public string ProfileUrl { get; set; }
        public string CountryCode { get; set; }
    }
}
