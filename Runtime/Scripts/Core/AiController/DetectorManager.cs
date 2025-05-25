using System;
using System.Collections.Generic;
using System.Linq;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class DetectorManager : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Settings")] [Tooltip("Only colliders on these layers will be considered as targets.")] [SerializeField] private LayerMask detectionLayerMask;
        [BoxGroup("Settings")] [Tooltip("Only colliders with these tags will be considered as targets.")] [SerializeField] private TargetTagMappingSO detectedTargetTagMapping;

        [BoxGroup("Settings")] [Tooltip("Maximum number of colliders that can be detected by detectors. This is to avoid garbage collection.")]
        [SerializeField] protected int detectionBufferSize = 20;

        [BoxGroup("Settings")] [SerializeField] private bool overrideDetectorPolling = true;

        [BoxGroup("Settings")] [Tooltip("Poll detectors every n frames. Lower values means greater accuracy but slower performance.")]
        [SerializeField] private int detectorPollFrames = 5;

        [BoxGroup("Settings")]
        [Tooltip("List of detectors that will be queried by this manager. Use the button below to automatically populate, or manually drag in detectors required.")]
        [SerializeField] private Detector[] detectors;

        [BoxGroup("Events")] [Tooltip("This event is fired when a new closest target is acquired.")] public UnityEvent newTargetDetectedEvent;
        [BoxGroup("Events")] [Tooltip("This event is fired when the current closest target is lost.")] public UnityEvent targetLostEvent;

        [BoxGroup("Debug")] [ShowInInspector] [SerializeField] private DetectorTargets _allDetectedTargets;
        [BoxGroup("Debug")] [SerializeField] [InlineEditor] private ClosestTargets _closestDetectedTargets;
        [BoxGroup("Debug")] [ShowInInspector] private Dictionary<TargetType, SortedDetectorTargetList> _detectorTargetsByTargetType;

        // This is used to distribute 'Update' calls to sensors across DetectorManagers. So rather than all instances polling on exactly the same frame,
        // calls will be randomly distributed across the number of seed frames.
        private const int LoadBalanceSeed = 10;
        private TargetType[] _detectedTargetTypes;
        private int _instanceLoadBalanceFrame;

        #endregion

        #region Startup

        private void OnEnable()
        {
            foreach (Detector detector in detectors)
            {
                detector.newTargetDetectedEvent.AddListener(NewTargetDetected);
                detector.targetLostEvent.AddListener(TargetLost);
            }
        }

        private void Awake()
        {
            _instanceLoadBalanceFrame = Random.Range(0, LoadBalanceSeed);
            _closestDetectedTargets = ScriptableObject.CreateInstance<ClosestTargets>();
            _detectedTargetTypes = detectedTargetTagMapping.GetTargetTypes();

            foreach (Detector detector in detectors)
            {
                detector.TargetTagMappings = detectedTargetTagMapping;
                detector.DetectionBufferSize = detectionBufferSize;
                detector.DetectionLayerMask = detectionLayerMask;

                _detectorTargetsByTargetType = new Dictionary<TargetType, SortedDetectorTargetList>();
                foreach (TargetType currTargetType in _detectedTargetTypes)
                {
                    if (!_detectorTargetsByTargetType.ContainsKey(currTargetType))
                    {
                        _detectorTargetsByTargetType.Add(currTargetType, new SortedDetectorTargetList());
                    }
                }
            }
        }

        private void OnDisable()
        {
            foreach (Detector detector in detectors)
            {
                detector.newTargetDetectedEvent.RemoveListener(NewTargetDetected);
                detector.targetLostEvent.RemoveListener(TargetLost);
            }
        }

        #endregion

        #region Update

        private void Update()
        {
            if (overrideDetectorPolling)
            {
                if ((Time.frameCount + _instanceLoadBalanceFrame) % detectorPollFrames == 0)
                {
                    foreach (Detector detector in detectors)
                    {
                        detector.CheckForTargets(true);
                    }

                    UpdateTargetDistances();
                }
            }

            foreach (Detector detector in detectors)
            {
                if ((Time.frameCount + _instanceLoadBalanceFrame) % detector.RefreshFrequency == 0)
                {
                    detector.CheckForTargets(true);
                    UpdateTargetDistances();
                }
            }
        }

        #endregion

        #region Class Methods

        public bool HasTarget()
        {
            return _allDetectedTargets.Any();
        }

        private void NewTargetDetected(DetectorTarget detectorTarget)
        {
            _detectorTargetsByTargetType[detectorTarget.targetType].Add(detectorTarget);
            _allDetectedTargets.AddTarget(detectorTarget);
            CalculateClosestTargets();
            newTargetDetectedEvent.Invoke();
        }

        private void TargetLost(DetectorTarget detectorTarget)
        {
            _allDetectedTargets.RemoveTarget(detectorTarget.guid);
            _detectorTargetsByTargetType[detectorTarget.targetType].Remove(detectorTarget);
            CalculateClosestTargets();
            targetLostEvent.Invoke();
        }

        private void UpdateTargetDistances()
        {
            _allDetectedTargets.UpdateDistances(transform);
        }

        private void CalculateClosestTargets()
        {
            _closestDetectedTargets.Clear();
            foreach (TargetType targetType in _detectedTargetTypes)
            {
                DetectorTarget target = _allDetectedTargets.GetClosestTargetByTargetType(targetType);
                if (target != null)
                {
                    _closestDetectedTargets.Add(targetType, target.targetObject);
                }
            }
        }

        public ClosestTargets GetAllClosestTargets()
        {
            return _closestDetectedTargets;
        }

        public bool GetClosestTargetOfType(TargetType targetType, out GameObject closestTarget)
        {
            DetectorTarget target = _detectorTargetsByTargetType[targetType].GetClosestTarget();
            if (target != null)
            {
                closestTarget = target.targetObject;
                return true;
            }

            closestTarget = null;
            return false;
        }

        #endregion

        #region Editor Methods

        [Button("Refresh Detectors")]
        private void RefreshDetectors()
        {
            detectors = GetComponentsInChildren<Detector>(true);
        }

        #endregion
    }
}