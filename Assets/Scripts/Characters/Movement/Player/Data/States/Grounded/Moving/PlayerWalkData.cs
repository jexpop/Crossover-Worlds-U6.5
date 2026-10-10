using System;
using System.Collections.Generic;
using CrossoverWorlds.Characters.Movement.Player.Data.Cameras;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States.Grounded.Moving
{
    [Serializable]
    public class PlayerWalkData
    {
        [field: SerializeField] [field: Range(0f, 1f)] public float SpeedModifier { get; private set; } = 0.225f;
        [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
    }
}