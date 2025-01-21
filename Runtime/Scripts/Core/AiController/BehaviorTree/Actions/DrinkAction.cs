using System;
using Unity.Behavior;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Drink", story: "[Agent] drinks", category: "Action/Needs",
        id: "e0244217a628d45716b10b2f14860411")]
    public partial class DrinkAction : AiBrainAction
    {
        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return AiBrain.Drink(1.0f) ? Status.Success : Status.Running;
        }
    }
}