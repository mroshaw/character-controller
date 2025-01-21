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
    [CreateAssetMenu(fileName = "CharacterSpawnerSettings", menuName = "Daft Apple Games/Character/Character Spawner Settings", order = 1)]
    public class CharacterSpawnerSettings : ScriptableObject
    {
        [BoxGroup("Prefabs")] [AssetsOnly] public GameObject characterPrefab;
        [BoxGroup("Prefabs")] [AssetsOnly] public GameObject footstepPoolsPrefab;
    }
}