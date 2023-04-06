using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class AppointmentDocuments
    {
        public int Id { get; set; }
        public int? AppointmetId { get; set; }
        public string DocUrl { get; set; }
        public int? Isseen { get; set; }
        public string Filetype { get; set; }
        public int? Usertype { get; set; }
        public string MeetingId { get; set; }
        public int? DocId { get; set; }
        public string FileName { get; set; }
    }
}
