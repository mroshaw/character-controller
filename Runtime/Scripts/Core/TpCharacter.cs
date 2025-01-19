using ECM2;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController
{
    public class TpCharacter : Character, IDamageable
    {
        [PropertyOrder(-1)][BoxGroup("Movement Settings")][SerializeField] private float sprintSpeed;
        [PropertyOrder(-1)][BoxGroup("Movement Settings")][Tooltip("Initial forward velocity when rolling.")][SerializeField] private float rollImpulse;

        [PropertyOrder(-1)][BoxGroup("Stats")][SerializeField] private float maxHealth = 100f;
        [PropertyOrder(-1)][BoxGroup("Stats")][SerializeField] private float maxStamina = 100f;
        [PropertyOrder(-1)][BoxGroup("Stats")][SerializeField] private float currentHealth = 100f;

        [FoldoutGroup("Events")] public UnityEvent crouchStartEvent;
        [FoldoutGroup("Events")] public UnityEvent crouchEndEvent;

        [BoxGroup("Debug")] [ShowInInspector] private bool _isSwimmingDebug;

        public float CurrentHealth => currentHealth;

        private bool _isAttacking;
        private bool _attackInputPressed;
        private bool _isSprinting;
        private bool _sprintInputPressed;

        private Vector3 _rollMovementDirection = Vector3.zero;

        private bool _isRolling;
        private bool _rollInputPressed;

        #region Startup
        protected override void OnEnable()
        {
            base.OnEnable();
            MovementModeChanged += ToggleRootMotion;
            Crouched += CrouchStarted;
            UnCrouched += CrouchEnded;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MovementModeChanged -= ToggleRootMotion;
            Crouched -= CrouchStarted;
            UnCrouched -= CrouchEnded;
        }
        #endregion

        #if UNITY_EDITOR
        #region Update methods - DEBUG ONLY!

        private void Update()
        {
            _isSwimmingDebug = IsSwimming();
        }
        #endregion
        #endif

        #region Movement methods

        private void CrouchStarted()
        {
            crouchStartEvent.Invoke();
        }

        private void CrouchEnded()
        {
            crouchEndEvent.Invoke();
        }
        public void SetMovementVelocity(Vector3 moveInput)
        {
            // Handle sprinting
            if (!IsSprinting() && useRootMotion)
            {
                if (moveInput.magnitude > 1.0f - sprintSpeed)
                {
                    moveInput = (moveInput.magnitude - sprintSpeed) * moveInput.normalized;
                }
            }

            // Compose a movement direction vector in world space
            Vector3 movementDirection = Vector3.zero;
            movementDirection += Vector3.forward * moveInput.y;
            movementDirection += Vector3.right * moveInput.x;
            movementDirection += Vector3.up * moveInput.z;

            // If character has a camera assigned,
            // make movement direction relative to this camera view direction
            if (camera)
            {
                movementDirection
                    = movementDirection.relativeTo(cameraTransform);
            }

            SetMovementDirection(movementDirection);
        }
        #endregion
        #region Jump methods
        public override void Jump()
        {
            // Set to 'Flying' to force the Jump to not use root motion anims
            // SetMovementMode(MovementMode.Flying);
            base.Jump();
        }

        public override void StopJumping()
        {
            base.StopJumping();
            // No need to set MovementMode, should return to 'Walking' when Grounded.
        }
        #endregion
        #region Roll methods
        public bool IsRolling()
        {
            return _isRolling;
        }
        private bool CanRoll()
        {
            return IsWalking() || IsCrouched();
        }

        public void Roll()
        {
            _rollInputPressed = true;
        }

        public void StopRolling()
        {
            _isRolling = false;
            // useRootMotion = false;
        }

        public void StopRollingInput()
        {
            _rollInputPressed = false;
        }

        protected void CheckRollInput()
        {
            if (!_rollInputPressed || _isRolling)
                return;

            if (CanRoll())
            {
                /*
                _rollMovementDirection = GetMovementDirection();
                if (_rollMovementDirection == Vector3.zero)
                {
                    _rollMovementDirection = transform.forward * rollSpeed;
                }
                */
                DoRoll();
                /*
                useRootMotion = true;
                */
                _isRolling = true;

            }
        }

        private void DoRoll()
        {
            float forwardSpeed = Mathf.Max(Vector3.Dot(characterMovement.velocity, transform.forward), rollImpulse);

            Vector3 newVelocity = Vector3.ProjectOnPlane(characterMovement.velocity, transform.forward) + transform.forward * forwardSpeed;

            characterMovement.velocity =
               newVelocity;
        }

        private void HandlingRolling()
        {
            if (!_isRolling)
            {
                return;
            }
            SetMovementDirection(_rollMovementDirection);
        }
        #endregion
        #region Sprint methods
        public bool IsSprinting()
        {
            return _isSprinting;
        }

        protected virtual bool CanSprint()
        {
            return IsWalking() && !IsCrouched();
        }

        public void Sprint()
        {
            _sprintInputPressed = true;
        }

        public void StopSprinting()
        {
            _sprintInputPressed = false;
        }

        private void CheckSprintInput()
        {
            if (!_isSprinting && _sprintInputPressed && CanSprint())
            {
                _isSprinting = true;
            }
            else if (_isSprinting && (!_sprintInputPressed || !CanSprint()))
            {
                _isSprinting = false;
            }
        }
        public float GetMaxAttainableSpeed()
        {
            return sprintSpeed > GetMaxSpeed() ? sprintSpeed : GetMaxSpeed();
        }
        #endregion
        #region Attack methods
        public bool IsAttacking()
        {
            return _isAttacking;
        }

        public void Attack()
        {
            _attackInputPressed = true;
        }

        public void StopAttacking()
        {
            _attackInputPressed = false;
        }

        public void CheckAttackInput()
        {
            if (!_isAttacking && _attackInputPressed && CanAttack())
            {
                _isAttacking = true;
            }
            if (_isAttacking && (!_attackInputPressed || !CanAttack()))
            {
                _isAttacking = false;
            }
        }
        #endregion
        #region Stats methods
        public bool IsDead( )
        {
            return currentHealth <= 0;
        }
        public void TakeDamage(float damage)
        {
            currentHealth = currentHealth - damage < 0 ? 0 : currentHealth - damage;
        }

        public void RestoreHealth(float health)
        {
            currentHealth = currentHealth + health > maxHealth ? maxHealth : currentHealth + health;
        }

        #endregion
        #region Character Overrides
        protected override void OnBeforeSimulationUpdate(float deltaTime)
        {
            // Call base method implementation
            base.OnBeforeSimulationUpdate(deltaTime);
            // Handle attacking
            CheckAttackInput();
            // Handle rolling
            CheckRollInput();
            // HandlingRolling();
            CheckSprintInput();
        }
        public override float GetMaxSpeed()
        {
            return _isSprinting ? sprintSpeed : base.GetMaxSpeed();
        }
        protected virtual bool CanAttack()
        {
            return !IsRolling() && IsWalking() && !IsCrouched(); ;
        }
        private void ToggleRootMotion(MovementMode prevMovementMode, int prevCustomMovementMode)
        {
            useRootMotion = IsWalking();
        }
        #endregion


    }
}