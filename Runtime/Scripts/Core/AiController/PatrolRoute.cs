using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class PatrolRoute : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Patrol Settings")] [SerializeField] private Transform[] patrolPoints;
        [BoxGroup("Patrol Settings")] [SerializeField] private bool reverseOnReturn;
        [BoxGroup("Patrol Settings")] [SerializeField] private float speed;
        [BoxGroup("Patrol Settings")] [SerializeField] private float minPause;
        [BoxGroup("Patrol Settings")] [SerializeField] private float maxPause;
        public float Speed => speed;
        public float MinPause => minPause;
        public float MaxPause => maxPause;
        public Transform[] PatrolPoints => patrolPoints;
        public int NumberOfPatrolPoints => patrolPoints.Length;
        private int _currentPatrolIndex;

        #endregion

        #region Startup

        private void Start()
        {
            if (NumberOfPatrolPoints < 1)
            {
                Debug.LogError($"PatrolRoute: There are no patrol points for GameObject {gameObject.name}");
            }
        }

        #endregion

        #region Class Methods

        public Transform GetPatrolPointAtIndex(int patrolPointIndex)
        {
            return patrolPoints[patrolPointIndex];
        }

        public Transform GetFirstDestination()
        {
            _currentPatrolIndex = 0;
            return GetNextDestination();
        }

        public Transform GetNextDestination()
        {
            _currentPatrolIndex = _currentPatrolIndex >= NumberOfPatrolPoints - 1 ? 0 : _currentPatrolIndex + 1;
            return patrolPoints[_currentPatrolIndex];
        }

        public Transform GetCurrentDestination()
        {
            return patrolPoints[_currentPatrolIndex];
        }

        #endregion
    }
}