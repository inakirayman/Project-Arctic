using ProjectArtic.InputSystem;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    private InputManager _inputManager;
    private Rigidbody _rigidbody;
    private FixedJoint _playerJoint;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    private float _moveInput;
    private float _rotationInput;

    public bool _IsControlled = false;

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = InputManager.Instance;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _inputManager.GetPlayerMovement().y;
        _rotationInput = _inputManager.GetPlayerMovement().x;
    }

    private void FixedUpdate()
    {
        if (_IsControlled)
        {
            MoveVehicle(_moveInput);
            RotateVehicle(_rotationInput);
        }
    }

    private void MoveVehicle(float input)
    {
        Vector3 moveDirection = transform.forward * input * _moveSpeed * Time.deltaTime;
        _rigidbody.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
    }

    private void RotateVehicle(float input)
    {
        float rotation = input * _rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerJoint = other.gameObject.AddComponent<FixedJoint>();
            _playerJoint.connectedBody = _rigidbody;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(_playerJoint);
        }
    }
}
