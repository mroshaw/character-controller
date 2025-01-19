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

        // Cached Character
        protected ThirdPersonCharacter TpCharacter { get; private set; }
        protected Animator Animator { get; private set; }

        protected float ForwardAmount { get; private set; }
        protected Vector3 MoveDirection { get; private set; }

        protected virtual void Awake()
        {
            // Cache our Character
            TpCharacter = GetComponent<ThirdPersonCharacter>();
        }

        protected virtual void Start()
        {
            // Get Character animator
            Animator = TpCharacter.GetAnimator();
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            // Compute input move vector in local space
            MoveDirection = transform.InverseTransformDirection(TpCharacter.GetMovementDirection());

            // Update the animator parameters
            ForwardAmount = TpCharacter.useRootMotion && TpCharacter.GetRootMotionController()
                ? MoveDirection.z
                : Mathf.InverseLerp(0.0f, TpCharacter.GetMaxSpeed(), TpCharacter.GetSpeed());

            Animator.SetFloat(Forward, ForwardAmount, 0.1f, deltaTime);
            Animator.SetFloat(Turn, Mathf.Atan2(MoveDirection.x, MoveDirection.z), 0.1f, deltaTime);
            Animator.SetBool(Ground, TpCharacter.IsGrounded());
            // Animator.SetBool(Roll, TpCharacter.IsRolling());
            Animator.SetBool(Crouch, TpCharacter.IsCrouched());
            Animator.SetBool(Swimming, TpCharacter.IsSwimming());

            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            float jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * ForwardAmount;

            if (TpCharacter.IsGrounded())
            {
                Animator.SetFloat(JumpLeg, jumpLeg);
            }
            if (TpCharacter.IsFalling())
            {
                Animator.SetFloat(Jump, TpCharacter.GetVelocity().y, 0.1f, deltaTime);
            }
        }
    }
}