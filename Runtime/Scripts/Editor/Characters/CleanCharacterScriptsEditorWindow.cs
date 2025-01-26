#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.Editor
{
#if ODIN_INSPECTOR
    public class CleanCharacterScriptsEditorWindow : OdinEditorWindow
#else
    public class CleanCharacterScriptsEditorWindow : EditorWindow
#endif
    {
        [MenuItem("Daft Apple Games/Characters/Clean Character Scripts")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CleanCharacterScriptsEditorWindow));
        }

        [SerializeField] private bool disableInsteadOfDestroyCore = false;
        [SerializeField] private bool disableInsteadOfDestroyCollider = true;
        [SerializeField] private bool disableInsteadOfDestroyAnimator = false;

        [SerializeField] private GameObject[] selectedGameObject;

        private void OnSelectionChange()
        {
            selectedGameObject = Selection.gameObjects;
        }

#if ODIN_INSPECTOR
        [Button("Clean Scripts", ButtonSizes.Large), GUIColor(0, 1, 0)]
#else
        [Button("Clean Scripts")]
#endif
        private void CleanScripts()
        {
            foreach (GameObject currGameObject in selectedGameObject)
            {
                RemoveScripts(currGameObject);
            }
        }

#if PHYSIC_BONES
        private void RemovePhysicBonesScripts(GameObject currGameObject)
        {
            // Physics Bone
            PhysicBonesCore[] physicsBones = currGameObject.GetComponentsInChildren<PhysicBonesCore>(true);
            foreach (PhysicBonesCore physicBonesCore in physicsBones)
            {
                if (disableInsteadOfDestroyCore)
                {
                    physicBonesCore.enabled = false;
                }
                else
                {
                    DestroyImmediate(physicBonesCore);
                }
            }

            // Physics Bone Collider
            PhysicBonesCollider[] physicsBoneColliders = currGameObject.GetComponentsInChildren<PhysicBonesCollider>(true);
            foreach (PhysicBonesCollider physicBoneCollider in physicsBoneColliders)
            {
                if (disableInsteadOfDestroyCollider)
                {
                    physicBoneCollider.enabled = false;
                }
                else
                {
                    DestroyImmediate(physicBoneCollider);
                }
            }

        }
#endif

        private void RemoveScripts(GameObject currGameObject)
        {
            // Get prefab asset
            string prefabPath = "";
            GameObject instance;
            if (PrefabUtility.IsPartOfAnyPrefab(currGameObject))
            {
                prefabPath = AssetDatabase.GetAssetPath(currGameObject);
                instance = PrefabUtility.LoadPrefabContents(prefabPath);
            }
            else
            {
                instance = currGameObject;
            }

#if PHYSIC_BONES
            RemovePhysicBonesScripts(instance);
#endif

            // Animator
            Animator[] animators = instance.GetComponentsInChildren<Animator>(true);
            foreach (Animator animator in animators)
            {
                if (disableInsteadOfDestroyAnimator)
                {
                    animator.enabled = false;
                }
                else
                {
                    DestroyImmediate(animator);
                }
            }


            if (PrefabUtility.IsPartOfAnyPrefab(currGameObject))
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            }
        }
    }
}