using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Is Outside Distance", story: "[Agent] is outside [Distance] units from [Target]",
        category: "Conditions/Detection", id: "f3a7a9d40c83b91008d999961277c84d")]
    public partial class IsOutsideDistanceCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<float> Distance;
        [SerializeReference] public BlackboardVariable<Transform> Target;

        public override bool IsTrue()
        {
            float distanceToTarget = (Agent.Value.transform.position - Target.Value.transform.position).magnitude;
            return  distanceToTarget > Distance.Value;
        }
    }
}