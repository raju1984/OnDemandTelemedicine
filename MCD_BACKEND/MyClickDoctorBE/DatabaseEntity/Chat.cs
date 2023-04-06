using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Chat
    {
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? RecieverId { get; set; }
        public int? SenderUserType { get; set; }
        public int? RecieverUserType { get; set; }
        public string ConnectionId { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Isseen { get; set; }
        public bool? Userseen { get; set; }
        public string RequestType { get; set; }
        public string Chattype { get; set; }
        public string Chatwith { get; set; }
    }
}
