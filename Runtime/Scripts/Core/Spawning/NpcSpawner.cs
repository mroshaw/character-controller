#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class NpcSpawner : CharacterSpawner
    {
        #region Class Variables

        [PropertyOrder(10)]
        [FoldoutGroup("Events")] public UnityEvent NpcReadyEvent;

        [PropertyOrder(99)] [FoldoutGroup("DEBUG")] [SerializeField] private GameObject npcTestObject;

        NpcSpawnerSettings _npcSpawnerSettings;

        #endregion

        #region Startup

        protected override void Awake()
        {
            base.Awake();
            _npcSpawnerSettings = spawnSettings as NpcSpawnerSettings;
        }

        #endregion

        #region Class Methods

        protected override bool SpawnInstances()
        {
            base.SpawnInstances();
            return IsValidSpawn;
        }


        protected override void Configure()
        {
            base.Configure();
        }

        protected override void SetSpawnsActive()
        {
            base.SetSpawnsActive();
        }

        #endregion
    }
}