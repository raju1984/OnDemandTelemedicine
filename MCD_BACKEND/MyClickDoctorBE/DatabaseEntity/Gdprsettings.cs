using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Gdprsettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool FirstName { get; set; }
        public bool SecondName { get; set; }
        public bool HomeAddress { get; set; }
        public bool EmailAddress { get; set; }
        public bool? IdentificationCardNumber { get; set; }
        public bool? LocationData { get; set; }
        public bool? Ipaddress { get; set; }
        public bool? MedicalHistory { get; set; }
        public bool? Morbidity { get; set; }
    }
}
