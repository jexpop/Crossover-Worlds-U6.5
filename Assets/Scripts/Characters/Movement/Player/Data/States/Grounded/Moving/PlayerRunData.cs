using System;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States.Grounded.Moving
{
    [Serializable]
    public class PlayerRunData
    {
        [field: SerializeField] [field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
    }
}