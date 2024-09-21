using Haondt.Core.Models;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class NavigateEventHandler : IEventHandler
    {
        public const string OPEN_ = "";
        public Task<Optional<IComponent>> HandleAsync(string eventName, IRequestData requestData)
        {
            throw new NotImplementedException();
        }
    }
}
