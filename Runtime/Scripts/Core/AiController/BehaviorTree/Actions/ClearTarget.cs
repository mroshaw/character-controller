using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Clear Target", story: "Clear [Target]", category: "Action/Detection", id: "253fdde0be26f2c53f8ab51ae4e1a63d")]
    public partial class ClearTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        protected override Status OnStart()
        {
            Target.ObjectValue = null;
            return Status.Success;
        }
    }
}