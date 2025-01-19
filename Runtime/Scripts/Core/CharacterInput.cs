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
    public class CharacterInput : MonoBehaviour
    {
        [Space(15f)]
        [Tooltip("Collection of input action maps and control schemes available for user controls.")]
        [SerializeField]
        private InputActionAsset inputActionsAsset;

        [Tooltip("The character to be controlled. If left unassigned, this will attempt to locate a Character component within this GameObject.")]
        [SerializeField]
        private Character character;

        /// <summary>
        /// Our controlled character.
        /// </summary>

        public Character Character => character;

        /// <summary>
        /// InputActions assets.
        /// </summary>

        public InputActionAsset InputActionsAsset
        {
            get => inputActionsAsset;
            set
            {
                DeinitPlayerInput();

                inputActionsAsset = value;
                InitPlayerInput();
            }
        }

        /// <summary>
        /// Movement InputAction.
        /// </summary>

        public InputAction MovementInputAction { get; set; }

        /// <summary>
        /// Jump InputAction.
        /// </summary>

        public InputAction JumpInputAction { get; set; }

        /// <summary>
        /// Crouch InputAction.
        /// </summary>

        public InputAction CrouchInputAction { get; set; }

        /// <summary>
        /// Polls movement InputAction (if any).
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>

        public virtual Vector2 GetMovementInput()
        {
            return MovementInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// Jump InputAction handler.
        /// </summary>

        public virtual void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                character.Jump();
            else if (context.canceled)
                character.StopJumping();
        }

        /// <summary>
        /// Crouch InputAction handler.
        /// </summary>

        public virtual void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
                character.Crouch();
            else if (context.canceled)
                character.UnCrouch();
        }

        /// <summary>
        /// Initialize player InputActions (if any).
        /// E.g. Subscribe to input action events and enable input actions here.
        /// </summary>

        protected virtual void InitPlayerInput()
        {
            // Attempts to cache Character InputActions (if any)

            if (InputActionsAsset == null)
                return;

            // Movement input action (no handler, this is polled, e.g. GetMovementInput())

            MovementInputAction = InputActionsAsset.FindAction("Move");
            MovementInputAction?.Enable();

            // Setup Jump input action handlers

            JumpInputAction = InputActionsAsset.FindAction("Jump");
            if (JumpInputAction != null)
            {
                JumpInputAction.started += OnJump;
                JumpInputAction.canceled += OnJump;

                JumpInputAction.Enable();
            }

            // Setup Crouch input action handlers

            CrouchInputAction = InputActionsAsset.FindAction("Crouch");
            if (CrouchInputAction != null)
            {
                CrouchInputAction.started += OnCrouch;
                CrouchInputAction.canceled += OnCrouch;

                CrouchInputAction.Enable();
            }
        }

        /// <summary>
        /// Unsubscribe from input action events and disable input actions.
        /// </summary>

        protected virtual void DeinitPlayerInput()
        {
            // Unsubscribe from input action events and disable input actions

            if (MovementInputAction != null)
            {
                MovementInputAction.Disable();
                MovementInputAction = null;
            }

            if (JumpInputAction != null)
            {
                JumpInputAction.started -= OnJump;
                JumpInputAction.canceled -= OnJump;

                JumpInputAction.Disable();
                JumpInputAction = null;
            }

            if (CrouchInputAction != null)
            {
                CrouchInputAction.started -= OnCrouch;
                CrouchInputAction.canceled -= OnCrouch;

                CrouchInputAction.Disable();
                CrouchInputAction = null;
            }
        }

        protected virtual void HandleInput()
        {
            // Should this character handle input ?

            if (InputActionsAsset == null)
                return;

            // Poll movement InputAction

            Vector2 movementInput = GetMovementInput();

            Vector3 movementDirection =  Vector3.zero;

            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;

            // If character has a camera assigned...

            if (character.camera)
            {
                // Make movement direction relative to its camera view direction

                movementDirection = movementDirection.relativeTo(character.cameraTransform, character.GetUpVector());
            }

            // Set character's movement direction vector

            character.SetMovementDirection(movementDirection);
        }

        protected virtual void Awake()
        {
            // If no character assigned, try to get Character from this GameObject

            if (character == null)
            {
                character = GetComponent<Character>();
            }
        }

        protected virtual void OnEnable()
        {
            InitPlayerInput();
        }

        protected virtual void OnDisable()
        {
            DeinitPlayerInput();
        }

        protected virtual void Update()
        {
            HandleInput();
        }
    }
}