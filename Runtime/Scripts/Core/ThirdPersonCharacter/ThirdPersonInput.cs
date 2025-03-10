using DaftAppleGames.Gameplay;
using ECM2;
using UnityEngine;
using UnityEngine.InputSystem;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class ThirdPersonInput : CharacterInput, IPausable
    {
        [Space(15.0f)]
        public bool invertLook = true;

        [Tooltip("Look Sensitivity")]
        public Vector2 lookSensitivity = new Vector2(0.05f, 0.05f);

        [Tooltip("Zoom Sensitivity")]
        public float zoomSensitivity = 1.0f;

        [Space(15.0f)]
        [Tooltip("How far in degrees can you move the camera down.")]
        public float minPitch = -80.0f;

        [Tooltip("How far in degrees can you move the camera up.")]
        public float maxPitch = 80.0f;

        private bool _isPaused;

        /// <summary>
        /// Cached ThirdPersonCharacter.
        /// </summary>

        public ThirdPersonCharacter ThirdPersonCharacter { get; private set; }

        /// <summary>
        /// Movement InputAction.
        /// </summary>
        public InputAction LookInputAction { get; set; }

        /// <summary>
        /// Zoom InputAction.
        /// </summary>
        public InputAction ZoomInputAction { get; set; }

        /// <summary>
        /// Polls look InputAction (if any).
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>
        public Vector2 GetLookInput()
        {
            return LookInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// Polls zoom InputAction (if any).
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>
        public Vector2 GetZoomInput()
        {
            return ZoomInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// Initialize player InputActions (if any).
        /// E.g. Subscribe to input action events and enable input actions here.
        /// </summary>
        protected override void InitPlayerInput()
        {
            base.InitPlayerInput();

            // Look input action (no handler, this is polled, e.g. GetLookInput())
            LookInputAction = InputActionsAsset.FindAction("Look");
            LookInputAction?.Enable();

            // Zoom input action (no handler, this is polled, e.g. GetLookInput())
            ZoomInputAction = InputActionsAsset.FindAction("Zoom");
            ZoomInputAction?.Enable();
        }

        /// <summary>
        /// Unsubscribe from input action events and disable input actions.
        /// </summary>
        protected override void DeinitPlayerInput()
        {
            base.DeinitPlayerInput();

            // Unsubscribe from input action events and disable input actions

            if (LookInputAction != null)
            {
                LookInputAction.Disable();
                LookInputAction = null;
            }

            if (ZoomInputAction != null)
            {
                ZoomInputAction.Disable();
                ZoomInputAction = null;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            ThirdPersonCharacter = Character as ThirdPersonCharacter;
        }

        protected virtual void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void HandleInput()
        {
            // Don't process any input if paused.
            if (_isPaused)
            {
                return;
            }

            // Move
            Vector3 movementDirection = CalcMovementDirection(ThirdPersonCharacter.cameraTransform, ThirdPersonCharacter.GetUpVector());
            ThirdPersonCharacter.SetMovementDirection(movementDirection);

            // Look
            Vector2 lookInput = GetLookInput() * lookSensitivity;
            ThirdPersonCharacter.AddControlYawInput(lookInput.x);
            ThirdPersonCharacter.AddControlPitchInput(invertLook ? -lookInput.y : lookInput.y, minPitch, maxPitch);

            // Zoom
            Vector2 zoomInput = GetZoomInput() * zoomSensitivity;
            ThirdPersonCharacter.AddControlZoomInput(zoomInput.y);
        }

        void IPausable.Pause()
        {
            _isPaused = true;
        }

        void IPausable.Resume()
        {
            _isPaused = false;
        }

        bool IPausable.IsPaused()
        {
            return _isPaused;
        }
    }
}