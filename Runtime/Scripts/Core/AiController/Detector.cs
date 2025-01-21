using System.Linq;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.AiController
{
    internal abstract class Detector : MonoBehaviour
    {
        #region Class Fields

        [BoxGroup("Settings")] [Tooltip("The range of this detector, presented as a sphere around the gameObject transform.")] [SerializeField] protected float detectorRange;

        [BoxGroup("Settings")] [Tooltip("The number of frames to wait before refreshing the detector targets. Balance this between accuracy and performance.")]
        [SerializeField] public float refreshFrequency = 5.0f;

#if UNITY_EDITOR
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] protected bool drawGizmos = true;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] protected Color gizmoColor1 = Color.yellow;
        [PropertyOrder(3)] [FoldoutGroup("Gizmos")] [SerializeField] protected Color gizmoColor2 = Color.green;
#endif

        internal LayerMask DetectionLayerMask { get; set; }
        internal string[] DetectionTags { get; set; }
        internal int DetectionBufferSize { get; set; }

        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<DetectorTarget> newTargetDetectedEvent;
        [PropertyOrder(2)] [FoldoutGroup("Events")] public UnityEvent<DetectorTarget> targetLostEvent;

        protected DetectorTargets DetectedTargets;

        #endregion

        #region Abstract Methods

        protected internal abstract void CheckForTargets(bool triggerEvents);
        protected internal abstract DetectorTarget GetClosestTarget();

        #endregion

        #region Class Methods

        protected void SetLayerMask(LayerMask layerMaskToSet)
        {
            DetectionLayerMask = layerMaskToSet;
        }

        protected void SetTag(string[] tagsToSet)
        {
            DetectionTags = tagsToSet;
        }

        protected virtual void NewTargetDetected(DetectorTarget detectedTarget, bool triggerEvents)
        {
            if (triggerEvents)
            {
                // Debug.Log("Base Target Detected! Event triggered!");
                newTargetDetectedEvent.Invoke(detectedTarget);
            }
        }

        protected virtual void TargetLost(DetectorTarget lostTarget, bool triggerEvents)
        {
            if (triggerEvents)
            {
                // Debug.Log("Target Lost! Event triggered!");
                targetLostEvent.Invoke(lostTarget);
            }
        }

        protected bool IsColliderInArray(Collider colliderToCheck, Collider[] colliderArray)
        {
            return colliderArray.Contains(colliderToCheck);
        }

        protected float GetDistanceToTarget(GameObject target)
        {
            return (target.transform.position - transform.position).magnitude;
        }

        protected float GetAngleToTarget(GameObject target)
        {
            // Calculate the direction from GameObject1 to GameObject2
            Vector3 directionToTarget = target.transform.position - transform.position;

            // Calculate the angle between forward vector and the direction to the target
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            // Determine if the target is to the left or right of GameObject1
            // float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(transform.forward, directionToTarget)));
            return angle;
        }

#if UNITY_EDITOR

        protected abstract void DrawGizmos();

        protected void OnDrawGizmosSelected()
        {
            if (drawGizmos)
            {
                DrawGizmos();
            }
        }
#endif

        #endregion
    }
}