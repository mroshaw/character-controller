using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController
{
    public class DamageManager : MonoBehaviour
    {
        #region Class Variables
        [BoxGroup("Settings")] [SerializeField] private DamageApplier[] damageAppliers;
        [BoxGroup("Settings")] [SerializeField] private bool refreshAppliersOnStart;
        #endregion

        #region Startup
        private void Start()
        {
            if (refreshAppliersOnStart)
            {
                RefreshAppliers();
            }

            DisableDamageAppliers();
        }

        [Button("Refresh Appliers")]
        private void RefreshAppliers()
        {
            damageAppliers = GetComponentsInChildren<DamageApplier>(true);
        }
        #endregion

        #region Class methods

        public void EnableDamageAppliers()
        {
            foreach (DamageApplier damageApplier in damageAppliers)
            {
                damageApplier.enabled = true;
            }
        }

        public void DisableDamageAppliers()
        {
            foreach (DamageApplier damageApplier in damageAppliers)
            {
                damageApplier.enabled = false;
            }
        }
        #endregion
    }
}