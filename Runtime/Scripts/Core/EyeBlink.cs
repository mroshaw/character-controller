using System.Collections;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.AiController
{
    public class EyeBlink : MonoBehaviour
    {
        #region Class Variables
        [SerializeField] private float minBlinkWait = 1.0f;
        [SerializeField] private float maxBlinkWait = 3.0f;

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
            _isBlinking = true;
            StartCoroutine(BlinkAsync());
        }

        private void OnDisable()
        {
            _isBlinking = false;
        }
        #endregion

        #region Update Logic
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