using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Services;
using Whaledevelop.Transitions;

namespace Whaledevelop
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameStatesService", fileName = "GameStatesService")]
    public class GameStatesService : Service, IGameStatesService
    {
        [SerializeReference]
        private IGameTransition _defaultTransition;

        [SerializeField]
        private List<TransitionEntry> _transitionEntries = new();

        private Dictionary<(GameState from, GameState to), IGameTransition> _transitions;

        [Inject] private IDiContainer _diContainer;

        private IGameState _currentState;

        [ShowInInspector, ReadOnly]
        private List<IGameState> _usedStates = new();

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _transitions = new();

            foreach (var entry in _transitionEntries)
            {
                if (entry.From != null && entry.To != null && entry.Transition != null)
                {
                    _transitions[(entry.From, entry.To)] = entry.Transition;
                }
            }
            return UniTask.CompletedTask;
        }

        protected override async UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var state in _usedStates)
            {
                await state.ReleaseAsync(cancellationToken);
            }
            _usedStates.Clear();
        }

        public async UniTask ChangeStateAsync(IGameState toState, CancellationToken cancellationToken)
        {
            var fromState = _currentState;
            var noFromState = fromState == null;
            
            var transition = noFromState ? _defaultTransition 
                : _transitions.GetValueOrDefault(((GameState)fromState, (GameState)toState), _defaultTransition);

            _diContainer.Inject(transition);
            
            await transition.BeginAsync(fromState, toState, cancellationToken);

            if (!noFromState)
            {
                await _currentState.DisableAsync(cancellationToken);
            }

            await SetStateAsync(toState, cancellationToken);
            
            await transition.EndAsync(fromState, toState, cancellationToken);
        }

        private async UniTask SetStateAsync(IGameState nextState, CancellationToken cancellationToken)
        {
            _currentState = nextState;
            if (!_usedStates.Contains(_currentState))
            {
                _usedStates.Add(_currentState);
                _diContainer.Inject(nextState);
                await nextState.InitializeAsync(cancellationToken);
            }
            else
            {
                await nextState.EnableAsync(cancellationToken);
            }
        }
    }
}
