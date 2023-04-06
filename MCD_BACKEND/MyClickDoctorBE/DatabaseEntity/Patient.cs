using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Patient
    {
        public Patient()
        {
            Appoinments = new HashSet<Appoinments>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortIntroduction { get; set; }
        public DateTime Dob { get; set; }
        public string City { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public string Zipcode { get; set; }
        public string Gender { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ProfileUrl { get; set; }
        public int Status { get; set; }
        public int WizardStatus { get; set; }
        public bool? GDPR { get; set; }
        public string CountryCode { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceAddress { get; set; }
        public string TaxNumber { get; set; }

        public ICollection<Appoinments> Appoinments { get; set; }
    }
}
