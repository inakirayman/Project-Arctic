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
        Crouching,
        Air
    }

    [Header("Movement")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _groundDrag;
    private float _moveSpeed = 0;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    private bool _isJumpReady =true;

    [Header("Crouching")]
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _startYScale;
    [SerializeField] private float _crouchYScale;

    [Header("Ground Check")]
    [SerializeField] private float _PlayerHeight;
    [SerializeField] private LayerMask _ground;
    private bool _isGrounded = false;

    [Header("SlopeHandling")]
    [SerializeField] private float _maxSlapeAngle;
    private RaycastHit _slopeHit;
    private bool _exitingSlop = false;




    #region Head Bob
    [Header("Head Bob")]
    public bool _enableHeadBob = true;
    [SerializeField] private float _bobSpeed = 10f;
    [SerializeField] private Vector3 _bobAmount = new Vector3(.15f, .05f, 0f);

   
    [SerializeField] private Transform _joint;
    private Vector3 _jointOriginalPos;
    private float _timer = 0;
    private bool _isWalking = true;
    #endregion

    [Header("Referneces")]
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
        _startYScale = transform.localScale.y;
        _jointOriginalPos = _joint.localPosition;
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

        if (_enableHeadBob)
        {
            HeadBob();
        }
    }

    private void StateHandler()
    {
        if (_inputManager.PlayerCrouching())
        {
            _state = MovementState.Crouching;
            _moveSpeed = _crouchSpeed;
            return;
        }

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

        if (_moveDirection == Vector3.zero)
        {
            _isWalking = false;
        }
        else
        {
            _isWalking = true;
        }

        if (OnSlope() && !_exitingSlop)
        {
            _rb.AddForce(GetSlopeMoveDirection() * _moveSpeed * 20f, ForceMode.Force);
            if(_rb.velocity.y > 0)
            {
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else if (!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }


        _rb.useGravity = !OnSlope();
    }

    private void SpeedControle()
    {
        if (OnSlope() && !_exitingSlop)
        {
            if (_rb.velocity.magnitude > _moveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVelocity.magnitude > _moveSpeed)
             {
                Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
             }
        }
       

    }

    private void GetInputs()
    {
        //Looking
        _horizontalInput = _inputManager.GetPlayerMovement().x;
        _verticalInput = _inputManager.GetPlayerMovement().y;

        //Jumping
        if(_inputManager.PlayerJumpedCurrentFrame() && _isJumpReady && _isGrounded)
        {
            _isJumpReady = false;
            Jump();
            Invoke(nameof(ResetJump), _jumpCooldown);
        }

        //Crouching
        if (_inputManager.PlayerCrouchingStart())
        {
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (_inputManager.PlayerCrouchingEnd())
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private void Jump()
    {
        _exitingSlop = true;

        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.y);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        _isJumpReady = true;
        _exitingSlop = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down, out _slopeHit, (_PlayerHeight / 2) + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlapeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
        
    }

    private void HeadBob()
    {
        if (_isWalking)
        {
            if(_state == MovementState.Walking)
            {
                _timer += Time.deltaTime * _bobSpeed;
            }
            else if(_state == MovementState.Sprinting)
            {
                _timer += Time.deltaTime * (_bobSpeed + _moveSpeed);
            }
            else if(_state == MovementState.Crouching)
            {
                _timer += Time.deltaTime * (_bobSpeed * 0.5f);
            }

            // Applies HeadBob movement
            _joint.localPosition = new Vector3(_jointOriginalPos.x + Mathf.Sin(_timer) * _bobAmount.x, _jointOriginalPos.y + Mathf.Sin(_timer) * _bobAmount.y, _jointOriginalPos.z + Mathf.Sin(_timer) * _bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            _timer = 0;
            _joint.localPosition = new Vector3(Mathf.Lerp(_joint.localPosition.x, _jointOriginalPos.x, Time.deltaTime * _bobSpeed), Mathf.Lerp(_joint.localPosition.y, _jointOriginalPos.y, Time.deltaTime * _bobSpeed), Mathf.Lerp(_joint.localPosition.z, _jointOriginalPos.z, Time.deltaTime * _bobSpeed));
        }
    }
}
