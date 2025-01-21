using System;
using UnityEngine;
using Unity.Behavior;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    public abstract class AiBrainCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        protected AiBrain AiBrain { get; private set; }

        public override void OnStart()
        {
            if (Agent.Value == null)
            {
                Debug.LogError("No Agent set on Condition!");
            }

            AiBrain = Agent.Value.GetComponent<AiBrain>();

            if (!AiBrain)
            {
                Debug.LogError("AiBrain is not attached to Behaviour agent!");
            }
        }
    }
}