using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Administrator
    {
        public Administrator()
        {
            NotificationBox = new HashSet<NotificationBox>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Mshcid { get; set; }
        public int? UserId { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? Role { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<NotificationBox> NotificationBox { get; set; }
    }
}
