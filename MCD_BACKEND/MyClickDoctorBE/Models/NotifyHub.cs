using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
    public class NotifyHub : Hub<ITypedHubedClient>
    {
    }
}
