using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Hosting.Services
{
    public interface IIriService
    {
        LocalIri InstanceActorIri { get; }
        LocalActorIriCollection GetLocalActorIris(LocalIri iri);
        LocalIri GetIriForLocalizedActorname(string localizedUsername);
        string GetActornameFromLocalizedActorname(string localizedUsername);
        LocalIri GetActorScopedObjectIri(LocalIri user, string id);
        LocalIri GetAnonymousObjectIri(string id);
        LocalIri GetActorScopedActivityIri(LocalIri user, string id);
        bool IsScopedToLocalActor(LocalIri iri, LocalIri user);
        bool IsScopedToLocalActor(LocalIri iri);
        bool IsLocalActorIri(LocalIri iri);
    }
}
