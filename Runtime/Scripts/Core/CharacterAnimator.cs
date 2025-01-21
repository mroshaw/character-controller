using UnityEngine;

namespace DaftAppleGames.TpCharacterController
{
    public class CharacterAnimator : MonoBehaviour
    {
        // Cache Animator parameters
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Ground = Animator.StringToHash("OnGround");
        private static readonly int JumpLeg = Animator.StringToHash("JumpLeg");
        private static readonly int Roll = Animator.StringToHash("Roll");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Swimming = Animator.StringToHash("Swimming");
        private static readonly int Attack = Animator.StringToHash("Attack");

        // Cached Character
        private ThirdPersonCharacter ThirdPersonCharacter { get; set; }
        private CharacterAbilities CharacterAbilities { get; set; }
        private Animator Animator { get; set; }
        private float ForwardAmount { get; set; }
        private Vector3 MoveDirection { get; set; }

        private bool _hasAdditionalAbilities;

        protected virtual void Awake()
        {
            // Cache our Character
            ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            CharacterAbilities = GetComponent<CharacterAbilities>();
            _hasAdditionalAbilities = CharacterAbilities != null;
        }

        protected virtual void Start()
        {
            // Get Character animator
            Animator = ThirdPersonCharacter.GetAnimator();
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            // Compute input move vector in local space
            MoveDirection = transform.InverseTransformDirection(ThirdPersonCharacter.GetMovementDirection());

            // Update the animator parameters
            ForwardAmount = ThirdPersonCharacter.useRootMotion && ThirdPersonCharacter.GetRootMotionController()
                ? MoveDirection.z
                : Mathf.InverseLerp(0.0f, ThirdPersonCharacter.GetMaxSpeed(), ThirdPersonCharacter.GetSpeed());

            Animator.SetFloat(Forward, ForwardAmount, 0.1f, deltaTime);
            Animator.SetFloat(Turn, Mathf.Atan2(MoveDirection.x, MoveDirection.z), 0.1f, deltaTime);
            Animator.SetBool(Ground, ThirdPersonCharacter.IsGrounded());

            Animator.SetBool(Crouch, ThirdPersonCharacter.IsCrouched());
            Animator.SetBool(Swimming, ThirdPersonCharacter.IsSwimming());

            if (_hasAdditionalAbilities)
            {
                Animator.SetBool(Roll, CharacterAbilities.IsRolling());
                Animator.SetBool(Attack, CharacterAbilities.IsAttacking());
            }

            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            float jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * ForwardAmount;

            if (ThirdPersonCharacter.IsGrounded())
            {
                Animator.SetFloat(JumpLeg, jumpLeg);
            }

            if (ThirdPersonCharacter.IsFalling())
            {
                Animator.SetFloat(Jump, ThirdPersonCharacter.GetVelocity().y, 0.1f, deltaTime);
            }
        }
    }
}