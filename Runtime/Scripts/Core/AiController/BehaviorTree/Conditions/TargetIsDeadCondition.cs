using System;
using Unity.Behavior;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController.BehaviourTree.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "AI Target Is Dead", story: "[Target] is dead", category: "Conditions/Detection", id: "d5c47986fce649512068b39ccfe2acf5")]
    public partial class TargetIsDeadCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        Character _character;

        public override void OnStart()
        {
            base.OnStart();
            _character = Target.Value.GetComponent<Character>();
            if (!_character)
            {
                Debug.LogError("Checking for Dead on a Target that has no GameCharacter attached");
            }
        }

        public override bool IsTrue()
        {
            return true;
            // return _character.IsDead();
        }
    }
}