using DotNext;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IDocumentResolver
    {
        Task<Result<JObject>> GetDocument(Uri uri);
        Task<Result<JObject>> GetDocumentAsync(string url);
    }
}
