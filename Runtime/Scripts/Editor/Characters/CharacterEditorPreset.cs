#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
using DaftAppleGames.Extensions;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using CharacterController = UnityEngine.CharacterController;
using Vector3 = UnityEngine.Vector3;

namespace DaftAppleGames.TpCharacterController.Editor
{
    [CreateAssetMenu(fileName = "CharacterEditorPreset", menuName = "Daft Apple Games/Character/Character Editor Preset", order = 1)]
    [Serializable]
    public class CharacterEditorPreset : ScriptableObject
    {
        #region Class Variables

        [Header("Prefabs")]
        [SerializeField] private GameObject characterPrefab;

        [SerializeField] private GameObject cinemachinePrefab;
        [SerializeField] private GameObject cameraPrefab;
        [SerializeField] private GameObject footstepsPrefab;

        [Header("Animation Settings")]
        [SerializeField] private AnimationPresets animPresets;

        [Header("Controller Settings")]
        [SerializeField] private string layer = "Player";

        [SerializeField] private LayerMask groundLayerMask;

        #endregion

        #region Class Methods

        public bool Validate(out List<string> validationErrors)
        {
            bool validationResult = true;

            validationErrors = new();

            if (LayerMask.NameToLayer(layer) == -1)
            {
                validationResult = false;
                validationErrors.Add($"The Layer '{layer}' doesn't exist. Please  add a '{layer}' layer via 'Layers > Edit Layers");
            }

            if (!animPresets)
            {
                validationResult = false;
                validationErrors.Add("Please select your animation presets!");
            }

            if (animPresets && (!animPresets.animMappings || !animPresets.animMappings.referenceController))
            {
                validationResult = false;
                validationErrors.Add("Animation presets is not configured! Please check that mappings are assigned and the mapping has a reference controller set!");
            }

            if (cinemachinePrefab && FindAnyObjectByType<CinemachineBrain>())
            {
                validationResult = false;
                validationErrors.Add("There is already a CinemachineBrain preset in the scene. Please remove the CinemachineBrain and try again!");
            }

            return validationResult;
        }

        public bool ApplyPreset(GameObject characterModelGameObject, out List<string> applyResults)
        {
            bool applyResult = true;
            applyResults = new List<string>();

            // Clone and configure the character
            GameObject characterPrefabInstance = CloneCharacterPrefab(characterModelGameObject, "Character");
            characterPrefabInstance.layer = LayerMask.NameToLayer(layer);
            UpdateCharacterController(characterPrefabInstance);
            ConfigureAnimator(characterPrefabInstance);

            // Clone and configure the Cinemachine prefab, if present
            if (cinemachinePrefab)
            {
                GameObject mainCameraInstance = Camera.main ? Camera.main.gameObject : ClonePrefab(cameraPrefab, "Main Camera");
                GameObject cinemachinePrefabInstance = ClonePrefab(cinemachinePrefab, "Third Person Cinemachine Rig");
                ConfigureTpCamera(cinemachinePrefabInstance, mainCameraInstance, characterPrefabInstance);
            }

            if (footstepsPrefab)
            {
                GameObject footstepsPrefabInstance = ClonePrefab(footstepsPrefab, "Footsteps");
                // ConfigureFootstepManager(playerPrefabInstance);
            }

            return applyResult;
        }

        private GameObject ClonePrefab(GameObject prefab, string newName)
        {
            if (!prefab)
            {
                return null;
            }

            GameObject prefabInstance = Instantiate(prefab);
            prefabInstance.transform.position = Vector3.zero;
            prefabInstance.transform.rotation = Quaternion.identity;
            prefabInstance.gameObject.name = newName;

            return prefabInstance;
        }

        private GameObject ClonePrefabAndParent(GameObject prefab, string newName, Transform parentTransform)
        {
            GameObject prefabInstance = ClonePrefab(prefab, newName);
            prefabInstance.transform.SetParent(parentTransform);
            prefabInstance.transform.localPosition = Vector3.zero;
            prefabInstance.transform.localRotation = Quaternion.identity;

            return prefabInstance;
        }

        private GameObject CloneCharacterPrefab(GameObject playerModelGameObject, string newName)
        {
            if (!characterPrefab)
            {
                return null;
            }

            GameObject prefabInstance = Instantiate(characterPrefab);
            prefabInstance.transform.position = playerModelGameObject.transform.position;
            prefabInstance.transform.rotation = playerModelGameObject.transform.rotation;

            playerModelGameObject.transform.SetParent(prefabInstance.transform, false);
            playerModelGameObject.transform.localPosition = Vector3.zero;
            playerModelGameObject.transform.localRotation = Quaternion.identity;

            prefabInstance.name = $"{newName}";
            playerModelGameObject.name = $"{newName} Model";
            return prefabInstance;
        }

        private void ConfigureAnimator(GameObject prefabInstance)
        {
            // Copy the animator
            Animator sourceAnimator = characterPrefab.GetComponentInChildren<Animator>();

            if (sourceAnimator)
            {
                Animator targetAnimator = prefabInstance.EnsureComponent<Animator>();
#if ODIN_INSPECTOR
                // EditorUtils.MoveComponentToTop(prefabInstance, targetAnimator);
#endif
                EditorUtility.CopySerialized(sourceAnimator, targetAnimator);
                DestroyImmediate(sourceAnimator);
                AnimatorController controller =
                    animPresets.animMappings.DuplicateController(prefabInstance.name);
                targetAnimator.runtimeAnimatorController = controller;
                animPresets.UpdateAllAnims(controller);
            }
        }

        private void ConfigureTpCamera(GameObject cinemachinePrefabInstance, GameObject mainCameraInstance, GameObject characterPrefabInstance)
        {
            Camera mainCamera = mainCameraInstance.GetComponent<Camera>();
            CinemachineCamera cmCamera = cinemachinePrefabInstance.GetComponent<CinemachineCamera>();
            cmCamera.Follow = characterPrefabInstance.transform;
        }

        /*
        private void ConfigureFootstepManager(GameObject prefabInstance)
        {
            FootstepManager footstepManager = prefabInstance.GetComponent<FootstepManager>();
            if (!footstepManager)
            {
                return;
            }

            FootstepPoolManager poolManager = FindObjectOfType<FootstepPoolManager>();
            if (!poolManager)
            {
                GameObject poolManagerPrefabInstance = Instantiate(footstepPoolPrefab);
                poolManagerPrefabInstance.name = "Footstep Pool Manager";
                poolManager = poolManagerPrefabInstance.GetComponent<FootstepPoolManager>();
            }

            prefabInstance.GetComponent<FootstepManager>().SetPoolManager(poolManager);

            footstepManager.ConfigureFootstepTriggers();
        }
        */
        private bool UpdateCharacterController(GameObject targetGameObject)
        {
            CharacterController characterController = targetGameObject.GetComponent<CharacterController>();
            if (!characterController)
            {
                return false;
            }

            Vector3 modelSize = GetMeshSize(targetGameObject);
            characterController.center = new Vector3(0, modelSize.y / 2, 0);
            characterController.height = modelSize.y;
            characterController.radius = 0.25f;
            return true;
        }

        private Vector3 GetMeshSize(GameObject targetGameObject)
        {
            Bounds bounds = new Bounds(targetGameObject.transform.position, Vector3.zero);
            foreach (Renderer currRenderer in targetGameObject.GetComponentsInChildren<Renderer>(true))
            {
                bounds.Encapsulate(currRenderer.bounds);
            }

            return bounds.size;
        }

        #endregion
    }
}