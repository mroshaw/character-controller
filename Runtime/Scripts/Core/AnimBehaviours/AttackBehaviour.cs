using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AnimBehaviours
{
    public class AttackBehaviour : CharacterBehaviour
    {
        private DamageManager _damageManager;

        #region State events

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (!_damageManager)
            {
                _damageManager = animator.GetComponent<DamageManager>();
            }

            if (_damageManager)
            {
                _damageManager.EnableDamageAppliers();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (_damageManager)
            {
                _damageManager.DisableDamageAppliers();
            }

            Character.StopAttacking();
    }

        #endregion
    }
}