using ECM2;
using Unity.Cinemachine;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class ThirdPersonCharacter : Character
    {
        [Header("Cinemachine")]
        [Tooltip("The Cinemachine Camera following this Character.")]
        public CinemachineCamera followCamera;

        [Tooltip("The target set in the Cinemachine Camera that the camera will follow.")]
        public GameObject followTarget;

        [Tooltip("The default distance behind the follow Character.")]
        [SerializeField]
        public float followDistance = 5.0f;

        [Tooltip("The minimum distance to follow Character.")]
        [SerializeField]
        public float followMinDistance = 2.0f;

        [Tooltip("The maximum distance to follow Character.")]
        [SerializeField]
        public float followMaxDistance = 10.0f;

        // Current followTarget yaw and pitch angles
        private float _cameraTargetYaw;
        private float _cameraTargetPitch;

        // Current follow distance
        private CinemachineThirdPersonFollow _cmThirdPersonFollow;
        protected float _followDistanceSmoothVelocity;

        /// <summary>
        /// Add input (affecting Yaw).
        /// This is applied to the followTarget's yaw rotation.
        /// </summary>
        public void AddControlYawInput(float value, float minValue = -180.0f, float maxValue = 180.0f)
        {
            if (value != 0.0f) _cameraTargetYaw = MathLib.ClampAngle(_cameraTargetYaw + value, minValue, maxValue);
        }

        /// <summary>
        /// Add input (affecting Pitch).
        /// This is applied to the followTarget's pitch rotation.
        /// </summary>
        public void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            if (value != 0.0f)
                _cameraTargetPitch = MathLib.ClampAngle(_cameraTargetPitch + value, minValue, maxValue);
        }

        /// <summary>
        /// Adds input (affecting follow distance).
        /// </summary>
        public virtual void AddControlZoomInput(float value)
        {
            followDistance = Mathf.Clamp(followDistance - value, followMinDistance, followMaxDistance);
        }

        /// <summary>
        /// Update followTarget rotation using _cameraTargetYaw and _cameraTargetPitch values and its follow distance.
        /// </summary>
        private void UpdateCamera()
        {
            followTarget.transform.rotation = Quaternion.Euler(_cameraTargetPitch, _cameraTargetYaw, 0.0f);

            _cmThirdPersonFollow.CameraDistance =
                Mathf.SmoothDamp(_cmThirdPersonFollow.CameraDistance, followDistance, ref _followDistanceSmoothVelocity, 0.1f);
        }

        protected override void Start()
        {
            base.Start();

            // Cache and init CinemachineThirdPersonFollow component
            _cmThirdPersonFollow = followCamera.GetComponent<CinemachineThirdPersonFollow>();
            if (_cmThirdPersonFollow)
            {
                _cmThirdPersonFollow.CameraDistance = followDistance;
            }
            else
            {
                Debug.LogError("Can't find CinemachineThirdPersonFollow!");
            }
        }

        private void LateUpdate()
        {
            UpdateCamera();
        }
    }
}