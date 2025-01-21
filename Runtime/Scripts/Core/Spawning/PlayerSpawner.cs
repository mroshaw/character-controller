using DaftAppleGames.TpCharacterController.PlayerController;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using Unity.Cinemachine;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class PlayerSpawner : CharacterSpawner
    {
        #region Class Variables

        [PropertyOrder(2)] [BoxGroup("Behaviour")] [SerializeField] protected bool disableExistingMainCamera;
        [PropertyOrder(3)] [BoxGroup("Player")] [SerializeField] private string cameraInstanceName = "Player Main Camera";
        [PropertyOrder(3)] [BoxGroup("Player")] [SerializeField] private string cinemachineRigInstanceName = "Player CM Camera Rig";

        [PropertyOrder(99)] [FoldoutGroup("DEBUG")] [SerializeField] private GameObject cameraGameObjectInstance;
        [PropertyOrder(99)] [FoldoutGroup("DEBUG")] [SerializeField] private GameObject cmRigGameObjectInstance;

        private PlayerSpawnerSettings _playerSpawnerSettings;

        private Camera _camera;
        private CinemachineCamera _cinemachine;
        private ThirdPersonCharacter _thirdPersonCharacter;

        #endregion

        #region Startup

        protected override void Awake()
        {
            base.Awake();
            _playerSpawnerSettings = spawnSettings as PlayerSpawnerSettings;
        }

        #endregion

        #region Class Methods

        protected override bool SpawnInstances()
        {
            base.SpawnInstances();
            if (disableExistingMainCamera)
            {
                DisableMainCamera();
            }

            _thirdPersonCharacter = character.GetComponent<ThirdPersonCharacter>();

            cameraGameObjectInstance = SpawnPrefab(_playerSpawnerSettings.cameraPrefab, Vector3.zero, Quaternion.identity, null, cameraInstanceName, false);
            _camera = cameraGameObjectInstance.GetComponent<Camera>();
            if (!_camera)
            {
                Debug.LogError($"PlayerSpawner: Failed to find Camera component on PrefabInstance {_playerSpawnerSettings.cameraPrefab.name}");
                IsValidSpawn = false;
            }

            cmRigGameObjectInstance = SpawnPrefab(_playerSpawnerSettings.cmCameraRigPrefab, Vector3.zero, Quaternion.identity, null, cinemachineRigInstanceName, false);
            _cinemachine = cmRigGameObjectInstance.GetComponent<CinemachineCamera>();
            if (!_cinemachine)
            {
                Debug.LogError($"PlayerSpawner: Failed to find CinemachineCamera component on PrefabInstance {_playerSpawnerSettings.cmCameraRigPrefab.name}");
                IsValidSpawn = false;
            }

            return IsValidSpawn;
        }

        private void DisableMainCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera)
            {
                mainCamera.gameObject.SetActive(false);
            }
            /*
            Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera currCamera in allCameras)
            {
                if (currCamera.CompareTag("MainCamera"))
                {
                    currCamera.gameObject.SetActive(false);
                    return;
                }
            }
            */
        }

        protected override void Configure()
        {
            base.Configure();
            character.camera = _camera;
            _cinemachine.Target.TrackingTarget = _thirdPersonCharacter.followTarget.transform;
            _thirdPersonCharacter.followCamera = _cinemachine;
        }

        protected override void SetSpawnsActive()
        {
            base.SetSpawnsActive();
            SetSpawnActive(cameraGameObjectInstance);
            SetSpawnActive(cmRigGameObjectInstance);
        }

        #endregion

#if UNITY_EDITOR
        [Button("Spawn in Editor Scene")]
        private void SpawnInScene()
        {
            _playerSpawnerSettings = spawnSettings as PlayerSpawnerSettings;
            Spawn();
        }
#endif
    }
}