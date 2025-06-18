using System;
using UnityEngine;

namespace Whaledevelop.GameStates
{
    public abstract class GameStatesConfigBase<TEnum> : ScriptableObject where TEnum : Enum
    {
        [field: SerializeField] public StateEntry<TEnum>[] States { get; private set; }
        [field: SerializeField] public TransitionEntry<TEnum>[] Transitions { get; private set; }
        [field: SerializeReference] public IGameTransition DefaultTransition { get; private set; }
    }
}