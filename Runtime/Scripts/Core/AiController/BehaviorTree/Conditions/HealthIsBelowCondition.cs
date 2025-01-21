using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Health Is Below", story: "[Agent] health is below [HealthValue]", category: "Conditions/Detection",
        id: "796047cc49a721548f3cd8c360a9217a")]
    public partial class HealthIsBelowCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<float> HealthValue;

        public override bool IsTrue()
        {
            return true;
            // return AiBrain.TpCharacter.CurrentHealth < HealthValue.Value;
        }
    }
}