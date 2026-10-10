using System;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States
{
    [Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
    }
}