using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TpCharacterController
{
    public class NoiseMaker : MonoBehaviour, INoiseMaker
    {
        #region Class Variables

        [BoxGroup("Settings")] [SerializeField] private float fadeDuration = 2.0f;
        [BoxGroup("Debug")] [SerializeField] private float noiseLevel;
#if UNITY_EDITOR
        [BoxGroup("Debug")] [SerializeField] private NoiseEmitter[] noiseEmitters;
#endif

        #endregion

        #region Update methods

        private void Update()
        {
            if (noiseLevel < 0.01f)
            {
                noiseLevel = 0.0f;
                return;
            }

            noiseLevel -= Time.deltaTime / fadeDuration;
        }

        #endregion

        #region Class methods

        public void MakeNoise(float noiseHeard)
        {
            this.noiseLevel = Math.Max(this.noiseLevel, noiseHeard);
        }

        public float GetNoiseLevel()
        {
            return noiseLevel;
        }

        #endregion

        #region Editor methods

#if UNITY_EDITOR
        [Button("Refresh Noise Emitters")]
        private void RefreshEmitters()
        {
            noiseEmitters = GetComponentsInChildren<NoiseEmitter>(true);
        }
#endif

        #endregion
    }
}