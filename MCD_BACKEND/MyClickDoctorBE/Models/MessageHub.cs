using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MyClickDoctorBE.DatabaseEntity;

namespace MyClickDoctorBE.Models
{
    public class MessageHub : Hub
    {
        public Task Send(notify data)
        {
            using (MyClickDoctorV4 _db = new MyClickDoctorV4())
            {
                if (data.CallType==0)
                {
                    var CheckBeforeMeeting = _db.Appoinments.Where(a => a.BeforeCall == 1 && a.Appoinmentcode == data.MeetingId).ToList();
                    if (CheckBeforeMeeting.Count > 0)
                    {
                        data.CallStatus = 0;
                    }
                    else
                    {
                        var app = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).ToList();
                        foreach (var item in app)
                        {
                            item.BeforeCall = 1;
                        }
                        _db.SaveChanges();
                        data.CallStatus = 1;
                    }
                }
                else
                {
                    var CheckEndMeeting = _db.Appoinments.Where(a => a.EndCall == 1 && a.Appoinmentcode == data.MeetingId).ToList();
                    if (CheckEndMeeting.Count > 0)
                    {
                        data.CallStatus = 0;
                    }
                    else
                    {
                        var app = _db.Appoinments.Where(a => a.Appoinmentcode == data.MeetingId).ToList();
                        foreach (var item in app)
                        {
                            item.EndCall = 1;
                        }
                        _db.SaveChanges();
                        data.CallStatus = 1;
                    }
                }
               
            }
            return Clients.All.SendAsync("Send", data);
        }
        public class Message
        {
            public int? SenderId { get; set; }
            public int? RecieverId { get; set; }
            public int? SenderUserType { get; set; }
            public int? RecieverUserType { get; set; }
            public string ConnectionId { get; set; }
            public string Messages { get; set; }
            public DateTime? CreatedAt { get; set; }
            public bool? Isseen { get; set; }
            public string RequestType { get; set; }
            public string Chattype { get; set; }
            public string Chatwith { get; set; }
        }
        public class notify
        {
            public string MeetingId { get; set; }
            public int CallType { get; set; }
            public int CallStatus { get; set; }
        }
        public class newchat:ApiResponse
        {
            public string ConnectionId { get; set; }
            public string ChatName { get; set; }
            public string DocCategory { get; set; }
            public int DocId { get; set; }
        }
    }
}
