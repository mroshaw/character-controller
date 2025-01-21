#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using System;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    [Serializable]
    public struct WanderParams
    {
        [BoxGroup("Wander Settings")] [SerializeField] private float speed;
        [BoxGroup("Wander Settings")] [SerializeField] private float minRange;
        [BoxGroup("Wander Settings")] [SerializeField] private float maxRange;
        [BoxGroup("Wander Settings")] [SerializeField] private float minPause;
        [BoxGroup("Wander Settings")] [SerializeField] private float maxPause;
        [BoxGroup("Wander Settings")] [SerializeField] private Transform centerTransform;

        public float Speed => speed;
        public float MinRange => minRange;
        public float MaxRange => maxRange;
        public float MinPause => minPause;
        public float MaxPause => maxPause;
        public Transform CenterTransform => centerTransform;
    }
}