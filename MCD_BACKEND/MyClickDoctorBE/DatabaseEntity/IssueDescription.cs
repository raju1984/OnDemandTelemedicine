using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class IssueDescription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ActivityName { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
