using System;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController
{
    public enum MovementSpeed
    {
        Walking = 2,
        Running = 4,
        Sprinting = 6
    }

    [Serializable]
    public struct MoveSpeeds
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private MovementSpeed movementSpeed;

        public float GetMoveSpeed()
        {
            switch (movementSpeed)
            {
                case MovementSpeed.Walking:
                    return walkSpeed;
                case MovementSpeed.Running:
                    return runSpeed;
                case MovementSpeed.Sprinting:
                    return sprintSpeed;
                default:
                    return 0;
            }
        }

        public float GetSprintSpeed()
        {
            return sprintSpeed;
        }

        public float GetSprintSpeedDifference()
        {
            return sprintSpeed - walkSpeed;
        }
    }
}