using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using Unity.AI.Navigation;
using Unity.Behavior;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class NavMeshSplineManager : MonoBehaviour
    {
        #region Editor methods

#if UNITY_EDITOR
        [Button("Refresh All Modifiers")]
        private void RefreshAllModifiers()
        {
            foreach (NavMeshSplineModifier modifier in GetComponentsInChildren<NavMeshSplineModifier>(false))
            {
                modifier.UpdateNavMeshModifiers();
            }

            RefreshNavMeshSurfaces();
        }

        internal static void RefreshNavMeshSurfaces()
        {
            foreach (NavMeshSurface surface in FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None))
            {
                surface.BuildNavMesh();
            }
        }
#endif

        #endregion
    }
}