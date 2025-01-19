#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class CharacterSpawnManagerEvents : MonoBehaviour
    {
        #region Class Variables
        [FoldoutGroup("Player Events")] public UnityEvent PlayerSpawnedEvent;
        [FoldoutGroup("NPC Events")] public UnityEvent NPCSpawnedEvent;
        [FoldoutGroup("Enemy Events")] public UnityEvent EnemySpawnedEvent;
        [FoldoutGroup("Animal Events")] public UnityEvent AnimalSpawnedEvent;
        #endregion

        #region Start
        private void OnEnable()
        {
            if (!CharacterSpawnManager.Instance)
            {
                Debug.LogError("CharacterSpawnManagerEvents: There is no CharacterSpawnManager in the scene! Please add one!");
                return;
            }

            CharacterSpawnManager.Instance.playerSpawnedEvent.AddListener(PlayerSpawnedProxy);
        }

        private void OnDisable()
        {
            if (CharacterSpawnManager.Instance)
            {
                CharacterSpawnManager.Instance.playerSpawnedEvent.RemoveListener(PlayerSpawnedProxy);
            }
        }

        private void Start()
        {

        }
        #endregion

        #region Class Methods
        private void PlayerSpawnedProxy()
        {
            PlayerSpawnedEvent.Invoke();
        }
        #endregion
    }
}