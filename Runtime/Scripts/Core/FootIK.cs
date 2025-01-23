using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class FootIK : MonoBehaviour
    {
        #region Class fields

        [BoxGroup("Settings")] [SerializeField] private bool ikActive = true;
        [BoxGroup("Settings")] [SerializeField] [Range(0f, 1f)] private float weightPositionRight = 1f;
        [BoxGroup("Settings")] [SerializeField] [Range(0f, 1f)] private float weightRotationRight = 0f;
        [BoxGroup("Settings")] [SerializeField] [Range(0f, 1f)] private float weightPositionLeft = 1f;
        [BoxGroup("Settings")] [SerializeField] [Range(0f, 1f)] private float weightRotationLeft = 0f;
        [BoxGroup("Anim Settings")] [SerializeField] private Animator animator;
        [BoxGroup("Anim Settings")] [Tooltip("Offset for Foot position")] [SerializeField] private Vector3 offsetFoot;
        [BoxGroup("Anim Settings")] [Tooltip("Layer where foot can adjust to surface")] [SerializeField] private LayerMask rayMask;

        private RaycastHit _hit;

        #endregion

        #region Startup

        private void Start()
        {
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
        }

        #endregion

        #region Class methods

        private void OnAnimatorIK(int layerIndex)
        {
            if (ikActive)
            {
                RotateFoot(AvatarIKGoal.LeftFoot, weightPositionLeft, weightRotationLeft);
                RotateFoot(AvatarIKGoal.RightFoot, weightPositionRight, weightRotationRight);
            }
        }

        private void RotateFoot(AvatarIKGoal avatarFoot, float weightPosition, float weightRotation)
        {
            Vector3 footPos = animator.GetIKPosition(avatarFoot); // get current foot position (After animation apply)
            if (Physics.Raycast(footPos + Vector3.up, Vector3.down, out _hit, 1.2f, rayMask))
            {
                animator.SetIKPositionWeight(avatarFoot, weightPosition);
                animator.SetIKRotationWeight(avatarFoot, weightRotation);
                animator.SetIKPosition(avatarFoot, _hit.point + offsetFoot);

                if (weightRotation > 0f)
                {
                    // Calculate foot rotation
                    Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, _hit.normal), _hit.normal);
                    animator.SetIKRotation(avatarFoot, footRotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(avatarFoot, 0f);
                animator.SetIKRotationWeight(avatarFoot, 0f);
            }
        }

        #endregion
    }
}