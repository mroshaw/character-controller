using System;
using Unity.Behavior;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Wander", story: "[Agent] wanders", category: "Action/Navigation", id: "79530ae655e4c01296bec8f3939d187e")]
    public partial class WanderAction : AiBrainAction
    {
        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            AiBrain.Wander();
            return Status.Success;
        }
    }
}