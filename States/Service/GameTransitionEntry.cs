using System;
using UnityEngine;

namespace Whaledevelop.GameStates
{
    [Serializable]
    public class GameTransitionEntry<TEnum> where TEnum : Enum
    {
        [field:SerializeField] public TEnum From { get; private set; }
        [field: SerializeField] public TEnum To { get; private set; }
        [field: SerializeReference] public IGameTransition Transition { get; private set; }
    }
}