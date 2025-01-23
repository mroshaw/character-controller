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

        [Tooltip("Additional character abilities. If left unassigned, this will attempt to locate a Character component within this GameObject.")]
        [SerializeField]
        private CharacterAbilities characterAbilities;

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

        // Abilities InputAction
        public InputAction SprintInputAction { get; set; }
        public InputAction RollInputAction { get; set; }
        public InputAction AttackInputAction { get; set; }
        public InputAction InteractInputAction { get; set; }

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
        /// Sprint InputAction handler.
        /// </summary>
        public virtual void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
                characterAbilities.Sprint();
            else if (context.canceled)
                characterAbilities.StopSprinting();
        }

        public virtual void OnRoll(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                characterAbilities.Roll();
            }
            else if (context.canceled)
            {
                characterAbilities.StopRolling();
            }
        }

        public virtual void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
                characterAbilities.Attack();
            else if (context.canceled)
                characterAbilities.StopAttacking();
        }

        public virtual void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                characterAbilities.Interact();
            else if (context.canceled)
                characterAbilities.StopInteracting();
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

            // Abilities input action handlers
            SprintInputAction = inputActionsAsset.FindAction("Sprint");
            if (SprintInputAction != null)
            {
                SprintInputAction.started += OnSprint;
                SprintInputAction.canceled += OnSprint;

                SprintInputAction.Enable();
            }

            RollInputAction = inputActionsAsset.FindAction("Roll");
            if (RollInputAction != null)
            {
                RollInputAction.started += OnRoll;
                RollInputAction.canceled += OnRoll;

                RollInputAction.Enable();
            }

            AttackInputAction = inputActionsAsset.FindAction("Attack");
            if (AttackInputAction != null)
            {
                AttackInputAction.started += OnAttack;
                AttackInputAction.canceled += OnAttack;

                AttackInputAction.Enable();
            }

            InteractInputAction = inputActionsAsset.FindAction("Interact");
            if (InteractInputAction != null)
            {
                InteractInputAction.started += OnInteract;
                InteractInputAction.canceled += OnInteract;

                InteractInputAction.Enable();
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

            if (SprintInputAction != null)
            {
                SprintInputAction.started -= OnSprint;
                SprintInputAction.canceled -= OnSprint;

                SprintInputAction.Disable();
                SprintInputAction = null;
            }

            if (RollInputAction != null)
            {
                RollInputAction.started -= OnRoll;
                RollInputAction.canceled -= OnRoll;

                RollInputAction.Disable();
                RollInputAction = null;
            }

            if (AttackInputAction != null)
            {
                AttackInputAction.started -= OnAttack;
                AttackInputAction.canceled -= OnAttack;

                AttackInputAction.Disable();
                AttackInputAction = null;
            }

            if (InteractInputAction != null)
            {
                InteractInputAction.started -= OnInteract;
                InteractInputAction.canceled -= OnInteract;

                InteractInputAction.Disable();
                InteractInputAction = null;
            }
        }

        protected Vector3 CalcMovementDirection(Transform relativeTransform, Vector3 relativeVector)
        {
            // Poll movement InputAction
            Vector2 movementInput = GetMovementInput();
            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;

            // If character has a camera assigned...
            if (relativeTransform)
            {
                // Make movement direction relative to its camera view direction
                movementDirection = movementDirection.relativeTo(relativeTransform, relativeVector);
            }

            // Handle rolling
            if (characterAbilities.IsRolling() && !character.useRootMotion)
            {
                // Check if character is not moving, and set a direction and velocity
                float movementMagnitude = movementDirection.magnitude;
                if (character.GetMovementDirection().magnitude <= 0.01f)
                {
                    movementDirection = character.transform.forward *  characterAbilities.minRollSpeed;
                }
                // If the character was moving when rolling, we want at least the minimum roll velocity
                else if (character.GetMovementDirection().magnitude <= characterAbilities.minRollSpeed)
                {
                    movementDirection *= characterAbilities.minRollSpeed;
                }
            }

            return movementDirection;
        }

        protected virtual void HandleInput()
        {
            // Should this character handle input ?
            if (InputActionsAsset == null)
                return;

            // Poll movement InputAction
            Vector2 movementInput = GetMovementInput();
            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * movementInput.x;
            movementDirection += Vector3.forward * movementInput.y;

            // If character has a camera assigned...
            if (character.camera)
            {
                // Make movement direction relative to its camera view direction
                movementDirection = CalcMovementDirection(character.cameraTransform, character.GetUpVector());
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

            if (characterAbilities == null)
            {
                characterAbilities = GetComponent<CharacterAbilities>();
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