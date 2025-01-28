#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#else
using DaftAppleGames.Attributes;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.Editor
{
    [CreateAssetMenu(fileName = "AnimationPreset", menuName = "Daft Apple Games/Character/Animation Presets", order = 1)]
    public class AnimationPresets : ScriptableObject
    {
        [BoxGroup("Animation Targets")] public AnimationMappings animMappings;
        [BoxGroup("Animation Mapping")] public AnimPreset[] animPresets;

        public void UpdateMappings()
        {
            List<AnimPreset> tempAnimMappingList = animPresets.ToList();

            foreach (AnimationMappings.AnimMapping currAnimTarget in animMappings.animMappings)
            {
                Debug.Log($"Looking for: {currAnimTarget.animationHeader}/{currAnimTarget.animationName}");
                if (!DoesTargetExist(currAnimTarget))
                {
                    tempAnimMappingList.Add(new AnimPreset(currAnimTarget));
                }
            }

            animPresets = tempAnimMappingList.ToArray();
        }


        [Button("Sort")]
        public void Sort()
        {
            Array.Sort(animPresets);
        }

        private bool DoesTargetExist(AnimationMappings.AnimMapping animMapping)
        {
            foreach (AnimPreset currAnimMapping in animPresets)
            {
                if (currAnimMapping.AnimMapping == animMapping)
                {
                    return true;
                }
            }

            return false;
        }

        public void InitSettings()
        {
            if (!animMappings || animMappings.animMappings.Length == 0)
            {
                return;
            }

            int numMappings = animMappings.animMappings.Length;
            animPresets = new AnimPreset[numMappings];

            for (int currMappingIndex = 0; currMappingIndex < numMappings; currMappingIndex++)
            {
                animPresets[currMappingIndex] = new AnimPreset(animMappings.animMappings[currMappingIndex]);
            }
        }

        public void UpdateAllAnims(AnimatorController animatorController)
        {
            foreach (AnimPreset currAnimMapping in animPresets)
            {
                currAnimMapping.UpdateInController(animatorController);
            }
        }

        public bool Validate(out List<string> validationErrors)
        {
            bool validationResult = true;

            validationErrors = new();
            foreach (AnimPreset currAnimPreset in animPresets)
            {
                if (!currAnimPreset.animClip)
                {
                    validationErrors.Add($"Animation clip is missing: {currAnimPreset.AnimMapping.AnimLabel}");
                    validationResult = false;
                }

                if (currAnimPreset.animSpeed == 0)
                {
                    validationErrors.Add($"Animation has 0 speed: {currAnimPreset.AnimMapping.AnimLabel}");
                    validationResult = false;
                }
            }

            return validationResult;
        }

        [Serializable]
        public class AnimPreset : IComparable<AnimPreset>
        {
            public AnimationClip animClip;
            public bool mirrorAnimation;
            public float animSpeed;
            [SerializeField] private AnimationMappings.AnimMapping animMapping;

            public bool IsMapped => animClip == null;

            public AnimationMappings.AnimMapping AnimMapping => animMapping;

            public AnimPreset(AnimationMappings.AnimMapping mapping, AnimationClip clip)
            {
                animClip = clip;
                animMapping = mapping;
                mirrorAnimation = false;
                animSpeed = 1.0f;
            }

            public AnimPreset(AnimationMappings.AnimMapping mapping)
            {
                animClip = null;
                animMapping = mapping;
                mirrorAnimation = false;
                animSpeed = 1.0f;
            }

            public int CompareTo(AnimPreset otherAnimPreset)
            {
                if (otherAnimPreset == null) return 1;

                // Compare each property in the specified order
                int comparison = string.Compare(animMapping.animationHeader, otherAnimPreset.animMapping.animationHeader, StringComparison.Ordinal);
                if (comparison != 0) return comparison;
                comparison = string.Compare(animMapping.blendTreeName, otherAnimPreset.animMapping.blendTreeName, StringComparison.Ordinal);
                if (comparison != 0) return comparison;

                // Finally, compare blendTreeIndex numerically
                return animMapping.blendTreeIndex.CompareTo(otherAnimPreset.animMapping.blendTreeIndex);
            }

            public void UpdateInController(AnimatorController animatorController)
            {
                if (animMapping == null)
                {
                    Debug.LogError($"CharacterAnimationSettings: AnimTarget is null!");
                    return;
                }

                if (animClip == null)
                {
                    Debug.LogError($"CharacterAnimationSettings: AnimClip is empty on: {animMapping.AnimLabel}");
                    return;
                }

                animMapping.ApplyAnimSettings(animatorController, animClip, mirrorAnimation, animSpeed, true);
            }
        }
    }
}