using Elysium.GrainInterfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class HostIntegrityGrain : Grain, IHostIntegrityGrain
    {
        private readonly IPersistentState<HostIntegrityState> _state;
        private readonly HostIntegritySettings _settings;
        private bool _dirty;
        private IDisposable? _timer;

        public HostIntegrityGrain([PersistentState(nameof(HostIntegrityState))] IPersistentState<HostIntegrityState> state,
            IOptions<HostIntegritySettings> options)
        {
            _state = state;
            _settings = options.Value;
        }
        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _state.ReadStateAsync();
            _timer = this.RegisterGrainTimer(SaveState, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            return base.OnActivateAsync(cancellationToken);
        }

        private Task SaveState()
        {
            if (!_dirty)
                return Task.CompletedTask;
            _dirty = false;
            return _state.WriteStateAsync();
        }
        public Task<bool> ShouldSendRequest()
        {
            if (_state.State.Integrity == HostIntegrity.Stable)
                return Task.FromResult(true);
            if (DateTime.UtcNow > _state.State.DeactivatedUntil)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        public Task VoteAgainst()
        {
            _state.State.ConsecutivePositiveVotes = 0;
            if (_state.State.Integrity == HostIntegrity.Stable)
            {
                _state.State.NegativeVotes++;
                _state.State.Integrity = HostIntegrity.Testing;
                _state.State.ConsecutivePositiveVotes = 0;
                _state.State.DeactivatedUntil = DateTime.UtcNow + TimeSpan.FromSeconds(0.1 * Math.Pow(2, _state.State.NegativeVotes));
            }
            else if (_state.State.Integrity == HostIntegrity.Testing)
            {
                _state.State.NegativeVotes++;
                if (_state.State.NegativeVotes > _settings.MaxFailures)
                {
                    _state.State.Integrity = HostIntegrity.Faulty;
                    _state.State.NegativeVotes = 0;
                    _state.State.DeactivatedUntil = DateTime.UtcNow + TimeSpan.FromHours(_settings.FaultyServerPeriodInHours);
                }
                else
                {
                    _state.State.DeactivatedUntil = DateTime.UtcNow + TimeSpan.FromSeconds(0.1 * Math.Pow(2, _state.State.NegativeVotes));
                }
            }
            else if (_state.State.Integrity == HostIntegrity.Faulty)
            {
                _state.State.NegativeVotes++;
                if (_state.State.NegativeVotes > _settings.MaxFaultyFailures)
                {
                    _state.State.Integrity = HostIntegrity.Dead;
                    _state.State.NegativeVotes = 0;
                }
            }

            _dirty = true;
            return Task.CompletedTask;
        }

        public Task VoteFor()
        {
            if (_state.State.Integrity == HostIntegrity.Stable)
            {
                _state.State.NegativeVotes = 0;
            }
            else if (_state.State.Integrity == HostIntegrity.Testing)
            {
                _state.State.ConsecutivePositiveVotes++;
                if (_state.State.ConsecutivePositiveVotes >= _settings.MinPasses)
                {
                    _state.State.NegativeVotes = 0;
                    _state.State.ConsecutivePositiveVotes = 0;
                    _state.State.Integrity = HostIntegrity.Stable;
                }
            }
            else if (_state.State.Integrity == HostIntegrity.Faulty)
            {
                _state.State.ConsecutivePositiveVotes++;
                if (_state.State.ConsecutivePositiveVotes >= _settings.MinFaultyPasses)
                {
                    _state.State.NegativeVotes = 0;
                    _state.State.ConsecutivePositiveVotes = 0;
                    _state.State.Integrity = HostIntegrity.Testing;
                }
            }

            _dirty = true;
            return Task.CompletedTask;
        }
    }
}
