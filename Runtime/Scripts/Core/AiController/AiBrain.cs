using System.Collections;
using DaftAppleGames.Extensions;
using ECM2;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public enum AiState
    {
        Idle,
        Wandering,
        Patrolling,
        Fleeing,
        Attacking,
    }

    /// <summary>
    /// Helper component for AI Behaviour features
    /// </summary>
    public abstract class AiBrain : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Patrol Settings")] [SerializeField] private PatrolRoute patrolRoute;
        [BoxGroup("Patrol Settings")] [SerializeField] private float patrolSpeed = 0.2f;
        [BoxGroup("Patrol Settings")] [SerializeField] private float patrolRotationRate = 40.0f;
        [BoxGroup("Patrol Settings")] [SerializeField] private float patrolMinPause = 5.0f;
        [BoxGroup("Patrol Settings")] [SerializeField] private float patrolMaxPause = 15.0f;

        [BoxGroup("Wander Settings")] [SerializeField] private float wanderSpeed = 0.2f;
        [BoxGroup("Wander Settings")] [SerializeField] private float wanderRotationRate = 40.0f;
        [BoxGroup("Wander Settings")] [SerializeField] private float wanderMinRange = 20.0f;
        [BoxGroup("Wander Settings")] [SerializeField] private float wanderMaxRange = 60.0f;
        [BoxGroup("Wander Settings")] [SerializeField] private float wanderMinPause = 0.0f;
        [BoxGroup("Wander Settings")] [SerializeField] private float wanderMaxPause = 10.0f;
        [BoxGroup("Wander Settings")] [SerializeField] private Transform wanderCenterTransform;

        [BoxGroup("Flee Settings")] [SerializeField] private float fleeSpeed = 0.9f;
        [BoxGroup("Flee Settings")] [SerializeField] private float fleeRotationRate = 80.0f;
        [BoxGroup("Flee Settings")] [SerializeField] private float fleeMinRange = 20.0f;
        [BoxGroup("Flee Settings")] [SerializeField] private float fleeMaxRange = 30.0f;
        [BoxGroup("Flee Settings")] [SerializeField] private float fleeRestTime = 30.0f;

        [BoxGroup("Needs")] [SerializeField] private float startingThirst = 100.0f;
        [BoxGroup("Needs")] [SerializeField] private float thirstRate = 0.01f;
        [BoxGroup("Needs")] [SerializeField] private WaterSource[] knownWaterSources;

        [BoxGroup("Needs Debug")] [SerializeField] private float _thirst;

        private BlackboardReference _blackboardRef;

        #endregion

        #region Properties

        public NavMeshCharacter NavMeshCharacter { get; private set; }
        public Character Character { get; private set; }
        public DetectorManager DetectorManager { get; private set; }
        public Animator Animator { get; private set; }
        public AiState AiState { get; private set; }

        #endregion

        #region Startup

        protected virtual void Awake()
        {
            NavMeshCharacter = GetComponent<NavMeshCharacter>();
            Character = GetComponent<Character>();
            DetectorManager = GetComponent<DetectorManager>();
            Animator = GetComponent<Animator>();

            if (!wanderCenterTransform)
            {
                wanderCenterTransform = transform;
            }

            AiState = AiState.Idle;
            _thirst = startingThirst;
        }

        protected virtual void Start()
        {
        }

        #endregion

        #region Update

        protected virtual void Update()
        {
            if (_thirst > 0)
            {
                _thirst -= thirstRate * Time.deltaTime;
            }
            else
            {
                _thirst = 0.0f;
            }
        }

        #endregion

        #region Class methods

        #region Move methods

        public void SetMoveSpeed(float speed)
        {
            Character.maxWalkSpeed = speed;
        }

        public void SetRotationRate(float rotateRate)
        {
            Character.rotationRate = rotateRate;
        }

        public bool HasArrived()
        {
            return !NavMeshCharacter.agent.hasPath || NavMeshCharacter.agent.velocity.sqrMagnitude == 0.0f;
        }

        public void MoveTo(Vector3 position)
        {
            SetMoveSpeed(wanderSpeed);
            SetRotationRate(wanderRotationRate);
            NavMeshCharacter.MoveToDestination(position);
        }

        public void MoveTo(Vector3 position, float moveSpeed, float rotationRate)
        {
            SetMoveSpeed(moveSpeed);
            SetRotationRate(rotationRate);
            NavMeshCharacter.MoveToDestination(position);
        }

        public void MoveTo(Vector3 position, NavMeshCharacter.DestinationReachedEventHandler arrivalCallBack)
        {
            SetMoveSpeed(wanderSpeed);
            NavMeshCharacter.DestinationReached -= arrivalCallBack;
            NavMeshCharacter.DestinationReached += arrivalCallBack;
            NavMeshCharacter.MoveToDestination(position);
        }

        #endregion

        #region Patrol methods

        public bool HasPatrolRoute()
        {
            return patrolRoute != null;
        }

        [Button("Start Patrol")]
        public void Patrol()
        {
            if (!patrolRoute || AiState == AiState.Patrolling)
            {
                return;
            }

            NavMeshCharacter.DestinationReached -= ArrivedAtPatrolDestination;
            NavMeshCharacter.DestinationReached += ArrivedAtPatrolDestination;

            SetMoveSpeed(patrolSpeed);
            SetRotationRate(patrolRotationRate);

            NavMeshCharacter.MoveToDestination(patrolRoute.GetNextDestination().position);
            AiState = AiState.Patrolling;
        }

        private void StopPatrolling()
        {
            NavMeshCharacter.DestinationReached -= ArrivedAtPatrolDestination;
            AiState = AiState.Idle;
        }

        private void ArrivedAtPatrolDestination()
        {
            StartCoroutine(MoveToNextPatrolPointAfterDelayAsync());
        }

        private IEnumerator MoveToNextPatrolPointAfterDelayAsync()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(patrolMinPause, patrolMaxPause));
            NavMeshCharacter.MoveToDestination(patrolRoute.GetNextDestination().position);
        }

        #endregion

        #region Wander methods

        public void Wander()
        {
            if (AiState == AiState.Wandering)
            {
                return;
            }

            SetMoveSpeed(wanderSpeed);
            SetRotationRate(wanderRotationRate);
            NavMeshCharacter.DestinationReached += ArrivedAtWanderDestination;
            GoToRandomDestination();
            AiState = AiState.Wandering;
        }

        private void StopWandering()
        {
            NavMeshCharacter.StopMovement();
            NavMeshCharacter.DestinationReached -= ArrivedAtWanderDestination;
            AiState = AiState.Idle;
        }

        private void GoToRandomDestination()
        {
            Vector3 wanderLocation = GetRandomWanderLocation(wanderCenterTransform.position, wanderMinRange, wanderMaxRange);
            NavMeshCharacter.MoveToDestination(wanderLocation);
        }

        private Vector3 GetRandomWanderLocation(Vector3 center, float minDistance, float maxDistance)
        {
            Terrain.activeTerrain.GetRandomLocation(center, minDistance, maxDistance, out Vector3 location);
            return location;
        }

        private IEnumerator MoveToRandomPositionAfterDelayAsync()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(wanderMinPause, wanderMaxPause));
            GoToRandomDestination();
        }

        private void ArrivedAtWanderDestination()
        {
            StartCoroutine(MoveToRandomPositionAfterDelayAsync());
        }

        #endregion

        #region Flee methods

        public void Flee(Transform fleeFromTarget)
        {
            switch (AiState)
            {
                case AiState.Fleeing:
                    return;
                case AiState.Wandering:
                    StopWandering();
                    break;
            }

            SetMoveSpeed(fleeSpeed);
            SetRotationRate(fleeRotationRate);
            NavMeshCharacter.MoveToDestination(GetFleeDestination(fleeFromTarget));
            NavMeshCharacter.DestinationReached += ArrivedAtFleeDestination;
            AiState = AiState.Fleeing;
        }

        private void StopFleeing()
        {
            NavMeshCharacter.DestinationReached -= ArrivedAtFleeDestination;
            StartCoroutine(RestAfterFlee());
        }

        private IEnumerator RestAfterFlee()
        {
            yield return new WaitForSeconds(fleeRestTime);
            AiState = AiState.Idle;
        }

        private Vector3 GetFleeDestination(Transform targetTransform)
        {
            float fleeRange = Random.Range(fleeMinRange, fleeMaxRange);
            Vector3 fleePosition = transform.position + ((transform.position - targetTransform.position).normalized) * fleeRange;

            if (NavMesh.SamplePosition(fleePosition, out NavMeshHit myNavHit, 100, -1))
            {
                return myNavHit.position;
            }

            return fleePosition;
        }

        private void ArrivedAtFleeDestination()
        {
            StopFleeing();
        }

        #endregion

        #region Needs methods

        public bool IsThirsty()
        {
            return _thirst <= 0;
        }

        public bool Drink(float thirstValue)
        {
            _thirst = (_thirst + thirstValue) > startingThirst ? startingThirst : (_thirst + thirstValue);
            return Mathf.Approximately(_thirst, startingThirst);
        }

        [Button("Update Water Sources")]
        private void GetWaterSources()
        {
            knownWaterSources = FindObjectsByType<WaterSource>(FindObjectsSortMode.None);
        }

        public bool GetClosestWaterSource(out Transform closestWaterSource)
        {
            if (knownWaterSources == null || knownWaterSources.Length == 0)
            {
                closestWaterSource = null;
                return false;
            }

            float closestDistanceSqr = float.MaxValue;
            Vector3 playerPosition = transform.position;

            closestWaterSource = null;

            foreach (WaterSource waterSource in knownWaterSources)
            {
                if (waterSource == null)
                    continue;

                Vector3 directionToWaterSource = waterSource.transform.position - playerPosition;
                float distanceSqr = directionToWaterSource.sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestWaterSource = waterSource.transform;
                }
            }

            return closestWaterSource != null;
        }

        #endregion

        #endregion
    }
}