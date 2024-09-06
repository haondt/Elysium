using DotNext;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IActivityPubJsonNavigator
    {
        public Result<(string PublicKey, PublicKeyType PublicKeyType)> GetPublicKey(JArray expanded);
        public Result<Uri> GetInbox(JArray expanded);
        Result<string> GetType(JObject target);
        Optional<string> GetId(JObject target);
    }
    public enum PublicKeyType
    {
        Pem,
        Multibase
    } 
}
