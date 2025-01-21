using DaftAppleGames.TpCharacterController.AiController;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.FootSteps
{
    public class FootstepTrigger : CharacterTrigger
    {
        #region Class Variables

        private AudioSource _audioSource;
        private float _cooldownCounter = 0.0f;
        public FootstepManager FootstepManager { get; set; }
        private TerrainData _terrainData;
        private bool _terrainDetected;

        private FootstepSurface _defaultSurface;

        #endregion

        #region Startup

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (!_audioSource)
            {
                Debug.LogError($"FootstepTrigger: no AudioSource on this gameobject! {gameObject}");
            }

            _terrainDetected = !(Terrain.activeTerrain == null);

            if (_terrainDetected)
            {
                _terrainData = Terrain.activeTerrain.terrainData;
            }
        }

        private void Start()
        {
            if (!FootstepManager || !FootstepManager.footstepsEnabled)
            {
                GetComponent<SphereCollider>().enabled = false;
                return;
            }

            _defaultSurface = FootstepManager.GetDefaultSurface();
        }

        #endregion

        #region Class methods

        public override void TriggerEnter(Collider other)
        {
            if (_cooldownCounter > 0.0f || other is null)
            {
                return;
            }

            GetSurfaceFromCollision(transform, other, out FootstepSurface footstepSurface,
                out Vector3 spawnPosition);

            // Spawn particles
            if (footstepSurface.spawnParticle)
            {
                FootstepManager.SpawnFootStepParticleFx(spawnPosition, FootstepManager.transform.rotation);
            }

            // Spawn decal
            if (footstepSurface.spawnDecal)
            {
                FootstepManager.SpawnFootStepDecal(spawnPosition, FootstepManager.transform.rotation);
            }

            // Play random audio
            System.Random randomAudio = new System.Random();
            int audioIndex = randomAudio.Next(0, footstepSurface.audioClips.Length);
            AudioClip audioClip = footstepSurface.audioClips[audioIndex];
            _audioSource.Stop();
            _audioSource.PlayOneShot(audioClip);

            _cooldownCounter = 0.5f;
        }

        private void Update()
        {
            _cooldownCounter -= Time.deltaTime;
        }

        public override void TriggerExit(Collider other)
        {
        }

        #endregion

        #region Class methods

        public void GetSurfaceFromCollision(Transform footTransform, Collider otherCollider,
            out FootstepSurface footstepSurface, out Vector3 spawnPosition)
        {
            // Collision if on a Terrain
            if (otherCollider is TerrainCollider)
            {
                Vector3 collisionPosition = footTransform.position;
                if (!FindTerrainTextureAtPosition(footTransform.position, out var terrainTextureName))
                {
                    footstepSurface = _defaultSurface;
                    spawnPosition = collisionPosition;
                }
                else
                {
                    float terrainHeight = Terrain.activeTerrain.SampleHeight(collisionPosition);
                    spawnPosition = new Vector3(collisionPosition.x, terrainHeight, collisionPosition.z);
                    footstepSurface = FootstepManager.GetSurfaceFromTextureName(terrainTextureName);
                }

                return;
            }

            // Collision if with a Mesh or other collider type
            spawnPosition = otherCollider is MeshCollider { convex: true } or BoxCollider or SphereCollider or CapsuleCollider
                ? otherCollider.ClosestPoint(footTransform.position)
                : footTransform.position;

            if (FindMaterialTextureFromCollider(otherCollider, out var meshTextureName))
            {
                footstepSurface = FootstepManager.GetSurfaceFromTextureName(meshTextureName);
                return;
            }

            // We haven't found anything, so return the default
            footstepSurface = _defaultSurface;
            spawnPosition = footTransform.position;
        }

        private bool FindMaterialTextureFromCollider(Collider other, out string textureName)
        {
            textureName = "";

            MeshRenderer meshRender = other.GetComponent<MeshRenderer>();
            if (!meshRender)
            {
                return false;
            }

            Material meshMaterial = meshRender.material;
            if (!meshMaterial || !meshMaterial.mainTexture)
            {
                return false;
            }

            textureName = meshMaterial.mainTexture.name;
            if (FootstepManager.DebugTextureName)
            {
                Debug.Log($"FootstepManager: Mesh texture is : {textureName}");
            }

            return true;
        }

        private bool FindTerrainTextureAtPosition(Vector3 collisionPosition, out string textureName)
        {
            textureName = "";

            if (Terrain.activeTerrain.terrainData.alphamapTextureCount == 0)
            {
                return false;
            }

            Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
            Vector2 textureSize = new Vector2(Terrain.activeTerrain.terrainData.alphamapWidth,
                Terrain.activeTerrain.terrainData.alphamapHeight);

            int alphaX = (int)((collisionPosition.x / terrainSize.x) * textureSize.x + 0.5f);
            int alphaY = (int)((collisionPosition.z / terrainSize.z) * textureSize.y + 0.5f);

            float[,,] terrainMaps = Terrain.activeTerrain.terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);

            float[] textures = new float[terrainMaps.GetUpperBound(2) + 1];

            for (int n = 0; n < textures.Length; n++)
            {
                textures[n] = terrainMaps[0, 0, n];
            }

            if (textures.Length == 0)
            {
                return false;
            }

            // Looking for the texture with the highest 'mix'
            float textureMaxMix = 0;
            int textureMaxIndex = 0;

            for (int currTexture = 0; currTexture < textures.Length; currTexture++)
            {
                if (textures[currTexture] > textureMaxMix)
                {
                    textureMaxIndex = currTexture;
                    textureMaxMix = textures[currTexture];
                }
            }

            // Texture is at index textureMaxIndex
            textureName = (_terrainData != null && _terrainData.terrainLayers.Length > 0 && _terrainData.terrainLayers[textureMaxIndex].diffuseTexture)
                ? (_terrainData.terrainLayers[textureMaxIndex]).diffuseTexture.name
                : "";

            if (FootstepManager.DebugTextureName)
            {
                Debug.Log($"FootstepManager: Terrain texture is : {textureName}");
            }

            return true;
        }

        #endregion
    }
}