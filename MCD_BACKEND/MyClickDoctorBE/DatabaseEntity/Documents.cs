using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Documents
    {
        public Documents()
        {
            PharmaRepDocumentMap = new HashSet<PharmaRepDocumentMap>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<PharmaRepDocumentMap> PharmaRepDocumentMap { get; set; }
    }
}
