using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerDrawingState : PlayerCombatingState
    {
        public PlayerDrawingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.DrawingParameterHash);

            stateMachine.ReusableData.ShouldSheat = true;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.DrawingParameterHash);
            stateMachine.ReusableData.ShouldDraw = false;
        }

        public override void OnAnimationEnterEvent()
        {
            // Sword animation            
            stateMachine.Player.SheathedWeapon.transform.GetChild(0).parent = stateMachine.Player.DrawedWeapon.transform;
            stateMachine.Player.DrawedWeapon.transform.GetChild(0).transform.localPosition = Vector3.zero;
        }

        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }

    }
}