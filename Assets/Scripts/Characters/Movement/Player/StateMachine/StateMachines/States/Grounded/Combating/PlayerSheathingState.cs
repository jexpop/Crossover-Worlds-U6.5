using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerSheathingState : PlayerCombatingState
    {
        public PlayerSheathingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.SheathingParameterHash);
            stateMachine.ReusableData.ShouldSheat = false;
            stateMachine.ReusableData.ShouldFight = false;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.SheathingParameterHash);
            stateMachine.ReusableData.ShouldDraw = true;
        }

        public override void OnAnimationEnterEvent()
        {
            // Sword animation            
            stateMachine.Player.DrawedWeapon.transform.GetChild(0).parent = stateMachine.Player.SheathedWeapon.transform;
            stateMachine.Player.SheathedWeapon.transform.GetChild(0).transform.localPosition = Vector3.zero;
            stateMachine.Player.SheathedWeapon.transform.GetChild(0).transform.localRotation = Quaternion.identity;
        }

        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
    }
}
