using System;
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.Darskerry.Editor.CharacterConfiguration
{
    public class ForceTPose
    {
        [MenuItem("Daft Apple Games/Characters/Force T pose")]
        private static void TPose()
        {
            var selected = Selection.activeGameObject;

            if (!selected) return; // If no object was selected, exit.
            if (!selected.TryGetComponent(out Animator animator))
                return; // If the selected object has no animator, exit.
            if (!animator.avatar) return;

            var
                skeletonbones = animator.avatar?.humanDescription.skeleton; // Get the list of bones in the armature.

            foreach (var sb in skeletonbones) // Loop through all bones in the armature.
                foreach (HumanBodyBones hbb in Enum.GetValues(typeof(HumanBodyBones)))
                    if (hbb != HumanBodyBones.LastBone)
                    {
                        var bone = animator.GetBoneTransform(hbb);
                        if (bone != null)
                            if (sb.name == bone
                                    .name) // If this bone is a normal humanoid bone (as opposed to an ear or tail bone), reset its transform.
                            {
                                // The bicycle pose happens when for some reason the transforms of an avatar's bones are incorectly saved in a state that is not the t-pose.
                                // For most of the bones this affects only their rotation, but for the hips, the position is affected as well.
                                // As the scale should be untouched, and the user may have altered these intentionally, we should leave them alone.
                                if (hbb == HumanBodyBones.Hips) bone.localPosition = sb.position;
                                bone.localRotation = sb.rotation;

                                //bone.localScale = sb.scale;
                                // An alternative to setting the values above would be to revert each bone to its prefab state like so:
                                // RevertObjectOverride(boneT.gameObject, InteractionMode.UserAction); // InteractionMode.UserAction should save the changes to the undo history.
                                // Though this may only work if the object actually is a prefab, and it would overwrite any user changes to scale or position, and who knows what else.
                                break; // We found a humanbodybone that matches, so we need not check the rest against this skeleton bone.
                            }
                    }
        }
    }
}