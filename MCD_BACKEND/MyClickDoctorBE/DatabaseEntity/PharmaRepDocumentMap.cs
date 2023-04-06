using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PharmaRepDocumentMap
    {
        public int Id { get; set; }
        public int? PharmaRepId { get; set; }
        public int? DocumentId { get; set; }

        public Documents Document { get; set; }
    }
}
