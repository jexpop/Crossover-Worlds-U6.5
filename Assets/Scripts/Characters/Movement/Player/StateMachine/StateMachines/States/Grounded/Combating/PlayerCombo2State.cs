namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerCombo2State : PlayerCombatingState
    {
        public PlayerCombo2State(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

             HitTagged("Hit");

            StartAnimation(stateMachine.Player.AnimationData.Combo2ParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            HitTagged("Untagged");

            StopAnimation(stateMachine.Player.AnimationData.Combo2ParameterHash);
        }

        public override void OnAnimationTransitionEvent()
        {
            // Reset fight combos
            stateMachine.ReusableData.comboFight = -1;
            stateMachine.ChangeState(stateMachine.IdlingState);
        }

    }
}
