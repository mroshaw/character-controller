using DaftAppleGames.TpCharacterController.AiController;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasTargetWithTargetType", story: "[Agent] has [Target] with [TargetType]", category: "Conditions", id: "737ba11cb617005b28f17ea5940e039b")]
public partial class HasTargetWithTargetTypeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<ClosestTargets> Target;
    [SerializeReference] public BlackboardVariable<TargetType> TargetType;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}