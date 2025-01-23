#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#else
using DaftAppleGames.Attributes;
#endif

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaftAppleGames.TpCharacterController.Editor
{
    #if ODIN_INSPECTOR
    public class FixMixamoAnimationsEditorWindow : OdinEditorWindow
    #else
    public class FixMixamoAnimationsEditorWindow : EditorWindow
    #endif
    {
        [BoxGroup("Settings")] [SerializeField] private VisualTreeAsset tree;
        [BoxGroup("Settings")] private ModelImporterAnimationType modelImportType = ModelImporterAnimationType.Human;
        [BoxGroup("Settings")] private Avatar animAvatar;

        [MenuItem("Daft Apple Games/Characters/Fix Mixamo Animations")]
        public static void ShowWindow()
        {
            FixMixamoAnimationsEditorWindow window = GetWindow<FixMixamoAnimationsEditorWindow>();
            window.titleContent = new GUIContent("Fix Mixamo Animations");
        }

        public void CreateGUI()
        {
            tree.CloneTree(rootVisualElement);

            rootVisualElement.Q<EnumField>("ImportType").RegisterValueChangedCallback(evt =>
            {
                modelImportType = (ModelImporterAnimationType)evt.newValue;
            });

            rootVisualElement.Q<ObjectField>("Avatar").RegisterValueChangedCallback(evt =>
            {
                animAvatar = evt.newValue as Avatar;
            });


            Button fixButton = rootVisualElement.Q<Button>("FixButton");

            fixButton.RegisterCallback<ClickEvent>(evt =>
            {
                FixSelectedAssets();
            });
        }

        [Button("Fix Selected Assets")]
        private void FixSelectedAssets()
        {
            GameObject[] updatedGameObjects = Selection.gameObjects;

            foreach (GameObject gameObject in updatedGameObjects)
            {
                FixAnimProperties(gameObject);
            }
        }

        private void FixAnimProperties(GameObject fbxAsset)
        {
            if (!animAvatar)
            {
                Debug.LogError("You must select a source avatar!");
                return;
            }

            string fbxAssetName = fbxAsset.name;
            string path = AssetDatabase.GetAssetPath(fbxAsset);
            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            if (modelImporter == null)
            {
                Debug.LogError($"Failed to load ModelImporter for the specified FBX file: {fbxAssetName}");
                return;
            }

            modelImporter.animationType = ModelImporterAnimationType.Human;
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
            modelImporter.sourceAvatar = animAvatar;
            modelImporter.importTangents = ModelImporterTangents.None;
            modelImporter.importAnimation = true;



            if (modelImporter.clipAnimations.Length != 1)
            {
                Debug.LogError($"There must be only one animation clip per FBX file. {fbxAssetName} has {modelImporter.clipAnimations.Length}.");
                return;
            }

            Debug.Log($"Processing: {fbxAssetName}...");

            ModelImporterClipAnimation[] newAnimClips = modelImporter.defaultClipAnimations;

            ModelImporterClipAnimation animClip = newAnimClips[0];

            animClip.name = fbxAssetName;
            animClip.keepOriginalOrientation = true;
            animClip.keepOriginalPositionY = true;
            animClip.keepOriginalPositionXZ = true;

            animClip.rotationOffset = 0;
            animClip.lockRootRotation = false;


            modelImporter.clipAnimations = newAnimClips;

            Debug.Log($"Saving: {fbxAssetName}...");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Debug.Log($"Done processing: {fbxAssetName}.");
        }

        private void SavePrefabChanges(GameObject updatedGameObject)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(updatedGameObject))
            {
                UnityEditor.EditorUtility.SetDirty(updatedGameObject);
                AssetDatabase.SaveAssetIfDirty(updatedGameObject);
            }
        }

        /// <summary>
        /// If any of the Selection is a Prefab, mark as dirty and force a save
        /// </summary>
        private void SaveAllPrefabChanges(GameObject[] updatedGameObjects)
        {
            foreach (GameObject updatedGameObject in updatedGameObjects)
            {
                SavePrefabChanges(updatedGameObject);
            }
        }
    }
}