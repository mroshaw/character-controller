using System;
using Unity.Behavior;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Patrol", story: "[Agent] patrols waypoints", category: "Action/Navigation", id: "8016e055e781d0a76aedfc22d3146316")]
    public partial class PatrolAction : AiBrainAction
    {
        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            AiBrain.Patrol();
            return Status.Success;
        }
    }
}