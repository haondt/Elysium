using DotNext;
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
        public Task<byte[]> GetSigningKeyAsync();
        public Task<byte[]> GetPublicKeyAsync();
        public Task<Result<JObject>> GenerateDocumentAsync();
    }
}
