using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AnimBehaviours
{
    public class RollBehaviour : CharacterBehaviour
    {
        #region State events
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (HasAdditionalAbilities)
            {
                CharacterAbilities.StopRolling();
            }
        }
        #endregion
    }
}