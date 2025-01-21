using System.Collections;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class EyeBlink : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Settings")] [SerializeField] private float minBlinkWait = 1.0f;
        [BoxGroup("Settings")] [SerializeField] private float maxBlinkWait = 3.0f;

        private static readonly int Blink = Animator.StringToHash("Blink");

        private Animator _animator;

        private bool _isBlinking = false;

        #endregion

        #region Startup

        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            _isBlinking = true;
            StartCoroutine(BlinkAsync());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _isBlinking = false;
        }

        #endregion

        #region Class methods

        private IEnumerator BlinkAsync()
        {
            while (_isBlinking)
            {
                yield return new WaitForSeconds(Random.Range(minBlinkWait, maxBlinkWait));
                _animator.SetTrigger(Blink);
            }
        }

        #endregion
    }
}