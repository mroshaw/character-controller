using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Actions
{
    public abstract class AiBrainAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected AiBrain AiBrain { get; private set; }

        protected bool Init()
        {
            AiBrain = Agent.Value.GetComponent<AiBrain>();
            if (AiBrain == null)
            {
                LogFailure("No AIBrain found on agent.");
                return false;
            }
            return true;
        }
    }
}