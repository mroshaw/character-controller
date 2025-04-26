using DaftAppleGames.TpCharacterController.AiController;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AiLookForClosestTargetOfType", story: "[Agent] looks for [TargetType] [Target]", category: "Action", id: "7fc1312b9779eeaf3720cfe27ceff456")]
    public partial class AiLookForClosestTargetOfTypeAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<TargetType> TargetType;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
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
            if(AiBrain.DetectorManager.GetClosestTargetOfType(TargetType.Value, out GameObject closestTarget))
            {
                Target.Value = closestTarget;
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }

}