using ProjectArtic.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private MovementState _state;
    public enum MovementState
    {
        Walking,
        Sprinting,
        Air
    }



    [Header("Movement")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _groundDrag;
    private float _moveSpeed = 0;

    [Header("Jump")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    private bool _isJumpReady =true;

    [Header("Ground Check")]
    [SerializeField] private float _PlayerHeight;
    [SerializeField] private LayerMask _ground;
    private bool _isGrounded = false;

    [SerializeField] private Transform _orientation;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private Rigidbody _rb;

    private InputManager _inputManager;
    private float _horizontalInput;
    private float _verticalInput;
    

    void Start()
    {
        _inputManager = InputManager.Instance;
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, (_PlayerHeight / 2) + 0.2f,_ground);

        GetInputs();
        SpeedControle();
        StateHandler();

        if (_isGrounded)
        {
            _rb.drag = _groundDrag;
        }
        else
        {
            _rb.drag = 0;
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if (_isGrounded && _inputManager.PlayerSprinting())
        {
            _state = MovementState.Sprinting;
            _moveSpeed = _sprintSpeed;
        }
        else if (_isGrounded)
        {
            _state = MovementState.Walking;
            _moveSpeed = _walkSpeed;
        }
        else
        {
            _state = MovementState.Air;
        }
    }


    private void MovePlayer()
    {
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        if (_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else if (!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }
        
    }

    private void SpeedControle()
    {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if(flatVelocity.magnitude > _moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
        }

    }

    private void GetInputs()
    {
        _horizontalInput = _inputManager.GetPlayerMovement().x;
        _verticalInput = _inputManager.GetPlayerMovement().y;

        if(_inputManager.PlayerJumpedCurrentFrame() && _isJumpReady && _isGrounded)
        {
            _isJumpReady = false;
            Jump();
            Invoke(nameof(ResetJump), _jumpCooldown);
        }

    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.y);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        _isJumpReady = true;
    }


}
