using System;
using Unity.Behavior;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Has Patrol Route", story: "[Agent] has a Patrol Route", category: "Conditions/Navigation",
        id: "7d590140f7684dd8c4607439cb520623")]
    public partial class HasPatrolRouteCondition : AiBrainCondition
    {
        public override bool IsTrue()
        {
            return AiBrain.HasPatrolRoute();
        }
    }
}