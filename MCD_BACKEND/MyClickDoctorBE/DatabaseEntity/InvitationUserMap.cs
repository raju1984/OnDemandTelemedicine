using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class InvitationUserMap
    {
        public int Id { get; set; }
        public int? SentTo { get; set; }
        public int? FromUserId { get; set; }
        public string Email { get; set; }
        public string MessageWithInvitation { get; set; }
    }
}
