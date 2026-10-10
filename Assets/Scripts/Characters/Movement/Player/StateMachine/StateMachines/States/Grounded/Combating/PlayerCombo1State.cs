namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerCombo1State : PlayerCombatingState
    {
        public PlayerCombo1State(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

             HitTagged("Hit");

            StartAnimation(stateMachine.Player.AnimationData.Combo1ParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            HitTagged("Untagged");

            StopAnimation(stateMachine.Player.AnimationData.Combo1ParameterHash);
        }

        public override void OnAnimationTransitionEvent()
        {
            if (stateMachine.ReusableData.comboFight == 2)
            {
                stateMachine.ChangeState(stateMachine.Combo2State);
            }
            else
            {
                // Reset fight combos
                stateMachine.ReusableData.comboFight = -1;
                stateMachine.ChangeState(stateMachine.IdlingState);
            }
        }

    }
}
