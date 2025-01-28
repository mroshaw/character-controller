using ECM2;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class Character : ECM2.Character
    {
        [BoxGroup("Sprinting")] public float maxSprintSpeed = 10.0f;
        [BoxGroup("Rolling")] public float minRollSpeed = 5.0f;
        [BoxGroup("Attack")] public float attackDamage = 5.0f;

        private bool _isSprinting;
        private bool _isRolling;
        private bool _isAttacking;
        private bool _isInteracting;

        private bool _sprintInputPressed;
        private bool _rollInputPressed;
        private bool _attackInputPressed;
        private bool _interactInputPressed;
        private float _cachedMaxWalkSpeed;
        private float _cachedRollSpeed;
        public float CachedRollSpeed => _cachedRollSpeed;
        /// <summary>
        /// Request the character to start to sprint.
        /// </summary>
        public void Sprint()
        {
            _sprintInputPressed = true;
        }

        public void Roll()
        {
            _rollInputPressed = true;
        }

        public void Attack()
        {
            _attackInputPressed = true;
        }

        public void Interact()
        {
            _interactInputPressed = true;
        }

        /// <summary>
        /// Request the character to stop sprinting.
        /// </summary>
        public void StopSprinting()
        {
            _sprintInputPressed = false;
        }

        public void StopRolling()
        {
            _rollInputPressed = false;
        }

        public void RollComplete()
        {
            _isRolling = false;
        }

        public void StopAttacking()
        {
            _attackInputPressed = false;
        }

        public void StopInteracting()
        {
            _interactInputPressed = false;
        }

        /// <summary>
        /// Return true if the character is sprinting, false otherwise.
        /// </summary>
        public bool IsSprinting()
        {
            return _isSprinting;
        }

        public bool IsRolling()
        {
            return _isRolling;
        }

        public bool IsAttacking()
        {
            return _isAttacking;
        }

        public bool IsInteracting()
        {
            return _isInteracting;
        }

        /// <summary>
        /// Determines if the character, is able to sprint in its current state.
        /// </summary>
        private bool CanSprint()
        {
            return IsWalking() && !IsCrouched();
        }

        private bool CanRoll()
        {
            return IsWalking() || IsCrouched();
        }

        private bool DoRoll()
        {
            _rollInputPressed = false;
            _cachedRollSpeed = GetMovementDirection().magnitude;
            return true;
        }

        private bool CanAttack()
        {
            return IsWalking();
        }

        private bool CanInteract()
        {
            return IsWalking();
        }

        protected override bool CanJump()
        {
            return !IsRolling() && base.CanJump();
        }

        /// <summary>
        /// Handles sprint input and adjusts character speed accordingly.
        /// </summary>
        private void CheckSprintInput()
        {
            if (!_isSprinting && _sprintInputPressed && CanSprint())
            {
                _isSprinting = true;

                _cachedMaxWalkSpeed = maxWalkSpeed;
                maxWalkSpeed = maxSprintSpeed;
            }
            else if (_isSprinting && (!_sprintInputPressed || !CanSprint()))
            {
                _isSprinting = false;

                maxWalkSpeed = _cachedMaxWalkSpeed;
            }
        }

        private void CheckRollInput()
        {
            if (!_rollInputPressed)
            {
                return;
            }

            bool didRoll = CanRoll() && DoRoll();
            _isRolling = didRoll;
        }

        private void CheckAttackInput()
        {
            if (!_isAttacking && _attackInputPressed && CanAttack())
            {
                _isAttacking = true;
            }
            else if (_isAttacking && (!_attackInputPressed || !CanAttack()))
            {
                _isAttacking = false;
            }
        }

        private void CheckInteractInput()
        {
            if (!_isInteracting && _interactInputPressed && CanInteract())
            {
                _isInteracting = true;
            }
            else if (_isInteracting && (!_interactInputPressed || !CanInteract()))

            {
                _isInteracting = false;
            }
        }

        public override float GetMaxSpeed()
        {
            return movementMode == MovementMode.Walking ? maxSprintSpeed : base.GetMaxSpeed();
        }

        protected override void OnBeforeSimulationUpdate(float deltaTime)
        {
            base.OnBeforeSimulationUpdate(deltaTime);
            // Handle sprinting
            CheckSprintInput();
            CheckRollInput();
            CheckAttackInput();
            CheckInteractInput();
        }
    }
}