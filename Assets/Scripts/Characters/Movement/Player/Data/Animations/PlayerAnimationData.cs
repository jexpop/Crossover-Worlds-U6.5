using System;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.Animations
{
    [Serializable]
    public class PlayerAnimationData
    {
        [Header("State Group Parameter Names")]
        [SerializeField] private string groundedParameterName = "Grounded";
        [SerializeField] private string movingParameterName = "Moving";
        [SerializeField] private string stoppingParameterName = "Stopping";
        [SerializeField] private string landingParameterName = "Landing";
        [SerializeField] private string airborneParameterName = "Airborne";
        [SerializeField] private string combatingParameterName = "Combating";

        [Header("Grounded Parameter Names")]
        [SerializeField] private string idleParameterName = "isIdling";
        [SerializeField] private string dashParameterName = "isDashing";
        [SerializeField] private string walkParameterName = "isWalking";
        [SerializeField] private string runParameterName = "isRunning";
        [SerializeField] private string sprintParameterName = "isSprinting";
        [SerializeField] private string mediumStopParameterName = "isMediumStopping";
        [SerializeField] private string hardStopParameterName = "isHardStopping";
        [SerializeField] private string rollParameterName = "isRolling";
        [SerializeField] private string lightLandParameterName = "isLightLanding";
        [SerializeField] private string hardLandParameterName = "isHardLanding";

        [Header("Airborne Parameter Names")]
        [SerializeField] private string fallParameterName = "isFalling";

        [Header("Combating Parameter Names")]
        [SerializeField] private string drawingParameterName = "isDrawing";
        [SerializeField] private string sheathingParameterName = "isSheathing";
        [SerializeField] private string fightingParameterName = "isFighting";
        [SerializeField] private string combo1ParameterName = "isCombo1";
        [SerializeField] private string combo2ParameterName = "isCombo2";

        public int GroundedParameterHash { get; private set; }
        public int MovingParameterHash { get; private set; }
        public int StoppingParameterHash { get; private set; }
        public int CombatingParameterHash { get; private set; }
        public int LandingParameterHash { get; private set; }
        public int AirborneParameterHash { get; private set; }

        public int IdleParameterHash { get; private set; }
        public int DashParameterHash { get; private set; }
        public int WalkParameterHash { get; private set; }
        public int RunParameterHash { get; private set; }
        public int SprintParameterHash { get; private set; }
        public int MediumStopParameterHash { get; private set; }
        public int HardStopParameterHash { get; private set; }
        public int RollParameterHash { get; private set; }
        public int LightLandParameterHash { get; private set; }
        public int HardLandParameterHash { get; private set; }

        public int FallParameterHash { get; private set; }

        public int DrawingParameterHash { get; private set; }
        public int SheathingParameterHash { get; private set; }
        public int FightingParameterHash { get; private set; }
        public int Combo1ParameterHash { get; private set; }
        public int Combo2ParameterHash { get; private set; }


        public void Initialize()
        {
            GroundedParameterHash = Animator.StringToHash(groundedParameterName);
            MovingParameterHash = Animator.StringToHash(movingParameterName);
            StoppingParameterHash = Animator.StringToHash(stoppingParameterName);
            CombatingParameterHash = Animator.StringToHash(combatingParameterName);
            LandingParameterHash = Animator.StringToHash(landingParameterName);
            AirborneParameterHash = Animator.StringToHash(airborneParameterName);

            IdleParameterHash = Animator.StringToHash(idleParameterName);
            DashParameterHash = Animator.StringToHash(dashParameterName);
            WalkParameterHash = Animator.StringToHash(walkParameterName);
            RunParameterHash = Animator.StringToHash(runParameterName);
            SprintParameterHash = Animator.StringToHash(sprintParameterName);
            MediumStopParameterHash = Animator.StringToHash(mediumStopParameterName);
            HardStopParameterHash = Animator.StringToHash(hardStopParameterName);
            RollParameterHash = Animator.StringToHash(rollParameterName);
            LightLandParameterHash = Animator.StringToHash(lightLandParameterName);
            HardLandParameterHash = Animator.StringToHash(hardLandParameterName);

            FallParameterHash = Animator.StringToHash(fallParameterName);

            DrawingParameterHash = Animator.StringToHash(drawingParameterName);
            SheathingParameterHash = Animator.StringToHash(sheathingParameterName);
            FightingParameterHash = Animator.StringToHash(fightingParameterName);
            Combo1ParameterHash= Animator.StringToHash(combo1ParameterName);
            Combo2ParameterHash = Animator.StringToHash(combo2ParameterName);
        }
    }
}