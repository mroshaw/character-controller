using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Move To", story: "[Agent] moves to [Transform]", category: "Action/Navigation", id: "704c056a6c235a2d3dd8e4e731f7a37d")]
    public partial class MoveToAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<Transform> Transform;
        private bool _hasMoved;

        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            if (Transform.Value == null)
            {
                LogFailure("MoveTo Transform is null.");
                return Status.Failure;
            }

            AiBrain.MoveTo(Transform.Value.position);
            // AiBrain.MoveTo(Transform.Value.position, DestinationReached);
            _hasMoved = false;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return AiBrain.HasArrived() ? Status.Success : Status.Running;
            // return _hasMoved ? Status.Success : Status.Running;
        }

        private void DestinationReached()
        {
            _hasMoved = true;
        }
    }
}