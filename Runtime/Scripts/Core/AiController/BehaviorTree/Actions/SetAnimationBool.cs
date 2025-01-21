using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Set Animator Bool", story: "Set [AnimBoolParam] to [AnimState] on [Agent]", category: "Action/Animation", id: "c88cdeec261f1b053278dfafc7b55f45")]
    public partial class SetAnimationBoolAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<string> AnimBoolParam;
        [SerializeReference] public BlackboardVariable<bool> AnimState;

        protected override Status OnStart()
        {
            if (!Init())
            {
                return Status.Failure;
            }

            AiBrain.Animator.SetBool(AnimBoolParam.Value, AnimState.Value);
            return Status.Success;
        }
    }
}