using System;
using UnityEngine;

namespace Whaledevelop.GameStates
{
    [Serializable]
    public class StateEntry<TEnum> where TEnum : Enum
    {
        [field: SerializeField] public TEnum Code { get; private set; }
            
        [field: SerializeReference] public IGameState GameState { get; private set; }
    }
}