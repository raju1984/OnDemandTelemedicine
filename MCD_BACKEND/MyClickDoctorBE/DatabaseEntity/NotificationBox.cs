using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class NotificationBox
    {
        public int Id { get; set; }
        public string ReqId { get; set; }
        public string Message { get; set; }
        public int? AdminId { get; set; }
        public int? ParentId { get; set; }
        public int Duration { get; set; }
        public int Status { get; set; }
        public int Isseen { get; set; }
        public string AdminComment { get; set; }
        public int UserId { get; set; }
        public int UserNotificationStatus { get; set; }
        public string MeetingId { get; set; }

        public Administrator Admin { get; set; }
        public Users User { get; set; }
    }
}
