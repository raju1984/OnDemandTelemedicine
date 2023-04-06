using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Doctors
    {
        public Doctors()
        {
            Appoinments = new HashSet<Appoinments>();
            DoctorReview = new HashSet<DoctorReview>();
            Doctorspecilization = new HashSet<Doctorspecilization>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime? ContarctEndDate { get; set; }
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
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Gander { get; set; }
        public DateTime? Dob { get; set; }
        public string PhoneNo { get; set; }
        public string AddressLine { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string AboutMe { get; set; }
        public string RegistrationNo { get; set; }
        public int? Year { get; set; }
        public bool? DoctorConsultation { get; set; }
        public int? DoctorConslutationCount { get; set; }
        public int? DoctorType { get; set; }
        public string CountryCode { get; set; }
        public int? MSHCID { get; set; }
        public string DocCategory { get; set; }

        public bool? Agreement { get; set; }
        public bool? IsValidated { get; set; }

        public int? MaxCallCount { get; set; }
        public string Project { get; set; }
        public bool? AllowPatient { get; set; }
        public Users User { get; set; }
        public ICollection<Appoinments> Appoinments { get; set; }
        public ICollection<DoctorReview> DoctorReview { get; set; }
        public ICollection<Doctorspecilization> Doctorspecilization { get; set; }
    }
}
