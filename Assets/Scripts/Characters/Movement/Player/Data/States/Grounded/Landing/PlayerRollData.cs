using System;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States.Grounded.Landing
{
    [Serializable]
    public class PlayerRollData
    {
        [field: SerializeField] [field: Range(0f, 3f)] public float SpeedModifier { get; private set; } = 1f;
    }
}