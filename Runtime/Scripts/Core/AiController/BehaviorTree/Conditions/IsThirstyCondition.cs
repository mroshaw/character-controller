using System;
using Unity.Behavior;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI IsThirsty", story: "[Agent] is thirsty", category: "Conditions/Needs",
        id: "515c33877af94ec88d8cc0aa86e2cbaf")]
    public partial class IsThirstyCondition : AiBrainCondition
    {
        public override bool IsTrue()
        {
            return AiBrain.IsThirsty();
        }
    }
}