using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
    public enum ApiCustomResponseCode
    {
        Ok = 200,
        BadRequest = 400,
        Error = 501,
        Block=250
    }
    enum UserType
    {
        Admin=1,
        Doctor=2,
        Pharma = 3,
        Patient =4,
        PharmaRep=5,
        Nurse=6,
        Operator=7
    }
    enum UserStatus
    {
        Blocked=0,
        Active=1, 
       // Rejected=2,
        Waiting_for_Aproval=2
    }
}
