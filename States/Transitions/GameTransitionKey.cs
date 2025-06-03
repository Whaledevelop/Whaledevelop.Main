using System;
using UnityEngine;

namespace Whaledevelop.GameStates
{

    [Serializable]
    public class TransitionEntry
    {
        [SerializeField] private GameState _from;
        [SerializeField] private GameState _to;

        [SerializeReference] private IGameTransition _transition;

        public GameState From => _from;
        public GameState To => _to;
        public IGameTransition Transition => _transition;
    }
}