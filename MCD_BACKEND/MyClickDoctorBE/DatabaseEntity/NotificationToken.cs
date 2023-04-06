using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class NotificationToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }

        public Users User { get; set; }
    }
}
