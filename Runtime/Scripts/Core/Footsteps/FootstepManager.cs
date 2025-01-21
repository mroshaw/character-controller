#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace DaftAppleGames.TpCharacterController.FootSteps
{
    public class FootstepManager : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Settings")] public bool footstepsEnabled;
        [BoxGroup("Settings")] [SerializeField] private List<FootstepTrigger> footstepTriggers;
        [BoxGroup("Settings")] [SerializeField] private FootstepSurface[] footstepSurfaces;
        [BoxGroup("Settings")] [SerializeField] private FootstepSurface defaultSurface;
        [BoxGroup("Settings")] [SerializeField] private LayerMask triggerLayerMask;
        [BoxGroup("Settings")] [SerializeField] private AudioMixerGroup audioMixerGroup;
        [BoxGroup("Debug")] [SerializeField] public bool debugTextureName;

        [BoxGroup("Spawn Settings")] public bool alignToTerrainSlope;

        [BoxGroup("Pool Settings")] [SerializeField] private PrefabPool particleFxPool;
        [BoxGroup("Pool Settings")] [SerializeField] private PrefabPool decalPool;

        public bool DebugTextureName => debugTextureName;

        #endregion

        private Dictionary<string, FootstepSurface> _footstepSurfaceDict;

        #region Startup

        public void OnEnable()
        {
            // Register the triggers
            foreach (FootstepTrigger trigger in footstepTriggers)
            {
                if (trigger)
                {
                    trigger.FootstepManager = this;
                }
            }
        }

        private void Awake()
        {
            // Create a dictionary of all supported textures to corresponding surfaces
            CreateSurfaceDictionary();
        }

        #endregion

        #region Class methods

        public FootstepSurface GetDefaultSurface()
        {
            return defaultSurface;
        }

        public FootstepSurface[] GetAllSurfaces()
        {
            return footstepSurfaces;
        }

        public FootstepSurface GetSurfaceFromTextureName(string textureName)
        {
            return _footstepSurfaceDict.GetValueOrDefault(textureName, defaultSurface);
        }

        public void SpawnFootStepParticleFx(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            particleFxPool.SpawnInstance(spawnPosition, spawnRotation);
        }

        public void SpawnFootStepDecal(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            decalPool.SpawnInstance(spawnPosition, spawnRotation);
        }

        private void CreateSurfaceDictionary()
        {
            _footstepSurfaceDict = new Dictionary<string, FootstepSurface>();

            foreach (FootstepSurface surface in footstepSurfaces)
            {
                foreach (string textureName in surface.textureNames)
                {
                    _footstepSurfaceDict.Add(textureName, surface);
                }
            }
        }

        #endregion

        #region Editor methods

        [Button("Create Triggers")]
        private void CreateFootstepTriggers()
        {
            if (!GetComponentInChildren<Animator>())
            {
                return;
            }

            footstepTriggers = new List<FootstepTrigger>
            {
                CreateFootstepTriggerOnFoot(HumanBodyBones.LeftFoot),
                CreateFootstepTriggerOnFoot(HumanBodyBones.RightFoot)
            };
        }

        private FootstepTrigger CreateFootstepTriggerOnFoot(HumanBodyBones footBone)
        {
            Animator animator = GetComponentInChildren<Animator>();
            Transform footTransform = animator.GetBoneTransform(footBone);

            FootstepTrigger existingTrigger = footTransform.gameObject.GetComponentInChildren<FootstepTrigger>();
            if (existingTrigger)
            {
                DestroyImmediate(existingTrigger.gameObject);
            }

            GameObject newTrigger = new GameObject($"Footstep Trigger {footBone}");
            newTrigger.transform.SetParent(footTransform);
            newTrigger.transform.localPosition = Vector3.zero;
            newTrigger.transform.localScale = Vector3.one;
            FootstepTrigger newFootStepTrigger = newTrigger.AddComponent<FootstepTrigger>();
            newFootStepTrigger.triggerLayers = triggerLayerMask;
            SphereCollider newSphereCollider = newTrigger.AddComponent<SphereCollider>();
            newSphereCollider.radius = 0.1f;
            newSphereCollider.isTrigger = true;
            newSphereCollider.center = new Vector3(0, -0.1f, 0.0f);
            AudioSource newAudioSource = newTrigger.AddComponent<AudioSource>();
            newAudioSource.loop = false;
            newAudioSource.outputAudioMixerGroup = audioMixerGroup;
            newAudioSource.spatialBlend = 1.0f;

            return newFootStepTrigger;
        }

        public void SetDecalPrefabPool(PrefabPool newDecalPool)
        {
            decalPool = newDecalPool;
        }

        public void SetParticlePool(PrefabPool newParticlePool)
        {
            particleFxPool = newParticlePool;
        }

        [Button("Find Triggers")]
        private void FindTriggers()
        {
            FootstepTrigger[] foundTriggers = gameObject.GetComponentsInChildren<FootstepTrigger>(true);
            footstepTriggers = new();
            footstepTriggers = foundTriggers.ToList();
        }

        #endregion
    }
}