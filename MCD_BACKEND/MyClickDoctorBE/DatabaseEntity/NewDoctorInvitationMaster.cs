using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class NewDoctorInvitationMaster
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specility { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public int? CompanyId { get; set; }
    }
}
