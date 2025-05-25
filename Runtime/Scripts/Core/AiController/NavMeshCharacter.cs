using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    public enum AiMoveSpeed
    {
        Walking,
        Running,
        Sprinting
    }

    public class NavMeshCharacter : ECM2.NavMeshCharacter
    {
        #region Properties

        [SerializeField] private float walkSpeed = 0.3f;
        [SerializeField] private float runSpeed = 0.5f;
        [SerializeField] private float sprintSpeed = 0.8f;

        private float _currSpeed;
        #endregion

        #region Class methods

        protected override void SyncNavMeshAgent()
        {
            base.SyncNavMeshAgent();
            agent.speed = _currSpeed;
        }

        internal void MoveToDestination(Vector3 destination, AiMoveSpeed moveSpeed, DestinationReachedEventHandler arrivalCallBack = null)
        {
            switch (moveSpeed)
            {
                case AiMoveSpeed.Walking:
                    _currSpeed = walkSpeed;
                    break;
                case AiMoveSpeed.Running:
                    _currSpeed = runSpeed;
                    break;

                case AiMoveSpeed.Sprinting:
                    _currSpeed = sprintSpeed;
                    break;
            }

            if (arrivalCallBack != null)
            {
                DestinationReached -= arrivalCallBack;
                DestinationReached += arrivalCallBack;
            }

            MoveToDestination(destination);
        }

        #endregion
    }
}