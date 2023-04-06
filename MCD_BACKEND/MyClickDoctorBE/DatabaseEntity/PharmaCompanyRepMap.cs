using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PharmaCompanyRepMap
    {
        public int Id { get; set; }
        public int PharmaRepId { get; set; }
        public int PharmaComId { get; set; }
        public int? Status { get; set; }
        public DateTime? Created { get; set; }

        public PharmaceuticalsCompany PharmaCom { get; set; }
        public PharmaRepresentative PharmaRep { get; set; }
    }
}
