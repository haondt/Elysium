using Elysium.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public interface IActivityPubServerService
    {
        Task IngestActivityAsync(OutgoingRemoteActivityData activity);
    }
}
