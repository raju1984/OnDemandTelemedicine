using System;
using System.Collections.Generic;

namespace MyClickDoctorBE.DatabaseEntity
{
    public partial class Projects
    {
        public Projects()
        {
            Reports = new HashSet<Reports>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Reports> Reports { get; set; }
    }
}
