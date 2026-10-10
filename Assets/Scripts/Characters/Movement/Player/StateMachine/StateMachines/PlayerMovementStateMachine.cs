using CrossoverWorlds.Characters.Movement.Player.Data.States;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Airborne;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Landing;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Moving;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Stopping;

namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public Player Player { get; }
        public PlayerStateReusableData ReusableData { get; }

        public PlayerIdlingState IdlingState { get; }
        public PlayerDashingState DashingState { get; }

        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }

        public PlayerLightStoppingState LightStoppingState { get; }
        public PlayerMediumStoppingState MediumStoppingState { get; }
        public PlayerHardStoppingState HardStoppingState { get; }

        public PlayerLightLandingState LightLandingState { get; }
        public PlayerRollingState RollingState { get; }
        public PlayerHardLandingState HardLandingState { get; }

        public PlayerJumpingState JumpingState { get; }
        public PlayerFallingState FallingState { get; }

        public PlayerDrawingState DrawingState { get; }
        public PlayerSheathingState SheathState { get; }
        public PlayerFightingState FightState { get; }
        public PlayerCombo1State Combo1State { get; }
        public PlayerCombo2State Combo2State { get; }


        public PlayerMovementStateMachine(Player player)
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();

            IdlingState = new PlayerIdlingState(this);
            DashingState = new PlayerDashingState(this);

            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);

            LightStoppingState = new PlayerLightStoppingState(this);
            MediumStoppingState = new PlayerMediumStoppingState(this);
            HardStoppingState = new PlayerHardStoppingState(this);

            LightLandingState = new PlayerLightLandingState(this);
            RollingState = new PlayerRollingState(this);
            HardLandingState = new PlayerHardLandingState(this);

            JumpingState = new PlayerJumpingState(this);
            FallingState = new PlayerFallingState(this);

            DrawingState = new PlayerDrawingState(this);
            SheathState = new PlayerSheathingState(this);
            FightState = new PlayerFightingState(this);
            Combo1State = new PlayerCombo1State(this);
            Combo2State = new PlayerCombo2State(this);
        }
    }
}