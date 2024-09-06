using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Extensions
{
    public static class OutgoingRemoteActivityDataExtensions
    {
        public static DispatchRemoteActivityData PrepareForDispatch(this OutgoingRemoteActivityData data)
        {
            return new DispatchRemoteActivityData
            {
                Payload = data.Payload,
                CompliantRequestTarget = data.CompliantRequestTarget,
                Headers = data.Headers,
                Target = data.Target
            };
        }
    }
}
