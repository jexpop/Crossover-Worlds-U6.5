namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerFightingState : PlayerCombatingState
    {

        public PlayerFightingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

             HitTagged("Hit");

            StartAnimation(stateMachine.Player.AnimationData.FightingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            HitTagged("Untagged");

            StopAnimation(stateMachine.Player.AnimationData.FightingParameterHash);
        }

        public override void OnAnimationTransitionEvent()
        {
            if (stateMachine.ReusableData.comboFight >= 1)
            {
                stateMachine.ChangeState(stateMachine.Combo1State);
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