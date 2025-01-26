#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using System;
using System.Collections.Generic;
using DaftAppleGames.GameObjects;
using DaftAppleGames.Utilities;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class FovDetectorOld : MonoBehaviour
    {
        #region Class Variables

        [PropertyOrder(1)] [BoxGroup("Sensor Configuration")] [Tooltip("Only GameObjects in these layers will be detected.")]
        [SerializeField] protected LayerMask detectionLayerMask;

        [PropertyOrder(1)] [BoxGroup("Sensor Configuration")] [Tooltip("Only GameObjects with this tag will be detected.")] [SerializeField]
        [TagSelector] protected string detectionTag;

        [PropertyOrder(1)] [BoxGroup("Sensor Configuration")] [Tooltip("Maximum number of GameObjects that can be detected by the OverlapSphere. Used to avoid GC.")]
        [SerializeField] private int detectionBufferSize = 20;

        [PropertyOrder(1)] [BoxGroup("Aware Sensor Configuration")]
        [Tooltip("An GameObject that is within the OverlapSphere of this radius will be added to the 'awareObjects' list")] [SerializeField] private float awareSensorRange = 15;

        [PropertyOrder(1)] [BoxGroup("Aware Sensor Configuration")]
        [Tooltip("The frequency with which the 'awareness' OverlapSphere is cast. Smaller number for greater accuracy, larger for better performance.")]
        [SerializeField] private int awareCheckFrequencyFrames = 5;

        [PropertyOrder(1)] [BoxGroup("Vision Sensor Configuration")] [Tooltip("Objects on these layers will block vision.")] [SerializeField] private LayerMask blockedLayerMask;

        [PropertyOrder(1)] [BoxGroup("Vision Sensor Configuration")] [Tooltip("Vision cone will be projected from the eyes transform.")]
        [SerializeField] private Transform eyesTransform;

        [PropertyOrder(1)] [BoxGroup("Vision Sensor Configuration")]
        [Tooltip("When a GameObject is in the 'awareObjects' list, a vision cone is cast at this range. If hit, the GameObject is added to the 'seeObjects' list")]
        [SerializeField] private float visionSensorRange = 5;

        [PropertyOrder(1)] [BoxGroup("Vision Sensor Configuration")] [Tooltip("The angle 'sweep' of the vision sensor.")] [SerializeField] private float visionSensorAngle = 170.0f;

        [PropertyOrder(1)] [BoxGroup("Vision Sensor Configuration")]
        [Tooltip("The frequency with which the vision cone is cast. Smaller number for greater accuracy, larger for better performance.")]
        [SerializeField] private int visionCheckFrequencyFrames = 5;

        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<GameObject> visionDetectedEvent;
        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<GameObject> visionLostEvent;
        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<GameObject> awareDetectedEvent;
        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<GameObject> awareLostEvent;

        [PropertyOrder(4)] [BoxGroup("Debug")] [SerializeField] protected bool debugEnabled = true;
        [PropertyOrder(4)] [BoxGroup("Debug")] [SerializeField] private GameObject[] awareGameObjectsDebug;
        [PropertyOrder(4)] [BoxGroup("Debug")] [SerializeField] private GameObject[] visionGameObjectsDebug;

#if UNITY_EDITOR
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private bool drawVisionSphereGizmo = true;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private Color visionConeGizmoColor = Color.red;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private int visionConeGizmoResolution = 50;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private bool drawAwareSphereGizmo = true;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private Color awareSphereGizmoColor = Color.yellow;
#endif

        // private Dictionary<string, GameObject> _visionGameObjectsDict;
        // private Dictionary<float, string> _visionGameObjectsDistanceDict;
        // private Dictionary<string, GameObject> _awareGameObjectsDict;
        private DetectorTargets _awareTargets;
        private DetectorTargets _visionTargets;

        private Collider[] _overlapSphereBuffer;
        private RaycastHit[] _rayHitsBuffer;
        private Collider[] _detectedBuffer;
        private readonly HashSet<string> _existingGuidsBuffer = new();

        private bool _isAwareOfTargets;
        private bool _canSeeTargets;

        #endregion

        #region Startup

        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        protected void Awake()
        {
            _awareTargets = new DetectorTargets();
            _visionTargets = new DetectorTargets();

            _overlapSphereBuffer = new Collider[detectionBufferSize];
            _rayHitsBuffer = new RaycastHit[detectionBufferSize];
            // _awareGameObjectsDict = new Dictionary<string, GameObject>();
            // _visionGameObjectsDict = new Dictionary<string, GameObject>();
            // _visionGameObjectsDistanceDict = new Dictionary<float, string>();
            _detectedBuffer = new Collider[detectionBufferSize];
        }

        #endregion

        #region Update Logic

        protected void Update()
        {
            if (!_canSeeTargets && Time.frameCount % awareCheckFrequencyFrames == 0)
            {
                CheckForAlertObjects();
                _isAwareOfTargets = _awareTargets.HasTargets();
            }

            // Only look for targets is we're aware of one
            if (_isAwareOfTargets && Time.frameCount % visionCheckFrequencyFrames == 0)
            {
                CheckForVisionObjects();
                _canSeeTargets = _visionTargets.HasTargets();
            }
        }

        #endregion

        #region Class methods

        // Can the detector currently see any targets at all?
        public bool CanSeeTargets()
        {
            return _visionTargets.HasTargets();
        }

        // Can the detector currently see any targets with this tag?
        public bool CanSeeTargetsWithTag(string targetTag)
        {
            return _visionTargets.HasTargetWithTag(targetTag);
        }

        // Return the closest target that the detector can currently see, with this tag
        public GameObject GetClosestTargetWithTag(string targetTag)
        {
            return _visionTargets.GetClosestTargetWithTag(targetTag);
        }

        private void CheckForAlertObjects()
        {
            int objectsDetected = Physics.OverlapSphereNonAlloc(transform.position, awareSensorRange, _overlapSphereBuffer, detectionLayerMask);
            RefreshAwareList(_overlapSphereBuffer, objectsDetected);

            if (debugEnabled)
            {
                awareGameObjectsDebug = _awareTargets.GetAllTargetGameObjects();
            }
        }

        private void CheckForVisionObjects()
        {
            int detectedBufferIndex = 0;

            // Loop through the 'aware' game objects, and see if any are within range, within the FOV angle, and not behind any blocking layers
            foreach (KeyValuePair<string, DetectorTarget> currTarget in _awareTargets)
            {
                if (GetDistanceToTarget(currTarget.Value.targetObject) < visionSensorRange && GetAngleToTarget(currTarget.Value.targetObject) < visionSensorAngle / 2 &&
                    CanSeeTarget(currTarget.Value.targetObject))
                {
                    _detectedBuffer[detectedBufferIndex] = currTarget.Value.targetObject.GetComponent<Collider>();
                    detectedBufferIndex++;
                }
            }

            // Clear out the rest of the buffer
            for (int currIndex = detectedBufferIndex; currIndex < detectionBufferSize; currIndex++)
            {
                _detectedBuffer[currIndex] = null;
            }

            RefreshVisionList(_detectedBuffer, detectedBufferIndex);

            if (debugEnabled)
            {
                visionGameObjectsDebug = _visionTargets.GetAllTargetGameObjects();
            }
        }

        private void UpdateTargetDict(Collider[] detectedColliders, int numberDetected, ref DetectorTargets currentTargets, Action<GameObject> targetAddedDelegate,
            Action<GameObject> targetLostDelegate)
        {
            _existingGuidsBuffer.Clear();

            for (int currCollider = 0; currCollider < numberDetected; currCollider++)
            {
                // Check if this is a new object
                ObjectGuid guid = detectedColliders[currCollider].GetComponent<ObjectGuid>();
                GameObject colliderGameObject = detectedColliders[currCollider].gameObject;

                if (guid && colliderGameObject.CompareTag(detectionTag))
                {
                    // Add to HashSet for later reference
                    _existingGuidsBuffer.Add(guid.Guid);

                    if (!currentTargets.HasGuid(guid.Guid))
                    {
                        float distanceToTarget = GetDistanceToTarget(colliderGameObject);
                        currentTargets.AddTarget(guid.Guid, colliderGameObject, distanceToTarget, colliderGameObject.tag);
                        targetAddedDelegate.Invoke(colliderGameObject);
                    }
                }
            }

            // Check to see if there are any objects in the 'aware dictionary' that are no longer in the alert list
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, DetectorTarget> currTarget in currentTargets)
            {
                if (!_existingGuidsBuffer.Contains(currTarget.Key))
                {
                    targetLostDelegate.Invoke(currTarget.Value.targetObject);
                    keysToRemove.Add(currTarget.Key);
                }
            }

            // Remove from the dictionary
            foreach (string key in keysToRemove)
            {
                currentTargets.RemoveTarget(key);
            }
        }

        private float GetDistanceToTarget(GameObject target)
        {
            return (target.transform.position - transform.position).magnitude;
        }

        private float GetAngleToTarget(GameObject target)
        {
            // Calculate the direction from GameObject1 to GameObject2
            Vector3 directionToTarget = target.transform.position - transform.position;

            // Calculate the angle between forward vector and the direction to the target
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            // Determine if the target is to the left or right of GameObject1
            // float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(transform.forward, directionToTarget)));
            return angle;
        }

        private bool CanSeeTarget(GameObject target)
        {
            Vector3 directionToTarget = (target.transform.position + (Vector3.up * 1.5f)) - eyesTransform.position;
            // float maxDistance = directionToTarget.magnitude;
            Ray ray = new Ray(eyesTransform.position, directionToTarget.normalized);

            int objectsDetected = Physics.RaycastNonAlloc(ray, _rayHitsBuffer, visionSensorRange + 0.5f, detectionLayerMask | blockedLayerMask);

#if UNITY_EDITOR
            if (debugEnabled)
            {
                Debug.DrawRay(eyesTransform.position, directionToTarget, Color.red, 0, false);
            }
#endif

            for (int i = 0; i < objectsDetected; i++)
            {
                RaycastHit hit = _rayHitsBuffer[i];

                // Check if the hit object is in blockingLayers; if so, return false
                if (((1 << hit.collider.gameObject.layer) & blockedLayerMask) != 0)
                {
                    return false;
                }

                // Check if the hit object is in targetLayers and matches the target
                if (((1 << hit.collider.gameObject.layer) & detectionLayerMask) != 0 && hit.collider.gameObject == target)
                {
                    return true;
                }
            }

            return false;
        }

        private void AwareDetected(GameObject detectedGameObject)
        {
            // Debug.Log($"RangeDetector: {gameObject.name} detected: {detectedGameObject.name}");
            awareDetectedEvent.Invoke(detectedGameObject);
        }

        private void AwareLost(GameObject detectedGameObject)
        {
            // Debug.Log($"RangeDetector: {gameObject.name} lost: {detectedGameObject.name}");
            awareLostEvent.Invoke(detectedGameObject);
        }

        private void VisionDetected(GameObject detectedGameObject)
        {
            // Debug.Log($"FovDetector: {gameObject.name} detected: {detectedGameObject.name}");
            awareDetectedEvent.Invoke(detectedGameObject);
        }

        private void VisionLost(GameObject detectedGameObject)
        {
            // Debug.Log($"FovDetector: {gameObject.name} lost: {detectedGameObject.name}");
            awareLostEvent.Invoke(detectedGameObject);
        }

        private void RefreshAwareList(Collider[] awareColliders, int numberDetected)
        {
            UpdateTargetDict(awareColliders, numberDetected, ref _awareTargets, AwareDetected, AwareLost);
        }

        private void RefreshVisionList(Collider[] visionColliders, int numberDetected)
        {
            UpdateTargetDict(visionColliders, numberDetected, ref _visionTargets, VisionDetected, VisionLost);
        }

        #endregion

        #region Editor methods

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (drawVisionSphereGizmo)
            {
                // Draw cone for the 'vision' range
                GizmoTools.DrawConeGizmo(eyesTransform, visionSensorAngle, visionSensorRange, visionConeGizmoColor, visionConeGizmoResolution);
            }

            if (drawAwareSphereGizmo)
            {
                // Draw a sphere for the 'alert' range
                Gizmos.color = awareSphereGizmoColor;
                Gizmos.DrawSphere(transform.position, awareSensorRange);
            }
        }
#endif

        #endregion
    }
}