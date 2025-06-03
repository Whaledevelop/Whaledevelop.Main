using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Reactive;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameStatesService", fileName = "GameStatesService")]
    public class GameStatesService : Service, IGameStatesService
    {
        [SerializeReference]
        private IGameTransition _defaultTransition;

        [SerializeField]
        private List<TransitionEntry> _transitionEntries = new();

        private Dictionary<(GameState from, GameState to), IGameTransition> _transitions;

        private IDiContainer _diContainer;

        [ShowInInspector, ReadOnly]
        private List<IGameState> _usedStates = new();

        public ReactiveValue<IGameState> CurrentState { get;  } = new();

        [Inject]
        public void Construct(IDiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
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
            var fromState = CurrentState.Value;
            var noFromState = fromState == null;
            
            var transition = noFromState ? _defaultTransition 
                : _transitions.GetValueOrDefault(((GameState)fromState, (GameState)toState), _defaultTransition);

            _diContainer.Inject(transition);
            
            await transition.BeginAsync(fromState, toState, cancellationToken);

            if (!noFromState)
            {
                await CurrentState.Value.DisableAsync(cancellationToken);
            }

            await SetStateAsync(toState, cancellationToken);
            
            await transition.EndAsync(fromState, toState, cancellationToken);
        }

        private async UniTask SetStateAsync(IGameState nextState, CancellationToken cancellationToken)
        {
            CurrentState.Value = nextState;
            if (!_usedStates.Contains(CurrentState.Value))
            {
                _usedStates.Add(CurrentState.Value);
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
