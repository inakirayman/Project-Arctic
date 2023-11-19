using UnityEngine;

namespace ProjectArtic.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance;

        private PlayerControls _playerControls;
        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
           _playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }

        public Vector2 GetPlayerMovement()
        {
            return _playerControls.Player.Movement.ReadValue<Vector2>();
        }

        public Vector2 GetLookDelta()
        {
            return _playerControls.Player.Look.ReadValue<Vector2>();
        }

        public bool PlayerJumpedCurrentFrame()
        {
            return _playerControls.Player.Jump.triggered;
        }

        public bool PlayerSprinting()
        {
            return _playerControls.Player.Sprint.IsPressed();
        }
    }
}
