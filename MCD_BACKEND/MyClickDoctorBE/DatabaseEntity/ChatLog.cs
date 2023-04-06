using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class ChatLog
    {
        public int Id { get; set; }
        public string Mesaage { get; set; }
        public DateTime ChatTime { get; set; }
        public bool? IsSeen { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public int ChatId { get; set; }
    }
}
