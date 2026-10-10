namespace CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines.States.Grounded.Combating
{
    public class PlayerCombatingState : PlayerGroundedState
    {
        public PlayerCombatingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Player.AnimationData.CombatingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.CombatingParameterHash);
        }

        public void HitTagged(string weaponTag)
        {
            stateMachine.Player.DrawedWeapon.transform.GetChild(0).tag=weaponTag;
        }

    }
}