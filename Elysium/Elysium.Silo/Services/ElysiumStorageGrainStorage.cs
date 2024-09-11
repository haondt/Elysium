using Elysium.Persistence.Services;
using Elysium.Silo.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Silo.Services
{
    public class ElysiumStorageGrainStorage(IElysiumStorage elysiumStorage,
        IServiceProvider serviceProvider,
        IOptions<ClusterOptions> clusterOptions) : IGrainStorage
    {
        public async Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            var key = ElysiumStorageGrainStateEntity<T>.GetStorageKey(clusterOptions.Value.ServiceId, grainId);
            try
            {

                var result = await elysiumStorage.Delete(key);
                if (!result.IsSuccessful)
                    if (result.Reason != Haondt.Persistence.Services.StorageResultReason.NotFound)
                        throw new OrleansStorageException($"Failed to clear grain state. Received reason {result.Reason} from storage");
                grainState.RecordExists = false;
            }
            catch (Exception ex)
            {
                WrappedException.CreateAndRethrow(ex);
            }
        }

        public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            var key = ElysiumStorageGrainStateEntity<T>.GetStorageKey(clusterOptions.Value.ServiceId, grainId);
            try
            {

                var result = await elysiumStorage.Get(key);
                if (!result.IsSuccessful)
                {

                    if (result.Reason == Haondt.Persistence.Services.StorageResultReason.NotFound)
                    {
                        grainState.State = ActivatorUtilities.CreateInstance<T>(serviceProvider);
                        grainState.RecordExists = true;
                        return;
                    }
                    throw new OrleansStorageException($"Failed to read grain state. Received reason {result.Reason} from storage");
                }

                grainState.State = result.Value.Value;
                grainState.RecordExists = true;
            }
            catch (Exception ex) {
                // todo: log
                WrappedException.CreateAndRethrow(ex);
            }
        }

        public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
        {
            var key = ElysiumStorageGrainStateEntity<T>.GetStorageKey(clusterOptions.Value.ServiceId, grainId);
            try
            {

                await elysiumStorage.Set(key, new ElysiumStorageGrainStateEntity<T>
                {
                    StateName = stateName,
                    Value = grainState.State
                });
                grainState.RecordExists = true;
            }
            catch (Exception ex) {
                // todo: log
                WrappedException.CreateAndRethrow(ex);
            }
        }
    }
}
