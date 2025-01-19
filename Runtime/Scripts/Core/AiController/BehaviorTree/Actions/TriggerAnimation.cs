using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Set Animator Trigger", story: "Trigger anim [Trigger] in [Agent]", category: "Action/Animation")]
    public partial class TriggerAnimationAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<string> Trigger;

        protected override Status OnStart()
        {
            if(!Init())
            {
                return Status.Failure;
            }
            AiBrain.Animator.SetTrigger(Trigger.Value);
            return Status.Success;
        }
    }
}