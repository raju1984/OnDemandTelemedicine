using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Nurses
    {
        public Nurses()
        {
            Appoinments = new HashSet<Appoinments>();
        }

        public int Id { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int IsAvailable { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ShortDescription { get; set; }
        public int UserId { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string ProfileUrl { get; set; }
        public string MobileNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public ICollection<Appoinments> Appoinments { get; set; }
    }
}
