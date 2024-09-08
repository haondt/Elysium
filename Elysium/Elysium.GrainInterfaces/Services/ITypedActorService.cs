using DotNext;
using Elysium.Hosting.Models;
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
        public Task<Result<byte[]>> GetSigningKeyAsync(LocalUri uri);
        public Task<Result<byte[]>> GetPublicKeyAsync(LocalUri uri);
        public Task<Result<JObject>> GenerateIdentityDocumentAsync(LocalUri uri);
    }
}
