#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#else
using DaftAppleGames.Attributes;
#endif

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaftAppleGames.TpCharacterController.Editor
{

#if ODIN_INSPECTOR
    public class AnimationPresetsEditorWindow : OdinEditorWindow
    #else
    public class AnimationPresetsEditorWindow : EditorWindow
    #endif
    {
        [BoxGroup("Settings")] [SerializeField] private VisualTreeAsset tree;
        [BoxGroup("Settings")] [SerializeField] private AnimationPresets animPresets;

        [SerializeField] private Animator selectedAnimator;

        [MenuItem("Daft Apple Games/Characters/Animation Presets Editor")]
        public static void ShowWindow()
        {
            AnimationPresetsEditorWindow window = GetWindow<AnimationPresetsEditorWindow>();
            window.titleContent = new GUIContent("Animation Presets");
        }

        public void CreateGUI()
        {
            tree.CloneTree(rootVisualElement);

            rootVisualElement.Q<ObjectField>("AnimPresets").RegisterValueChangedCallback(evt =>
            {
                animPresets = evt.newValue as AnimationPresets;
            });

            Button updateButton = rootVisualElement.Q<Button>("UpdateAnimatorButton");

            updateButton.RegisterCallback<ClickEvent>(evt =>
            {
                ApplyPresetsToSelected();
            });
        }

        [Button("Apply Presets")]
        private void ApplyPresetsToSelected()
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;

            if (selectedGameObjects.Length == 0)
            {
                Debug.LogError("You must select a GameObject with an Animator attached!");
                return;
            }

            Animator animator = selectedGameObjects[0].GetComponent<Animator>();

            if (!animator)
            {
                Debug.LogError("You must select a GameObject with an Animator attached!");
                return;
            }

            if (!animPresets)
            {
                Debug.LogError("You must select a Animation Preset!");
                return;
            }

            UpdateAnimator(animator);
        }

        private void OnSelectionChange()
        {
            if (Selection.gameObjects.Length == 0)
            {
                selectedAnimator = null;
                return;
            }
            if (Selection.gameObjects[0].TryGetComponent(out Animator animator))
            {
                selectedAnimator = animator;
            }
        }

        private void UpdateAnimator(Animator animator)
        {
            AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;
            animPresets.UpdateAllAnims(controller);
        }
    }
}