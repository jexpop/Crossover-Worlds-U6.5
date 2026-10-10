using CrossoverWorlds.Characters.Movement.Player.Data.States.Airborne;
using CrossoverWorlds.Characters.Movement.Player.Data.States.Grounded;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Player", menuName = "Movement System/Characters/Player")]
    public class PlayerSO : ScriptableObject
    {
        [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
        [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }
    }
}