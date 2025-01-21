using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Can See", story: "[Agent] can see [Target] tagged [Tag]", category: "Conditions/Detection",
        id: "8726cdf612c610ebfe8addaa580d1b4b")]
    public partial class CanSeeTargetCondition : AiBrainCondition
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;
        [SerializeReference] public BlackboardVariable<string> Tag;

        public override void OnStart()
        {
            base.OnStart();
            if (AiBrain.DetectorManager == null)
            {
                Debug.LogError("DetectorManager is not attached to Behaviour agent!");
            }
        }

        public override bool IsTrue()
        {
            GameObject target = AiBrain.DetectorManager.GetClosestTargetWithTag(Tag.Value);
            if (target)
            {
                Target.Value = target.transform;
                return true;
            }

            return false;
        }
    }
}