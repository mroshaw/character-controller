using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Target Is Not Dead", story: "[Target] is not dead", category: "Conditions/Detection", id: "0cc35976ede15cef2bd815328cc74d09")]
    public partial class TargetIsNotDeadCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        public override bool IsTrue()
        {
            return true;
        }
    }
}