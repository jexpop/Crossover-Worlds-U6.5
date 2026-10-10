using System.Collections.Generic;
using CrossoverWorlds.Characters.Movement.Player.Data.Cameras;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.States
{
    public class PlayerStateReusableData
    {
        public Vector2 MovementInput { get; set; }

        public float MovementSpeedModifier { get; set; } = 1f;
        public float MovementOnSlopesSpeedModifier { get; set; } = 1f;
        public float MovementDecelerationForce { get; set; } = 1f;

        public List<PlayerCameraRecenteringData> SidewaysCameraRecenteringData { get; set; }
        public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; set; }

        public bool ShouldWalk { get; set; }
        public bool ShouldSprint { get; set; }
        public bool ShouldDraw { get; set; } = true;
        public bool ShouldSheat { get; set; } = false;
        public bool ShouldFight { get; set; } = false;

        public int comboFight = -1; // Counter for the combos
        public float timeFight = 1f; // Time between draw and fight


        private Vector3 currentTargetRotation;
        private Vector3 timeToReachTargetRotation;
        private Vector3 dampedTargetRotationCurrentVelocity;
        private Vector3 dampedTargetRotationPassedTime;

        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargetRotation;
            }
        }

        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }
        }

        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }
        }

        public ref Vector3 DampedTargetRotationPassedTime
        {
            get
            {
                return ref dampedTargetRotationPassedTime;
            }
        }

        public Vector3 CurrentJumpForce { get; set; }

        public PlayerRotationData RotationData { get; set; }
    }
}