using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class DetectorManager : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Settings")] [Tooltip("Only colliders on these layers will be considered as targets.")] [SerializeField] private LayerMask detectionLayerMask;
        [BoxGroup("Settings")] [Tooltip("Only colliders with these tags will be considered as targets.")] [SerializeField] [TagSelector] string[] detectionTags;

        [BoxGroup("Settings")] [Tooltip("Maximum number of colliders that can be detected by detectors. This is to avoid garbage collection.")]
        [SerializeField] protected int detectionBufferSize = 20;

        [BoxGroup("Settings")]
        [Tooltip("List of detectors that will be queried by this manager. Use the button below to automatically populate, or manually drag in detectors required.")]
        [SerializeField] private Detector[] detectors;

        [BoxGroup("Events")] [Tooltip("This event is fired when a new closest target is acquired.")] public UnityEvent newTargetDetectedEvent;
        [BoxGroup("Events")] [Tooltip("This event is fired when the current closest target is lost.")] public UnityEvent targetLostEvent;

        [BoxGroup("Debug")] [ShowInInspector] [SerializeField] private SortedDetectorTargetList _sortedDetectedTargets;

        [BoxGroup("Debug")] [ShowInInspector] private Dictionary<string, SortedDetectorTargetList> _detectorTargetsByTag;

        // This is used to distribute 'Update' calls to sensors across DetectorManagers. So rather than all instances polling on exactly the same frame,
        // calls will be randomly distributed across the number of seed frames.
        private const int LoadBalanceSeed = 10;

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

            foreach (Detector detector in detectors)
            {
                detector.DetectionTags = detectionTags;
                detector.DetectionBufferSize = detectionBufferSize;
                detector.DetectionLayerMask = detectionLayerMask;
            }

            _detectorTargetsByTag = new Dictionary<string, SortedDetectorTargetList>();
            foreach (string currTag in detectionTags)
            {
                _detectorTargetsByTag.Add(currTag, new SortedDetectorTargetList());
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
            foreach (Detector detector in detectors)
            {
                if ((Time.frameCount + _instanceLoadBalanceFrame) % detector.refreshFrequency == 0)
                {
                    detector.CheckForTargets(true);
                    UpdateTargetDistances();
                }
            }
        }

        #region Class Methods

        public bool HasTarget()
        {
            return _sortedDetectedTargets.Count() > 0;
        }

        public GameObject GetClosestTarget()
        {
            return _sortedDetectedTargets.GetClosestTarget()?.targetObject;
        }

        public GameObject GetClosestTargetWithTag(string tagToFind)
        {
            return _detectorTargetsByTag[tagToFind].GetClosestTarget()?.targetObject;
        }

        private void NewTargetDetected(DetectorTarget detectorTarget)
        {
            _detectorTargetsByTag[detectorTarget.tag].Add(detectorTarget);
            _sortedDetectedTargets.Add(detectorTarget);
            newTargetDetectedEvent.Invoke();
        }

        private void TargetLost(DetectorTarget detectorTarget)
        {
            _sortedDetectedTargets.Remove(detectorTarget);
            _detectorTargetsByTag[detectorTarget.tag].Remove(detectorTarget);
            targetLostEvent.Invoke();
        }

        private void UpdateTargetDistances()
        {
            _sortedDetectedTargets.UpdateDistances(transform);
        }

        #endregion

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