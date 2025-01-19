using UnityEngine;

namespace DaftAppleGames.TpCharacterController
{
    public interface IDamageable
    {
        public bool IsDead();
        public void TakeDamage(float damage);
        public void RestoreHealth(float health);
    }
}