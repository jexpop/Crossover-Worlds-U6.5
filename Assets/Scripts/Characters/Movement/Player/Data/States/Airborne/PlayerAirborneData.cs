using System;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States.Airborne
{
    [Serializable]
    public class PlayerAirborneData
    {
        [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
        [field: SerializeField] public PlayerFallData FallData { get; private set; }
    }
}