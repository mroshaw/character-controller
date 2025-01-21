using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Find Water", story: "[Agent] finds nearest [WaterSource]", category: "Action/Needs", id: "cfbeaaac6039bc4e972763f2f0fd128e")]
    public partial class FindWaterAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<Transform> WaterSource;

        private Transform _foundWaterSource;

        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            if (AiBrain.GetClosestWaterSource(out _foundWaterSource))
            {
                WaterSource.Value = _foundWaterSource;
            }

            return Status.Success;
        }
    }
}