using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    internal class SoundDetector : ProximityDetector
    {
        [PropertyOrder(1)] [BoxGroup("Settings")] [SerializeField] private float minSoundVolume = 0.5f;

        [BoxGroup("Debug")] [ShowInInspector] private DetectorTargets _audibleTargets;

        #region Startup

        protected override void Start()
        {
            base.Start();
            _audibleTargets = new DetectorTargets();
        }

        #endregion

        protected internal override void CheckForTargets(bool triggerEvents)
        {
            base.CheckForTargets(false);
            if (base.HasTargets())
            {
                CheckForAudibleTargets();
            }
        }

        protected override void TargetLost(DetectorTarget lostTarget, bool triggerEvents)
        {
            // If target drops out of the detection range of hte base proximity detector, drop it from the list
            _audibleTargets.RemoveTarget(lostTarget.guid);
            base.TargetLost(lostTarget, true);
        }

        private void CheckForAudibleTargets()
        {
            // Loop through the 'proximity' game objects, and see if any are within range, and making a noise over the hearing limit
            foreach (KeyValuePair<string, DetectorTarget> currTarget in DetectedTargets)
            {
                if (currTarget.Value.targetObject.TryGetComponent(out INoiseMaker noiseMaker))
                {
                    if (noiseMaker.GetNoiseLevel() > minSoundVolume)
                    {
                        // Add the target, if it's not already there
                        if (_audibleTargets.AddTarget(currTarget.Value))
                        {
                            // Debug.Log("Sound Target Detected!");
                            NewTargetDetected(currTarget.Value, true);
                        }
                    }
                }
            }
        }
    }
}