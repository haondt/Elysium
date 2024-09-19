using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IPublicCollectionGrain : IGrainWithGuidKey
    {
        Task<IEnumerable<string>> GetReferencesAsync(int from, int to);
        Task<IEnumerable<string>> GetReferencesAsync(int count);
        Task IngestReferenceAsync(string iri);
    }
}
