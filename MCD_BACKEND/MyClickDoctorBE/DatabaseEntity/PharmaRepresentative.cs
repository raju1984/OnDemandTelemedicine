using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PharmaRepresentative
    {
        public PharmaRepresentative()
        {
            Appoinments = new HashSet<Appoinments>();
            PharmaCompanyRepMap = new HashSet<PharmaCompanyRepMap>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int PharmaCompanyId { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? ContractExpiryDate { get; set; }
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
        public int? AssinStatus { get; set; }
        public int? Status { get; set; }
        public string ProfileUrl { get; set; }
        public string CountryCode { get; set; }

        public Users User { get; set; }
        public ICollection<Appoinments> Appoinments { get; set; }
        public ICollection<PharmaCompanyRepMap> PharmaCompanyRepMap { get; set; }
    }
}
