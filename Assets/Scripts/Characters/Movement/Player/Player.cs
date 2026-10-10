using CrossoverWorlds.Characters.Movement.Player.Data.Animations;
using CrossoverWorlds.Characters.Movement.Player.Data.Layers;
using CrossoverWorlds.Characters.Movement.Player.Data.ScriptableObjects;
using CrossoverWorlds.Characters.Movement.Player.StateMachine.StateMachines;
using CrossoverWorlds.Characters.Movement.Player.Utilities.Cameras;
using CrossoverWorlds.Characters.Movement.Player.Utilities.Colliders;
using CrossoverWorlds.Characters.Movement.Player.Utilities.Inputs;
using CrossoverWorlds.unity_chan_.Unity_chan__Model.Scripts;
using MicahW.PointGrass;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player
{

    using PGDM = PointGrassDisplacementManager;

    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class Player : Singleton<Player>
    {
        [field: Header("References")]
        [field: SerializeField] public PlayerSO Data { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public GameObject GrassInteraction { get; private set; }
        
        [field: Header("Collisions")]
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public PlayerCameraRecenteringUtility CameraRecenteringUtility { get; private set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        [field: Header("Weapons")]
        [field: SerializeField] public GameObject DrawedWeapon { get; private set; }
        [field: SerializeField] public GameObject SheathedWeapon { get; private set; }


        public PlayerInput Input { get; private set; }
        public PlayerResizableCapsuleCollider ResizableCapsuleCollider { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;

        private void Awake()
        {
            CameraRecenteringUtility.Initialize();
            AnimationData.Initialize();

            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();

            Input = GetComponent<PlayerInput>();
            ResizableCapsuleCollider = GetComponent<PlayerResizableCapsuleCollider>();

            MainCameraTransform = Camera.main.transform;

            movementStateMachine = new PlayerMovementStateMachine(this);
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);

            // Weapons initialization
            DrawedWeapon = GameObject.FindGameObjectWithTag("DrawedWeapon");
            SheathedWeapon = GameObject.FindGameObjectWithTag("SheathedWeapon");

            // Grass interaction initialization
            GrassInteraction.AddComponent<PointGrassDisplacer>();
            PGDM.instance.AddDisplacer(GrassInteraction.GetComponent<PointGrassDisplacer>());

        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            movementStateMachine.OnCollisionEnter(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            movementStateMachine.OnCollisionExit(collision);
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }

        public void OnMovementStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        public void OnMovementStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }

        public void SetNewAnimator(Animator newAnimator)
        {
            Animator = newAnimator;
        }

        public void SetNewWeapons(GameObject drawedWeapon, GameObject sheathedWeapon)
        {
            DrawedWeapon = drawedWeapon;
            SheathedWeapon = sheathedWeapon;
        }

    }
}