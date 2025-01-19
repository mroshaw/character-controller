using ECM2;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using Unity.Behavior;
using UnityEngine.AI;
using UnityEngine.Events;

namespace DaftAppleGames.TpCharacterController.Spawning
{
    public class CharacterSpawnable : MonoBehaviour, ISpawnable
    {
        #region Class Variables
        [FoldoutGroup("Events")] public UnityEvent spawnEvent;
        [FoldoutGroup("Events")] public UnityEvent despawnEvent;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private NavMeshCharacter _navMeshCharacter;
        private BehaviorGraphAgent _behaviourGraphAgent;

        public Spawner Spawner { get; set; }

        #endregion
        #region Startup

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshCharacter = GetComponent<NavMeshCharacter>();
            _behaviourGraphAgent = GetComponent<BehaviorGraphAgent>();
        }
        #endregion
        #region Class Methods

        public void PreSpawn()
        {
            DisableComponents();
        }

        public void Spawn()
        {
            EnableComponents();
            spawnEvent.Invoke();
        }

        public void Despawn()
        {
            DisableComponents();
            despawnEvent.Invoke();
        }

        private void DisableComponents()
        {
            _behaviourGraphAgent.enabled = false;
            _navMeshCharacter.enabled = false;
            _navMeshAgent.enabled = false;
        }

        private void EnableComponents()
        {
            _navMeshAgent.enabled = true;
            _navMeshCharacter.enabled = true;
            _behaviourGraphAgent.enabled = true;
        }
        #endregion
    }
}