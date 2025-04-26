using DaftAppleGames.TpCharacterController.AiController;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasClosestTarget", story: "[Agent] has [ClosestTarget]", category: "Conditions", id: "924dadfb1fa42920d2ef61a1e8e8f352")]
public partial class HasClosestTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<ClosestTargets> ClosestTarget;

    public override bool IsTrue()
    {
        return ClosestTarget.Value.HasAnyTarget();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}