using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PharmaceuticalsCompany
    {
        public PharmaceuticalsCompany()
        {
            PharmaCompanyRepMap = new HashSet<PharmaCompanyRepMap>();
        }

        public int Id { get; set; }
        public string ComanyName { get; set; }
        public string Address { get; set; }
        public int? UserId { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string ProfileUrl { get; set; }
        public int Status { get; set; }

        public Users User { get; set; }
        public ICollection<PharmaCompanyRepMap> PharmaCompanyRepMap { get; set; }
    }
}
