using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class CharacterAbilities : MonoBehaviour
    {
        [BoxGroup("Sprinting")] public float maxSprintSpeed = 10.0f;
        [BoxGroup("Rolling")] public float minRollSpeed = 5.0f;
        [BoxGroup("Attack")] public float attackDamage = 5.0f;

        private Character _character;

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
            return _character.IsWalking() && !_character.IsCrouched();
        }

        private bool CanRoll()
        {
            return _character.IsWalking() || _character.IsCrouched();
        }

        private bool DoRoll()
        {
            _rollInputPressed = false;
            _cachedRollSpeed = _character.GetMovementDirection().magnitude;
            return true;
        }

        private bool CanAttack()
        {
            return _character.IsWalking();
        }

        private bool CanInteract()
        {
            return _character.IsWalking();
        }

        /// <summary>
        /// Handles sprint input and adjusts character speed accordingly.
        /// </summary>
        private void CheckSprintInput()
        {
            if (!_isSprinting && _sprintInputPressed && CanSprint())
            {
                _isSprinting = true;

                _cachedMaxWalkSpeed = _character.maxWalkSpeed;
                _character.maxWalkSpeed = maxSprintSpeed;
            }
            else if (_isSprinting && (!_sprintInputPressed || !CanSprint()))
            {
                _isSprinting = false;

                _character.maxWalkSpeed = _cachedMaxWalkSpeed;
            }
        }

        private void CheckRollInput()
        {
            if (!_rollInputPressed)
            {
                return;
            }

            bool didRoll = CanRoll() && DoRoll();
            if(didRoll)
            {
                _isRolling = true;
            }
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

        private void OnBeforeSimulationUpdated(float deltaTime)
        {
            // Handle sprinting
            CheckSprintInput();
            CheckRollInput();
            CheckAttackInput();
            CheckInteractInput();
        }

        private void Awake()
        {
            // Cache character
            _character = GetComponent<Character>();
        }

        private void OnEnable()
        {
            // Subscribe to Character BeforeSimulationUpdated event
            _character.BeforeSimulationUpdated += OnBeforeSimulationUpdated;
        }

        private void OnDisable()
        {
            // Un-Subscribe from Character BeforeSimulationUpdated event
            _character.BeforeSimulationUpdated -= OnBeforeSimulationUpdated;
        }
    }
}