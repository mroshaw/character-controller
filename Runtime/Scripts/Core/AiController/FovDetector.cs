#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using System.Collections.Generic;
using DaftAppleGames.Utilities;
using UnityEngine;

#if UNITY_EDITOR
#endif
namespace DaftAppleGames.TpCharacterController.AiController
{
    internal class FovDetector : ProximityDetector
    {
        #region Class Variables
        [PropertyOrder(1)][BoxGroup("Vision Sensor Configuration")][Tooltip("Objects on these layers will block vision.")][SerializeField] private LayerMask blockedLayerMask;
        [PropertyOrder(1)][BoxGroup("Vision Sensor Configuration")][Tooltip("Vision cone will be projected from the eyes transform.")][SerializeField] private Transform eyesTransform;
        [PropertyOrder(1)][BoxGroup("Vision Sensor Configuration")][Tooltip("Only objects within this distance are added to the target list")][SerializeField] private float visionSensorRange = 5;
        [PropertyOrder(1)][BoxGroup("Vision Sensor Configuration")][Tooltip("The angle 'sweep' of the vision sensor.")][SerializeField] private float visionSensorAngle = 170.0f;

#if UNITY_EDITOR
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] private bool drawLineOfSightRay;
        [PropertyOrder(3)][FoldoutGroup("Gizmos")][SerializeField] private int visionConeGizmoResolution = 50;
#endif

        [BoxGroup("Debug")] [ShowInInspector] private DetectorTargets _visibleTargets;
        private RaycastHit[] _rayHitsBuffer;

        #endregion
        #region Startup
        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        protected override void Start()
        {
            base.Start();
            _visibleTargets = new DetectorTargets();

            _rayHitsBuffer = new RaycastHit[DetectionBufferSize];
        }
        #endregion
        #region Class methods

        protected internal override void CheckForTargets(bool triggerEvents)
        {
            base.CheckForTargets(false);
            if (base.HasTargets())
            {
                CheckForVisibleTargets();
            }
        }

        // Can the detector currently see any targets at all?
        protected override bool HasTargets()
        {
            return _visibleTargets.HasTargets();
        }

        // Return the closest target that the detector can currently see, with this tag
        public override GameObject GetClosestTargetWithTag(string targetTag)
        {
            return _visibleTargets.GetClosestTargetWithTag(targetTag);
        }

        protected override void TargetLost(DetectorTarget lostTarget, bool triggerEvents)
        {
            // If target drops out of the detection range of hte base proximity detector, drop it from the list
            _visibleTargets.RemoveTarget(lostTarget.guid);
            base.TargetLost(lostTarget, true);

        }

        private void CheckForVisibleTargets()
        {
            // Loop through the 'proximity' game objects, and see if any are within range, within the FOV angle, and not behind any blocking layers
            foreach (KeyValuePair<string, DetectorTarget> currTarget in DetectedTargets)
            {
                if (GetDistanceToTarget(currTarget.Value.targetObject) < visionSensorRange && GetAngleToTarget(currTarget.Value.targetObject) < visionSensorAngle / 2 && CanSeeTarget(currTarget.Value.targetObject))
                {
                    // Add the target, if it's not already there
                    if (_visibleTargets.AddTarget(currTarget.Value))
                    {
                        Debug.Log("Visible Target Detected!");
                        NewTargetDetected(currTarget.Value, true);
                    }
                }
            }
        }

        private bool CanSeeTarget(GameObject target)
        {
            Vector3 directionToTarget = (target.transform.position + (Vector3.up * 1.5f)) - eyesTransform.position;
            // float maxDistance = directionToTarget.magnitude;
            Ray ray = new Ray(eyesTransform.position, directionToTarget.normalized);

            int objectsDetected = Physics.RaycastNonAlloc(ray, _rayHitsBuffer, visionSensorRange + 0.5f, DetectionLayerMask | blockedLayerMask);

#if UNITY_EDITOR
            if (drawLineOfSightRay)
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
                if (((1 << hit.collider.gameObject.layer) & DetectionLayerMask) != 0 && hit.collider.gameObject == target)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        #region Editor methods
#if UNITY_EDITOR

        protected override void DrawGizmos()
        {
            base.DrawGizmos();
            // Draw cone for the 'vision' range
            GizmoTools.DrawConeGizmo(eyesTransform, visionSensorAngle, visionSensorRange, gizmoColor2, visionConeGizmoResolution);
        }
#endif
        #endregion
    }
}