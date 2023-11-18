
using UnityEngine;

namespace ProjectArtic.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls _playerControls;
        private void Awake()
        {
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
