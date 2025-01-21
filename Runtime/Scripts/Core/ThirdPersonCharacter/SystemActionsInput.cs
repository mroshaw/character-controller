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

        #region Startup

        private void OnEnable()
        {
            // StartCoroutine(RegisterWithInputManagerAsync());

            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.SystemControls.Enable();
            PlayerInputManager.Instance.PlayerControls.SystemControls.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.SystemControls.Disable();
            PlayerInputManager.Instance.PlayerControls.SystemControls.RemoveCallbacks(this);
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

        private IEnumerator RegisterWithInputManagerAsync()
        {
            while (PlayerInputManager.Instance == null || PlayerInputManager.Instance.PlayerControls == null)
            {
                yield return null;
            }

            Debug.Log("SystemActionsInpt: Found PlayerCharacterInputManager!");
            PlayerInputManager.Instance.PlayerControls.SystemControls.Enable();
            PlayerInputManager.Instance.PlayerControls.SystemControls.SetCallbacks(this);
        }

        #endregion
    }
}