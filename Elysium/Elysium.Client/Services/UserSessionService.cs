using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Elysium.Persistence.Exceptions;
using Haondt.Core.Models;
using Haondt.Persistence.Services;

namespace Elysium.Client.Services
{
    public class UserSessionService : IUserSessionService
    {
        private const string USER_SESSION_COOKIE = "session";
        private Lazy<Task<Result<ILocalActorGrain, ElysiumWebReason>>> _localActorGrainLazy;
        private Lazy<Task<Result<IStorageKeyGrain<UserState>, ElysiumWebReason>>> _userStateGrainLazy;
        private readonly ISessionService _sessionService;
        private readonly IIriService _iriService;
        private readonly IHostingService _hostingService;
        private readonly IActivityPubClientService _activityPubClientService;
        public bool IsAuthenticated => _sessionService.IsAuthenticated();

        public UserSessionService(
            ISessionService sessionService,
            IIriService iriService,
            IHostingService hostingService,
            IGrainFactory<LocalIri> localGrainFactory,
            IStorageKeyGrainFactory<UserState> userStateGrainFactory,
            IActivityPubClientService activityPubClientService)
        {
            _sessionService = sessionService;
            _iriService = iriService;
            _hostingService = hostingService;
            _activityPubClientService = activityPubClientService;
            _localActorGrainLazy = new(async () =>
            {
                var iri = await GetIriAsync();
                return new(localGrainFactory.GetGrain<ILocalActorGrain>(iri.Value));
            });
            _userStateGrainLazy = new(async () =>
            {
                var iri = await GetIriAsync();
                if (!iri.IsSuccessful)
                    return new(iri.Reason);
                var storageKey = UserState.CreateStorageKey(iri.Value);
                return new(userStateGrainFactory.GetGrain(storageKey));
            });
        }

        public async Task<Result<LocalIri, ElysiumWebReason>> GetIriAsync()
        {
            if (!_sessionService.IsAuthenticated())
                return new(ElysiumWebReason.Unauthorized);

            var userIdentity = await _sessionService.GetUserKeyAsync();
            if (!userIdentity.HasValue)
                return new(ElysiumWebReason.Unauthorized);

            return new(await _activityPubClientService.GetLocalIriFromUserIdentityAsync(userIdentity.Value));
        }

        public async Task<Result<string, ElysiumWebReason>> GetLocalizedUsernameAsync()
        {
            var iri = await GetIriAsync();
            if (!iri.IsSuccessful)
                return new(iri.Reason);
            var username = _iriService.GetLocalizedActornameForLocalIri(iri.Value);
            return new(username);
        }

        public async Task<Result<UserState, ElysiumWebReason>> GetUserStateAsync()
        {
            var userStateGrain = await _userStateGrainLazy.Value;
            if (!userStateGrain.IsSuccessful)
                return new(userStateGrain.Reason);

            var state = await userStateGrain.Value.GetAsync();
            if (!state.IsSuccessful && state.Reason != StorageResultReason.NotFound)
                throw new StorageException($"Failed to load user state due to reason {state.Reason}");

            if (!state.IsSuccessful)
                return new(new UserState());
            return new(state.Value);
        }

        public async Task<Result<List<LocalIri>, ElysiumWebReason>> GetShadesAsync()
        {
            var state = await GetUserStateAsync();
            if (!state.IsSuccessful)
                return new(state.Reason);
            return new(state.Value.Shades);
        }

        public Optional<LocalIri> GetActiveShade()
        {
            var cookie = _sessionService.GetFromCookie<UserSessionState>(USER_SESSION_COOKIE);
            if (!cookie.HasValue)
                return new();

            if (string.IsNullOrEmpty(cookie.Value.ActiveShade))
                return new();

            Iri activeShade;
            try
            {
                activeShade = Iri.FromUnencodedString(cookie.Value.ActiveShade);
            }
            catch
            {
                cookie.Value.ActiveShade = null;
                _sessionService.SetCookie(USER_SESSION_COOKIE, cookie.Value);
                return new();
            }

            if (activeShade.Host != _hostingService.Host)
            {
                cookie.Value.ActiveShade = null;
                _sessionService.SetCookie(USER_SESSION_COOKIE, cookie.Value);
                return new();
            }

            return new(new LocalIri { Iri = activeShade });
        }
    }
}
