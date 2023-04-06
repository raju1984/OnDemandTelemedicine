using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Faqs
    {
        public int Id { get; set; }
        public string Ques { get; set; }
        public string Ans { get; set; }
        public int? UserType { get; set; }
    }
}
