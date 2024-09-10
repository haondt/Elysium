﻿using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorWorkerGrain : IGrain<RemoteIri>
    {
        Task PublishEvent(IncomingRemoteActivityData activity);
        Task IngestActivityAsync(OutgoingRemoteActivityData activity);
    }
}
