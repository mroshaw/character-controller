#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.TpCharacterController.PlayerController
{
    [DefaultExecutionOrder(-2)]
    public class SystemActionsInput : MonoBehaviour, PlayerControls.ISystemControlsActions
    {
        #region Class Variables

        [BoxGroup("Events")] [SerializeField] public UnityEvent pausePressedEvent;

        #endregion

        private PlayerControls _playerControls;

        #region Startup

        private void OnEnable()
        {
            // StartCoroutine(RegisterWithInputManagerAsync());

            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.Enable();
            }

            _playerControls.SystemControls.Enable();
            _playerControls.SystemControls.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (_playerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            _playerControls.SystemControls.Disable();
            _playerControls.SystemControls.RemoveCallbacks(this);
        }

        #endregion

        #region Input Callbacks

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                pausePressedEvent.Invoke();
            }
        }

        #endregion
    }
}