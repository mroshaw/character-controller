using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Is Within Distance", story: "[Agent] is within [Distance] units of [Target]",
        category: "Conditions/Detection", id: "83105e70eec53693a338d433f7662949")]
    public partial class IsWithinDistanceCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<float> Distance;
        [SerializeReference] public BlackboardVariable<Transform> Target;

        public override bool IsTrue()
        {
            if (Target.Value == null)
            {
                Debug.LogError("IsWithinDistanceCondition Target is null");
                return false;
            }

            return (Agent.Value.transform.position - Target.Value.transform.position).magnitude < Distance.Value;
        }
    }
}