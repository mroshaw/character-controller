using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Health Is Above", story: "[Agent] health is above [HealthValue]", category: "Conditions/Detection",
        id: "b33739f5ce379ffc6487e9b7d307ead6")]
    public partial class HealthIsAboveCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<float> HealthValue;

        public override bool IsTrue()
        {
            return AiBrain.TpCharacter.CurrentHealth >= HealthValue.Value;
        }
    }
}