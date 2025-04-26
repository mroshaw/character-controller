using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Move To", story: "[Agent] moves to [Transform] at [MoveSpeed]", category: "Action/Navigation", id: "704c056a6c235a2d3dd8e4e731f7a37d")]
    public partial class MoveToAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<Transform> Transform;
        [SerializeReference] public BlackboardVariable<AiMoveSpeed> MoveSpeed;
        private bool _hasArrived;

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

            AiBrain.MoveTo(Transform.Value.position, MoveSpeed.Value, DestinationReached);
            _hasArrived = false;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _hasArrived ? Status.Success : Status.Running;
        }

        private void DestinationReached()
        {
            _hasArrived = true;
        }
    }
}