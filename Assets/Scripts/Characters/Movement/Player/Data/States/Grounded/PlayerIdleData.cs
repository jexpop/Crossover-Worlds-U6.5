using System;
using System.Collections.Generic;
using CrossoverWorlds.Characters.Movement.Player.Data.Cameras;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States.Grounded
{
    [Serializable]
    public class PlayerIdleData
    {
        [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
    }
}