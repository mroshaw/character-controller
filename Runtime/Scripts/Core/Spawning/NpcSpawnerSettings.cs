#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    [CreateAssetMenu(fileName = "NPCSpawnerSettings", menuName = "Daft Apple Games/Character/NPC Spawner Settings", order = 1)]
    public class NpcSpawnerSettings : CharacterSpawnerSettings
    {
        [BoxGroup("Prefabs")][AssetsOnly] public GameObject npcPrefab;
    }
}