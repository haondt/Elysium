using DotNext;
using Elysium.Server.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public interface ITypedActorService
    {
        public ActorType ActorType { get; }
        public Task<Result<byte[]>> GetSigningKeyAsync(LocalUri id);
        public Task<Result<byte[]>> GetPublicKeyAsync();
        public Task<Result<JObject>> GenerateDocumentAsync();
    }
}
