using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Server.Services
{
    public interface IHostingService
    {
        bool IsLocalHost(Iri iri);
        LocalIri GetIriForLocalizedUsername(string localizedUsername);
        //Task<Result<string>> GetUsernameFromLocalUriAsync(LocalUri iri);
        string GetUsernameFromLocalizedUsername(string username);
        /// <summary>
        /// Check if the <paramref name="iri"/> is scoped to the <paramref name="user"/>.<br/> 
        /// For example, "https://localhost.com/users/terry/status/95538 is scoped
        /// to "https://localhost.com/users/terry"
        /// </summary>
        /// <param name="iri"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsScopedToLocalUser(LocalIri iri, LocalIri user);
        public LocalIri GetLocalUserScopedUri(LocalIri userUri, string next);
        public string Host { get; }
    }
}
