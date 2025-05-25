using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "AI Rotate To Target", story: "[Agent] rotates to [Target]", category: "Action/Navigation", id: "0a25791c3fc13c4d19bf2c45479771e8")]
    public partial class RotateToTargetAction : AiBrainAction
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        private float _progress = 0.0f;
        private Quaternion m_StartRotation;
        private Quaternion m_EndRotation;

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
            AiBrain.Character.RotateTowards((Target.Value.position - AiBrain.transform.position).normalized, Time.deltaTime, false);
            float dot = Vector3.Dot(AiBrain.transform.forward, (Target.Value.position - AiBrain.transform.position).normalized);
            return dot > 0.7f ? Status.Success : Status.Running;
        }
    }
}