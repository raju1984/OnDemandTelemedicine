using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Groupchat
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int? ParticipateId { get; set; }
        public string ConnectionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Createddate { get; set; }
    }
}
