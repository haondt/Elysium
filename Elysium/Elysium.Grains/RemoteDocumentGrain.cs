using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using Haondt.Core.Models;
using Haondt.Web.Core.Reasons;
using JsonLD.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class RemoteDocumentGrain([PersistentState(GrainConstants.GrainDocumentStorage)] IPersistentState<DocumentState> state,
        IGrainFactory grainFactory,
        IActivityPubHttpService httpService,
        IOptions<RemoteDocumentSettings> options,
        IJsonLdService jsonLdService) : Grain, IRemoteDocumentGrain
    {
        private readonly RemoteDocumentSettings _settings = options.Value;
        private RemoteIri _id;
        private IInstanceActorAuthorGrain _instanceActorGrain = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        // TODO: I forget what skipCachingFirstLayer is for
        // also, need to add option to use instance actor instead of requester? Or i guess the call can just inject the instance actor as the requester
        public async Task<Result<JArray, WebReason>> GetExpandedValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer = false)
        {
            var state = await GetValueAsync(requester, skipCachingFirstLayer);
            if (!state.IsSuccessful)
                return new(state.Reason);

            return new(await jsonLdService.ExpandAsync(requester, state.Value));
        }

        public async Task<Result<JObject, WebReason>> GetValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer = false)
        {
            if (state.State.Value != null)
            {
                if (state.State.UpdatedOnUtc != null)
                {
                    if (DateTime.UtcNow - state.State.UpdatedOnUtc > TimeSpan.FromHours(_settings.LifetimeInHours))
                    {
                        state.State.UpdatedOnUtc = null;
                        state.State.Value = null;
                    }
                    else
                    {
                        return new(state.State.Value);
                    }
                }
            }

            var result = await InternalGetValueAsync(requester);
            if (!result.IsSuccessful)
                return result;

            state.State.UpdatedOnUtc = DateTime.UtcNow;
            state.State.Value = result.Value;
            await state.WriteStateAsync();
            return new(state.State.Value);

        }
        private async Task<Result<JObject, WebReason>> InternalGetValueAsync(IHttpMessageAuthor requester)
        {
            //if (!_id.HasValue)
            //    _id = uriGrainFactory.GetIdentity(this);

            // todo: add check that ensures it is a jobject? or maybe just compact it here

            //var initialObject = await httpService.GetAsync(new HttpGetData
            //{
            //    Author = requester,
            //    Target = _id.Value
            //});
            //if (!initialObject.IsSuccessful)
            //    return initialObject;

            throw new NotImplementedException();
        }


    }
}
