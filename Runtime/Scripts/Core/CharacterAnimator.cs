using DaftAppleGames.Attributes;
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
        private static readonly int Roll = Animator.StringToHash("Rolling");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Swimming = Animator.StringToHash("Swimming");
        private static readonly int Attack = Animator.StringToHash("Attacking");
        private static readonly int AttackId = Animator.StringToHash("AttackID");
        private static readonly int InAction = Animator.StringToHash("InAction");
        private static readonly int ActionID = Animator.StringToHash("ActionID");

        // Cached Character
        private Character Character { get; set; }
        private Animator Animator { get; set; }
        private float ForwardAmount { get; set; }
        private Vector3 MoveDirection { get; set; }

        protected virtual void Start()
        {
            // Cache our Character
            Character = GetComponent<Character>();
            // Get Character animator
            Animator = Character.GetAnimator();
   }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            // Compute input move vector in local space
            MoveDirection = transform.InverseTransformDirection(Character.GetMovementDirection());

            // Update the animator parameters
            ForwardAmount = Character.useRootMotion && Character.GetRootMotionController()
                ? MoveDirection.z
                : Mathf.InverseLerp(0.0f, Character.GetMaxSpeed(), Character.GetSpeed());

            Animator.SetFloat(Forward, ForwardAmount, 0.1f, deltaTime);
            Animator.SetFloat(Turn, Mathf.Atan2(MoveDirection.x, MoveDirection.z), 0.1f, deltaTime);
            Animator.SetBool(Ground, Character.IsGrounded());

            Animator.SetBool(Crouch, Character.IsCrouched());
            Animator.SetBool(Swimming, Character.IsSwimming());

            Animator.SetBool(Roll, Character.IsRolling());
            Animator.SetBool(Attack, Character.IsAttacking());
            Animator.SetInteger(AttackId, 100);
            Animator.SetInteger(ActionID, 100);

            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            float jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * ForwardAmount;

            if (Character.IsGrounded())
            {
                Animator.SetFloat(JumpLeg, jumpLeg);
            }

            if (Character.IsFalling())
            {
                Animator.SetFloat(Jump, Character.GetVelocity().y, 0.1f, deltaTime);
            }
        }
    }
}