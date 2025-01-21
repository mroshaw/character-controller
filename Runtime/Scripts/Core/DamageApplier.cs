using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class DamageApplier : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Damage Settings")] [SerializeField] private float damageDealt = 10.0f;

        #endregion

        #region Class methods

        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageDealt);
            }
        }

        #endregion
    }
}