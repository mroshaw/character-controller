using DaftAppleGames.Attributes;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AnimBehaviours
{
    public class RollBehaviour : CharacterBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private float animationExitDuration = 1.0f;
        private float _timeInRoll;
        private bool _rollComplete;

        #region State events

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timeInRoll = 0;
            _rollComplete = false;
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_rollComplete)
            {
                _timeInRoll += Time.deltaTime;
                if (_timeInRoll >= animationExitDuration)
                {
                    _rollComplete = true;
                    Debug.Log("Roll Completed!");
                    Character.RollComplete();
                }
            }
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }
        #endregion
    }
}