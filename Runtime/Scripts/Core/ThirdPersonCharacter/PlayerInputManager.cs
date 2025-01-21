using DaftAppleGames.Utilities;
using UnityEngine;

namespace DaftAppleGames.TpCharacterController.PlayerController
{
    [DefaultExecutionOrder(-3)]
    public class PlayerInputManager : Singleton<PlayerInputManager>
    {
        public PlayerControls PlayerControls { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            InitPlayerControls();
        }

        private void InitPlayerControls()
        {
            if (PlayerControls == null)
            {
                PlayerControls = new PlayerControls();
                PlayerControls.Enable();
            }
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }

        public void PauseInput()
        {
            Debug.Log("Pausing input...");
            InitPlayerControls();
            PlayerControls.CharacterControls.Disable();
            PlayerControls.CameraControls.Disable();
        }

        public void UnpauseInput()
        {
            Debug.Log("Unpausing input...");
            InitPlayerControls();
            PlayerControls.CharacterControls.Enable();
            PlayerControls.CameraControls.Enable();
        }
    }
}