using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using Elysium.Hosting.Models;
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
    public class RemoteDocumentGrain([PersistentState(nameof(DocumentState))] IPersistentState<DocumentState> state,
        IUriGrainFactory uriGrainFactory,
        IGrainFactory grainFactory,
        IActivityPubHttpService httpService,
        IOptions<RemoteDocumentSettings> options,
        IJsonLdService jsonLdService) : Grain, IRemoteDocumentGrain
    {
        private readonly RemoteDocumentSettings _settings = options.Value;
        private Optional<RemoteUri> _id;
        private IInstanceActorGrain _instanceActorGrain = grainFactory.GetGrain<IInstanceActorGrain>(Guid.Empty);
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        // TODO: I forget what skipCachingFirstLayer is for
        // also, need to add option to use instance actor instead of requester? Or i guess the call can just inject the instance actor as the requester
        public async Task<Result<JArray>> GetExpandedValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer = false)
        {
            var state = await GetValueAsync(requester, skipCachingFirstLayer);
            if (!state.IsSuccessful)
                return new(state.Error);

            return await jsonLdService.ExpandAsync(requester, state.Value);
        }

        public async Task<Result<JObject>> GetValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer = false)
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
                        return state.State.Value;
                    }
                }
            }

            var result = await InternalGetValueAsync(requester);
            if (!result.IsSuccessful)
                return result;

            state.State.UpdatedOnUtc = DateTime.UtcNow;
            state.State.Value = result.Value;
            await state.WriteStateAsync();
            return state.State.Value;

        }
        private async Task<Result<JObject>> InternalGetValueAsync(IHttpMessageAuthor requester)
        {
            //if (!_id.HasValue)
            //    _id = uriGrainFactory.GetIdentity(this);

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
