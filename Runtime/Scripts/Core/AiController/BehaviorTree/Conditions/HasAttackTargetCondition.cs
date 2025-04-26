using DaftAppleGames.TpCharacterController.AiController;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasAttackTarget", story: "Agent has [AttackTarget]", category: "Conditions", id: "cfe372088c765e3d7bf4b14dbbaadb64")]
public partial class HasAttackTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<ClosestTargets> AttackTarget;

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
