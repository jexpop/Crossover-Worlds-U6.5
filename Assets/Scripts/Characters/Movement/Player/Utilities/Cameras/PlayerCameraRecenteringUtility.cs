using System;

using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Utilities.Cameras
{
    [Serializable]
    public class PlayerCameraRecenteringUtility
    {
        [field: SerializeField] public Unity.Cinemachine.CinemachineVirtualCamera VirtualCamera { get; private set; }
        [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
        [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;

        private Unity.Cinemachine.CinemachinePOV cinemachinePOV;

        public void Initialize()
        {
            cinemachinePOV = VirtualCamera.GetCinemachineComponent<Unity.Cinemachine.CinemachinePOV>();
        }

        public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f, float baseMovementSpeed = 1f, float movementSpeed = 1f)
        {
            cinemachinePOV.m_HorizontalRecentering.m_enabled = true;

            cinemachinePOV.m_HorizontalRecentering.CancelRecentering();

            if (waitTime == -1f)
            {
                waitTime = DefaultHorizontalWaitTime;
            }

            if (recenteringTime == -1f)
            {
                recenteringTime = DefaultHorizontalRecenteringTime;
            }

            recenteringTime = recenteringTime * baseMovementSpeed / movementSpeed;

            cinemachinePOV.m_HorizontalRecentering.m_WaitTime = waitTime;
            cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
        }

        public void DisableRecentering()
        {
            cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
        }
    }
}