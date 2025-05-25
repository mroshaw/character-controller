using System;
using System.Collections.Generic;
using System.Linq;
using DaftAppleGames.Attributes;
using Unity.Behavior;
#if ODIN_INSPECTOR
#endif
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    [BlackboardEnum]
    public enum TargetType { Friend, FleeEnemy, AttackEnemy, Ignore }

    [CreateAssetMenu(fileName = "TargetTagMappingSO", menuName = "Daft Apples Game/Character/Target Tag Mapping")]
    public class TargetTagMappingSO : ScriptableObject
    {
        [SerializeField] private TargetTagMapping[] targetTagMappings;

        /// <summary>
        /// Return a Dictionary of Tags with the given TargetType as key
        /// </summary>
        public Dictionary<TargetType,string[]> GetTagsByTarget(TargetType targetType)
        {
            Dictionary<TargetType,string[]> tagsByTarget = new Dictionary<TargetType,string[]>();

            foreach (TargetTagMapping targetTagMapping in targetTagMappings)
            {
                if (targetTagMapping.TargetType == targetType)
                {
                    tagsByTarget.Add(targetTagMapping.TargetType, targetTagMapping.Tags);
                }
            }
            return tagsByTarget;
        }

        public TargetType GetTargetTypeByTag(string tag)
        {
            foreach (TargetTagMapping targetTagMapping in targetTagMappings)
            {
                if (targetTagMapping.Tags.Contains(tag))
                {
                    return targetTagMapping.TargetType;
                }
            }
            return TargetType.Ignore;
        }

        /// <summary>
        /// Returns an array of unique TargetTypes
        /// </summary>
        /// <returns></returns>
        public TargetType[] GetTargetTypes()
        {
            List<TargetType> activeTargetTypes = new List<TargetType>();
            foreach (TargetTagMapping targetTagMapping in targetTagMappings)
            {
                if (!activeTargetTypes.Contains(targetTagMapping.TargetType))
                {
                    activeTargetTypes.Add(targetTagMapping.TargetType);
                }
            }
            return activeTargetTypes.ToArray();
        }

        public string[] GetTargetTags()
        {
            List<string> activeTargetTags = new List<string>();
            foreach (TargetTagMapping targetTagMapping in targetTagMappings)
            {
                foreach (string tag in targetTagMapping.Tags)
                {
                    if (!activeTargetTags.Contains(tag))
                    {
                        activeTargetTags.Add(tag);
                    }
                }
            }
            return activeTargetTags.ToArray();
        }
    }

    [Serializable]
    public struct TargetTagMapping
    {
        [SerializeField] private TargetType targetType;
        [SerializeField] [TagSelector] private string[] tags;

        public TargetType TargetType => targetType;
        public string[] Tags => tags;
    }
}