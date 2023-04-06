using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class TempDoctors
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int YearsOfExperiecne { get; set; }
        public string MedicalRegistrationNo { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
        public string StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string ContractualDoctorsCompany { get; set; }
        public string ShortIntroduction { get; set; }
        public int? PriceId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string Gander { get; set; }
        public DateTime? Dob { get; set; }
        public string PhoneNo { get; set; }
        public string AddressLine { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string AboutMe { get; set; }
        public string RegistrationNo { get; set; }
        public int? DoctorType { get; set; }
        public string CountryCode { get; set; }
        public int? MSHCID { get; set; }
    }
}
