﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IDispatchRemoteActivityGrain : IGrainWithGuidKey
    {
        Task Send(DispatchRemoteActivityData data);
        Task NotifyOfWorkerCompletion(long workerId);
    }
}
