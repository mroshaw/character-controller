using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AnimBehaviours
{
    public class RollBehaviour : CharacterBehaviour
    {
        #region State events

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterAbilities.RollComplete();
            base.OnStateExit(animator, stateInfo, layerIndex);
        }
        #endregion
    }
}