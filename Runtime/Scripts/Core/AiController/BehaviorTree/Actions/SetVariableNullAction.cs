using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Set Variable Null", story: "Clear [Variable]", category: "Action/Blackboard", id: "712c2a49cfcbf9fafdf24f3d4f4193b8")]
    public partial class SetVariableNullAction : Action
    {
        [SerializeReference] public BlackboardVariable Variable;

        protected override Status OnStart()
        {
            Variable.ObjectValue = null;
            return Status.Success;
        }
    }
}