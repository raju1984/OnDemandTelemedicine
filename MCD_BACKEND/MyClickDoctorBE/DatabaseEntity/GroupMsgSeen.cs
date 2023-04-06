using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class GroupMsgSeen
    {
        public int Id { get; set; }
        public string MsgId { get; set; }
        public int? ParticipateId { get; set; }
    }
}
