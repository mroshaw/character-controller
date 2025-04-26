using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    [CreateAssetMenu(fileName = "ClosestTargets", menuName = "Daft Apple Gamesa/Character/Closest Targets", order = 1)]
    public class ClosestTargets : ScriptableObject
    {
        [SerializeField] private ClosestTargetList allClosestTargets = new();

        public void Add(TargetType target, GameObject targetObject)
        {
            allClosestTargets.Add(target, targetObject);
        }

        public void Clear()
        {
            allClosestTargets.Clear();
        }

        public bool HasAnyTarget()
        {
            foreach (KeyValuePair<TargetType, GameObject> target in allClosestTargets)
            {
                if (target.Value != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetTargetOfType(TargetType targetType, out GameObject targetObject)
        {
            targetObject = allClosestTargets[targetType];
            return  targetObject == null;
        }

    }

    [Serializable]
    public class ClosestTargetList : IEnumerable<KeyValuePair<TargetType, GameObject>>
    {
            [ShowInInspector] readonly Dictionary<TargetType, GameObject> _closestTargets = new();
            public IEnumerator<KeyValuePair<TargetType, GameObject>> GetEnumerator()
            {
                return _closestTargets.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Clear()
            {
                _closestTargets.Clear();
            }

            public void Add(TargetType target, GameObject targetObject)
            {
                _closestTargets.Add(target, targetObject);
            }

            public GameObject this[TargetType targetType] => _closestTargets[targetType];
    }
}