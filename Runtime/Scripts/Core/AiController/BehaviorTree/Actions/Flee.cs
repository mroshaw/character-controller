using System;
using ECM2;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Flee", story: "[Agent] flees from [Target]", category: "Action/Navigation", id: "d9102898acb44b193285ab3326751e54")]
    public partial class FleeAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        private NavMeshCharacter _navMeshCharacter;

        private bool _hasArrived;

        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            AiBrain.Flee(Target.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (AiBrain.AiState == AiState.Fleeing)
            {
                return Status.Running;
            }

            return Status.Success;
        }

        protected override void OnEnd()
        {
            Target.Value = null;
        }
    }
}