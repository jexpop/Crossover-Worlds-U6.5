using CrossoverWorlds.Characters.Movement.Player.Data.Colliders;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Utilities.Colliders
{
    public class PlayerResizableCapsuleCollider : ResizableCapsuleCollider
    {
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TriggerColliderData.Initialize();
        }
    }
}