#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerSpawnerSettings", menuName = "Daft Apple Games/Character/Player Spawner Settings", order = 1)]
    public class PlayerSpawnerSettings : CharacterSpawnerSettings
    {
        [BoxGroup("Prefabs")] [AssetsOnly] public GameObject cameraPrefab;
        [BoxGroup("Prefabs")] [AssetsOnly] public GameObject cmCameraRigPrefab;
    }
}