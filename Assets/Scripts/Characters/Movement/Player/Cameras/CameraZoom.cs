
using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Player.Cameras
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] [Range(0f, 12f)] private float defaultDistance = 6f;
        [SerializeField] [Range(0f, 12f)] private float minimumDistance = 1f;
        [SerializeField] [Range(0f, 12f)] private float maximumDistance = 6f;

        [SerializeField] [Range(0f, 20f)] private float smoothing = 4f;
        [SerializeField] [Range(0f, 20f)] private float zoomSensitivity = 1f;

        private Unity.Cinemachine.CinemachineFramingTransposer framingTransposer;
        private Unity.Cinemachine.CinemachineInputProvider inputProvider;

        private float currentTargetDistance;

        private void Awake()
        {
            framingTransposer = GetComponent<Unity.Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Unity.Cinemachine.CinemachineFramingTransposer>();
            inputProvider = GetComponent<Unity.Cinemachine.CinemachineInputProvider>();

            currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;

            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minimumDistance, maximumDistance);

            float currentDistance = framingTransposer.m_CameraDistance;

            if (currentDistance == currentTargetDistance)
            {
                return;
            }

            float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);

            framingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }
}