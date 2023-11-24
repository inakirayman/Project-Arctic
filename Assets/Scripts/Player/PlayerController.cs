using ProjectArtic.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _walkSpeed = 2.0f;
    [SerializeField] private float _sprintMultiplier = 1.3f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] float _gravityValue = -9.81f;

    private InputManager _inputManager;
    private Transform _cameraTrasform;

    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
   
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _inputManager = InputManager.Instance;
        _cameraTrasform = Camera.main.transform;
    }

    void Update()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        MovementLogic();
        JumpLogic();
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    private void JumpLogic()
    {
        if (_inputManager.PlayerJumpedCurrentFrame() && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }
    }

    private void MovementLogic()
    {
        Vector2 movement = _inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = _cameraTrasform.forward * move.z + _cameraTrasform.right * move.x;
        move.y = 0f;
        


        if (!_inputManager.PlayerSprinting())
        {
            _controller.Move(move * Time.deltaTime * _walkSpeed);
        }
        else
        {
            _controller.Move(move * Time.deltaTime * (_walkSpeed * _sprintMultiplier));
        }
    }
}
