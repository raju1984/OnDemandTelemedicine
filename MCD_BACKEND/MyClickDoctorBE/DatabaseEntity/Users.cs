using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Users
    {
        public Users()
        {
            Doctors = new HashSet<Doctors>();
            NotificationBox = new HashSet<NotificationBox>();
            NotificationToken = new HashSet<NotificationToken>();
            PharmaRepresentative = new HashSet<PharmaRepresentative>();
            PharmaceuticalsCompany = new HashSet<PharmaceuticalsCompany>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string UserPassword { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public int? EmailOtp { get; set; }
        public int? MobileOtp { get; set; }
        public DateTime Lastlogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public string Token { get; set; }
        public int? BussinessStatus { get; set; }
        public DateTime LastMeeting { get; set; }
        public bool? EmailAuth { get; set; }
        public bool? SMSAuth { get; set; }
        public int? Isemail { get; set; }
        public int? Unscubscribe { get; set; }
        public DateTime? EmailUpdatedAt { get; set; }
        public ICollection<Doctors> Doctors { get; set; }
        public ICollection<NotificationBox> NotificationBox { get; set; }
        public ICollection<NotificationToken> NotificationToken { get; set; }
        public ICollection<PharmaRepresentative> PharmaRepresentative { get; set; }
        public ICollection<PharmaceuticalsCompany> PharmaceuticalsCompany { get; set; }
    }
}
