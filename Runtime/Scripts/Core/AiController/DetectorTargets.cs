using System;
using System.Collections;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    [Serializable]
    public class DetectorTargets : IEnumerable<KeyValuePair<string, DetectorTarget>>
    {
        [ShowInInspector] private readonly Dictionary<string, DetectorTarget> _targets = new();

        internal bool AddTarget(DetectorTarget detectorTarget)
        {
            if (_targets.TryGetValue(detectorTarget.guid, out DetectorTarget _))
            {
                return false;
            }
            _targets.Add(detectorTarget.guid, detectorTarget);
            return true;
        }
        internal DetectorTarget AddTarget(string guid, GameObject target, float distance, string tagValue)
        {
            if (_targets.TryGetValue(guid, out DetectorTarget addTarget))
            {
                return addTarget;
            }

            DetectorTarget newTarget = new()
            {
                guid = guid,
                targetObject = target,
                Distance = distance,
                tag = tagValue
            };
            _targets.Add(guid, newTarget);

            return newTarget;
        }

        internal void RemoveTarget(string guid)
        {
            _targets.Remove(guid);
        }

        internal bool HasGuid(string guid)
        {
            return _targets.ContainsKey(guid);
        }

        internal GameObject GetTargetGameObject(string guid)
        {
            return _targets[guid].targetObject;
        }

        internal bool HasTargets()
        {
            return _targets.Count > 0;
        }

        internal bool HasTargetWithTag(string tag)
        {
            foreach (var entry in _targets)
            {
                if (entry.Value.targetObject.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }

        internal DetectorTarget GetClosestTarget()
        {
            KeyValuePair<string, DetectorTarget> minTarget = default;
            float minDistance = float.MaxValue;
            foreach (var entry in _targets)
            {
                if (entry.Value.Distance < minDistance)
                {
                    minDistance = entry.Value.Distance;
                    minTarget = entry;
                }
            }
            return minTarget.Value;
        }

        internal GameObject GetClosestTargetGameObject()
        {
            return GetClosestTarget().targetObject;
        }

        internal GameObject GetClosestTargetWithTag(string tag)
        {
            KeyValuePair<string, DetectorTarget> minTarget = default;
            float minDistance = float.MaxValue;
            foreach (var entry in _targets)
            {
                if (entry.Value.targetObject.CompareTag(tag) && entry.Value.Distance < minDistance)
                {
                    minDistance = entry.Value.Distance;
                    minTarget = entry;
                }
            }

            // If target found, return closest. Otherwise, return null
            return minDistance < float.MaxValue ? minTarget.Value.targetObject : null;
        }

        internal GameObject[] GetAllTargetGameObjects()
        {
            int numTargets = _targets.Count;
            GameObject[] allGameObjects = new GameObject[numTargets];
            int currTargetIndex = 0;

            foreach (KeyValuePair<string, DetectorTarget> currTarget in _targets)
            {
                allGameObjects[currTargetIndex] = currTarget.Value.targetObject;
            }

            return allGameObjects;
        }

        public IEnumerator<KeyValuePair<string, DetectorTarget>> GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    public class SortedDetectorTargetList
    {
        [SerializeReference] private readonly List<DetectorTarget> _targets = new();

        public void Add(DetectorTarget target)
        {
            if (_targets.Exists(existingTarget => existingTarget.targetObject == target.targetObject))
            {
                return;
            }
            target.OnDistanceChanged += HandleDistanceChanged;
            _targets.Add(target);
            Sort();
        }

        public void Remove(DetectorTarget target)
        {
            target.OnDistanceChanged -= HandleDistanceChanged;
            _targets.Remove(target);
        }

        public DetectorTarget GetClosestTarget()
        {
            if (Count() > 0)
            {
                return _targets[0];
            }

            return null;
        }

        public int Count()
        {
            return _targets.Count;
        }

        public void UpdateDistances(Transform sourceTransform)
        {
            foreach (DetectorTarget target in _targets.ToArray())
            {
                target.UpdateDistance(sourceTransform);
            }
        }

        public IReadOnlyList<DetectorTarget> Targets => _targets.AsReadOnly();

        private void HandleDistanceChanged(DetectorTarget target)
        {
            Sort();
        }

        private void Sort()
        {
            _targets.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        }
    }

    [Serializable]
    public class DetectorTarget
    {
        [SerializeField] internal string guid;
        [SerializeField] internal GameObject targetObject;
        [SerializeField] internal string tag;

        [SerializeField] private float distance;
        public float Distance
        {
            get => distance;
            set
            {
                if (Math.Abs(distance - value) > Mathf.Epsilon)
                {
                    distance = value;
                    OnDistanceChanged?.Invoke(this);
                }
            }
        }

        public event Action<DetectorTarget> OnDistanceChanged;

        public float UpdateDistance(Transform sourceTransform)
        {
            Distance = (sourceTransform.position - targetObject.transform.position).magnitude;
            return Distance;
        }
    }
}