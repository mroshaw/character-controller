using System.Collections.Generic;
using CodiceApp;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaftAppleGames.TpCharacterController.Editor
{
    [CustomEditor(typeof(AnimationPresets))]
    public class AnimationPresetsEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset rootVisualElement;
        private VisualElement _inspectorTree;
        private AnimationPresets _target;

        private void OnEnable()
        {
            _target = target as AnimationPresets;
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (!rootVisualElement)
            {
                return null;
            }

            _inspectorTree = new VisualElement();
            rootVisualElement.CloneTree(_inspectorTree);

            Button initTargetButton = _inspectorTree.Q<Button>("InitSettingsButton");
            initTargetButton.RegisterCallback<ClickEvent>(evt => { InitSettings(); });

            Button updateSettingsButton = _inspectorTree.Q<Button>("UpdateSettingsButton");
            updateSettingsButton.RegisterCallback<ClickEvent>(evt => { UpdateSettings(); });

            Button validateButton = _inspectorTree.Q<Button>("ValidateButton");
            validateButton.RegisterCallback<ClickEvent>(evt => { Validate(); });

            Button sortButton = _inspectorTree.Q<Button>("SortButton");
            sortButton.RegisterCallback<ClickEvent>(evt => { Sort(); });

            return _inspectorTree;
        }

        private void InitSettings()
        {
            _target.InitSettings();
        }

        private void UpdateSettings()
        {
            _target.UpdateMappings();
        }

        private void Validate()
        {
            if (!_target.Validate(out List<string> validationErrors))
            {
                Debug.LogError("Errors found!");
                foreach (string validationError in validationErrors)
                {
                    Debug.LogError(validationError);
                }
            }
            else
            {
                Debug.Log("No errors found!");
            }
        }

        private void Sort()
        {
            _target.Sort();
            Repaint();
        }
    }

    [CustomPropertyDrawer(typeof(AnimationPresets.AnimPreset))]
    public class AnimMapping_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            // Create a new VisualElement to be the root the property UI.
            VisualElement container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Auto
                }
            };

            SerializedProperty animTargetProperty = property.FindPropertyRelative("animMapping");
            SerializedProperty mirrorProperty = property.FindPropertyRelative("mirrorAnimation");
            SerializedProperty animSpeedProperty = property.FindPropertyRelative("animSpeed");
            SerializedProperty animClipProperty = property.FindPropertyRelative("animClip");

            // Derive the label from the AnimTarget properties
            if (animTargetProperty != null)
            {
                string animTargetHeader = animTargetProperty.FindPropertyRelative("animationHeader").stringValue;
                string animTargetName = animTargetProperty.FindPropertyRelative("animationName").stringValue;
                string entryLabelText = $"{animTargetHeader}/{animTargetName}:";

                bool isEntryMapped = animClipProperty.objectReferenceValue != null;

                Debug.Log($"Is Mapped: {isEntryMapped}");

                if (isEntryMapped)
                {
                    container.Add(new PropertyField(property.FindPropertyRelative("animClip"), entryLabelText)
                    {
                        style =
                        {
                            width = new StyleLength(600),
                            paddingRight = new StyleLength(10),
                        }
                    });
                }
                else
                {
                    container.Add(new PropertyField(property.FindPropertyRelative("animClip"), entryLabelText)
                    {
                        style =
                        {
                            width = new StyleLength(600),
                            paddingRight = new StyleLength(10),
                            backgroundColor = new Color(0.5f, 0, 0, 0.5f)
                        }
                    });
                }
            }
            else
            {
                container.Add(new PropertyField(property.FindPropertyRelative("animClip"), "ERROR!"));
            }

            container.Add(new Label("Mirror:")
            {
                style =
                {
                    paddingRight = new StyleLength(10),
                }
            });
            container.Add(new PropertyField(mirrorProperty, ""));
            container.Add(new Label("Speed:")
            {
                style =
                {
                    paddingLeft = new StyleLength(10),
                }
            });
            container.Add(new PropertyField(animSpeedProperty, ""));

            // Return the finished UI.
            return container;
        }
    }
}