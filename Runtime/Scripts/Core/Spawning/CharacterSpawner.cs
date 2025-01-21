using DaftAppleGames.TpCharacterController.FootSteps;
using ECM2;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public abstract class CharacterSpawner : MonoBehaviour
    {
        #region Class Variables

        [PropertyOrder(-1)] [BoxGroup("Settings")] [SerializeField] protected CharacterSpawnerSettings spawnSettings;
        [PropertyOrder(2)] [BoxGroup("Behaviour")] [SerializeField] protected bool spawnOnStart;
        [PropertyOrder(3)] [BoxGroup("Character")] [SerializeField] private bool enableFootsteps;
        [PropertyOrder(3)] [BoxGroup("Character")] [SerializeField] private string characterInstanceName = "Player Character";
        [PropertyOrder(3)] [BoxGroup("Character")] [SerializeField] private string footstepPoolsInstanceName = "Footstep Pools";
        [PropertyOrder(3)] [BoxGroup("Character")] [SerializeField] protected Transform characterParentContainer;

        [PropertyOrder(10)]
        [FoldoutGroup("Events")] public UnityEvent CharacterSpawnedEvent;

        [PropertyOrder(99)] [FoldoutGroup("DEBUG")] [SerializeField] private GameObject characterGameObjectInstance;
        [PropertyOrder(99)] [FoldoutGroup("DEBUG")] [SerializeField] private GameObject footstepPoolsGameObjectInstance;

        protected Character character { get; private set; }
        protected bool IsValidSpawn { get; set; } = false;

        private FootstepManager _footstepManager;

        private PrefabPool[] _footstepPools;
        private PrefabPool _stepMarkPool;
        private PrefabPool _particlePool;

        #endregion

        #region Startup

        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        protected virtual void Awake()
        {
            if (!spawnSettings)
            {
                Debug.LogError($"CharacterSpawner: No spawn settings on Game Object: {gameObject}");
            }
        }

        private void OnEnable()
        {
            if (CharacterSpawnManager.Instance)
            {
                CharacterSpawnManager.Instance.RegisterNewSpawner(this);
            }
            else
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("CharacterSpawner: There is no CharacterSpawnManager in the scene! Please add one!");
                }
            }
        }

        private void OnDisable()
        {
            if (CharacterSpawnManager.Instance)
            {
                CharacterSpawnManager.Instance.DeregisterSpawner(this);
            }
        }

        /// <summary>
        /// Configure the component on start
        /// </summary>
        protected virtual void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        #endregion

        #region Class Methods

        protected virtual void Spawn()
        {
            if (SpawnInstances())
            {
                Configure();
                SetSpawnsActive();
                CharacterSpawnedEvent.Invoke();
            }
            else
            {
                Debug.LogError("CharacterSpawner: Spawning failed!");
            }
        }

        protected virtual bool SpawnInstances()
        {
            bool spawnStatus = true;

            characterGameObjectInstance =
                SpawnPrefab(spawnSettings.characterPrefab, transform.position, transform.rotation, characterParentContainer, characterInstanceName, false);
            character = characterGameObjectInstance.GetComponent<Character>();
            if (!character)
            {
                Debug.LogError($"CharacterSpawner: Failed to find Character component on PrefabInstance {spawnSettings.characterPrefab.name}");
                spawnStatus = false;
            }


            _footstepManager = characterGameObjectInstance.GetComponent<FootstepManager>();
            if (!_footstepManager && enableFootsteps)
            {
                Debug.LogError($"CharacterSpawner: Failed to find a FootstedManager component on PrefabInstance {spawnSettings.characterPrefab.name}");
                spawnStatus = false;
            }

            if (enableFootsteps)
            {
                footstepPoolsGameObjectInstance = SpawnPrefab(spawnSettings.footstepPoolsPrefab, Vector3.zero, Quaternion.identity, null, footstepPoolsInstanceName, false);
                _footstepPools = footstepPoolsGameObjectInstance.GetComponentsInChildren<PrefabPool>();
                if (_footstepPools.Length != 2)
                {
                    Debug.LogError($"CharacterSpawner: There should be 2 PrefabPools in PrefabInstance {spawnSettings.footstepPoolsPrefab.name}");
                    spawnStatus = false;
                }
                else
                {
                    foreach (PrefabPool currPool in _footstepPools)
                    {
                        if (currPool.PrefabPoolType == PrefabPoolType.FootstepMarks)
                        {
                            _stepMarkPool = currPool;
                        }

                        if (currPool.PrefabPoolType == PrefabPoolType.FootstepParticles)
                        {
                            _particlePool = currPool;
                        }
                    }

                    if (!_stepMarkPool || !_particlePool)
                    {
                        Debug.LogError(
                            $"CharacterSpawner: There should be 1 'FootstepMarks' and 1 'FootstepParticles' ParticlePool in PrefabInstance {spawnSettings.footstepPoolsPrefab.name}");
                        spawnStatus = false;
                    }
                }
            }

            IsValidSpawn = spawnStatus;
            return IsValidSpawn;
        }

        protected GameObject SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string instanceName, bool spawnActiveState)
        {
            prefab.SetActive(spawnActiveState);
#if UNITY_EDITOR
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            prefabInstance.transform.position = position;
            prefabInstance.transform.rotation = rotation;
            if (parent)
            {
                prefabInstance.transform.SetParent(parent);
            }
#else
            GameObject prefabInstance = parent ? Instantiate(prefab, position, rotation, parent) : Instantiate(prefab, position, rotation);
#endif
            if (string.IsNullOrEmpty(instanceName))
            {
                return prefabInstance;
            }

            prefabInstance.name = instanceName;
            return prefabInstance;
        }

        protected virtual void Configure()
        {
            Debug.Log("In CharacterSpawnerConfigure");
            if (!enableFootsteps)
            {
                if (_footstepManager)
                {
                    _footstepManager.footstepsEnabled = false;
                }

                return;
            }

            _footstepManager.SetParticlePool(_particlePool);
            _footstepManager.SetDecalPrefabPool(_stepMarkPool);
        }

        protected virtual void SetSpawnsActive()
        {
            if (enableFootsteps)
            {
                SetSpawnActive(footstepPoolsGameObjectInstance);
            }

            SetSpawnActive(characterGameObjectInstance);
        }

        protected void SetSpawnActive(GameObject spawnedGameObject)
        {
            if (!spawnedGameObject.activeSelf)
            {
                spawnedGameObject.SetActive(true);
            }
        }

        #endregion
    }
}