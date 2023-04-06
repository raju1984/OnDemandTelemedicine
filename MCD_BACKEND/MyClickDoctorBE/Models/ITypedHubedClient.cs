using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
   public interface ITypedHubedClient
    {
        Task BroadCastMessage(MessageHub message);
    }
}
