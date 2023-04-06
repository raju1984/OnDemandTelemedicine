using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class PatientMediHistoryDocuments
    {
        public int Id { get; set; }
        public int? PatientMediHistoryId { get; set; }
        public int? DocumentId { get; set; }
    }
}
