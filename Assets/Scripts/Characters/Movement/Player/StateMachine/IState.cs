using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.StateMachine
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void HandleInput();
        public void Update();
        public void PhysicsUpdate();
        public void OnCollisionEnter(Collision collision);
        public void OnCollisionExit(Collision collision);
        public void OnAnimationEnterEvent();
        public void OnAnimationExitEvent();
        public void OnAnimationTransitionEvent();
    }
}