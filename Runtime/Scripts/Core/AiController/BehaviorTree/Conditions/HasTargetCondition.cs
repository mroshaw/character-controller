using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Has Target", story: "[Agent] has [Target]", category: "Conditions/Detection", id: "0cbbe0c462bca3261a6b740e8fb2a5b0")]
    public partial class HasTargetCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        public override bool IsTrue()
        {
            if (Target.Type.IsValueType)
            {
                return false;
            }

            return !(Target.ObjectValue is null || Target.ObjectValue.Equals(null));
        }
    }
}