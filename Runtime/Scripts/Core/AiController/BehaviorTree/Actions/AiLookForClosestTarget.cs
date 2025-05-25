using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Look for Closest Target", story: "[Agent] looks for [ClosestTargets]", category: "Action", id: "b728d27c0a1f72cf322961b6b939b7a4")]
    public partial class AiLookForClosestTargets : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<ClosestTargets> ClosestTargets;
        private ClosestTargets _detectedTargets;

        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _detectedTargets = AiBrain.DetectorManager.GetAllClosestTargets();
            ClosestTargets.Value = _detectedTargets;

            if(_detectedTargets.HasAnyTarget())
            {
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }
    }
}