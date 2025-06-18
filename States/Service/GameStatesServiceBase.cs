using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.DiContainer;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    public abstract class GameStatesServiceBase<TEnum> : Service, IGameStatesService<TEnum> where TEnum : Enum
    {
        [SerializeField] private GameStatesConfigBase<TEnum> _config;

        private Dictionary<(TEnum from, TEnum to), IGameTransition> _transitionsDict;
        private Dictionary<TEnum, IGameState> _statesDict;
        
        private IDiContainer _diContainer;

        private (TEnum code, IGameState state) _currentState;

        [Inject]
        private void Construct(IDiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public async UniTask ChangeStateAsync(TEnum toStateCode, CancellationToken cancellationToken)
        {
            if (!_statesDict.TryGetValue(toStateCode, out var toState))
            {
                Debug.LogError($"No state with code {toStateCode}");
                return;
            }
            var (fromStateCode, fromState) = _currentState;
            var noFromState = fromState == null;
            
            var transition = noFromState ? _config.DefaultTransition 
                : _transitionsDict.GetValueOrDefault((fromStateCode, toStateCode), _config.DefaultTransition);

            _diContainer.Inject(transition);
            
            await transition.BeginAsync(fromState, toState, cancellationToken);

            _currentState = (toStateCode, toState);
            _diContainer.Inject(toState);
            await toState.InitializeAsync(cancellationToken);
            
            await transition.EndAsync(fromState, toState, cancellationToken);
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _statesDict = new Dictionary<TEnum, IGameState>(_config.States.Length);
            foreach (var state in _config.States)
            {
                _statesDict.Add(state.Code, state.GameState);
            }
            _transitionsDict = new Dictionary<(TEnum from, TEnum to), IGameTransition>(_config.Transitions.Length);
            foreach (var transition in _config.Transitions)
            {
                _transitionsDict[(transition.From, transition.To)] = transition.Transition;
            }
            return UniTask.CompletedTask;
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
