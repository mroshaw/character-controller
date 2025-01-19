using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine.Splines;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class NavMeshSplineModifier : MonoBehaviour
    {
        #region Class Variables
        [BoxGroup("Settings")] public GameObject navMeshModifierPrefab; // Prefab with NavMeshModifierVolume
        [BoxGroup("Settings")] public float interval = 1f; // Interval to sample the spline
        [BoxGroup("Settings")] public Vector3 volumeSize = new Vector3(2f, 2f, 2f); // Size of the modifier volume
        // [BoxGroup("Settings")] public int splineAreaType = 1; // Set an area type ID (1 is commonly 'Walkable')
        #endregion

        #region Editor methods
        #if UNITY_EDITOR
        [Button("Refresh NavMeshModifiers")]
        internal void RefreshNavMeshModifiers()
        {
            UpdateNavMeshModifiers();
            NavMeshSplineManager.RefreshNavMeshSurfaces();
        }

        internal void UpdateNavMeshModifiers()
        {
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            if (splineContainer == null || navMeshModifierPrefab == null)
            {
                Debug.LogError("SplineContainer or NavMeshModifierVolume prefab is missing.");
                return;
            }

            // Destroy any existing volumes
            foreach (NavMeshModifierVolume navMeshModifierVolume in splineContainer.GetComponentsInChildren<NavMeshModifierVolume>(true))
            {
                DestroyImmediate(navMeshModifierVolume.gameObject);
            }

            foreach (Spline spline in splineContainer.Splines)
            {
                // Sample points along the spline
                for (float t = 0; t <= 1f; t += interval / spline.GetLength())
                {
                    Vector3 localPosition = spline.EvaluatePosition(t);
                    // Convert to world position
                    Vector3 worldPosition = transform.TransformPoint(localPosition);

                    if (Terrain.activeTerrain)
                    {
                        float terrainHeight = Terrain.activeTerrain.SampleHeight(worldPosition);
                        worldPosition.y = terrainHeight - 1.0f;
                    }

                    // Instantiate a NavMeshModifierVolume at the world position
                    GameObject modifierObj = (GameObject) PrefabUtility.InstantiatePrefab(navMeshModifierPrefab);
                    modifierObj.transform.SetParent(splineContainer.transform, true);
                    modifierObj.transform.position = worldPosition;
                    modifierObj.transform.localRotation = Quaternion.identity;
                    NavMeshModifierVolume modifier = modifierObj.GetComponent<NavMeshModifierVolume>();
                    if (modifier != null)
                    {
                        modifier.size = volumeSize;
                        // modifier.area = splineAreaType; // Assign area type
                    }
                }

            }
        }
        #endif
        #endregion

    }
}