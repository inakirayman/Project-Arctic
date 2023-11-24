using ProjectArtic.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float _sensitivityX = 400;
    [SerializeField] private float _sensitivityY = 400;
    [SerializeField] private Transform _playerOrientation;
    [SerializeField] private Vector2 _viewAngleMinMax = new Vector2(-90, 90);

    private float _xRotation;
    private float _yRotation;

    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = InputManager.Instance;

    }

    private void Update()
    {
        float lookX = _inputManager.GetLookDelta().x * Time.deltaTime * _sensitivityX;
        float lookY = _inputManager.GetLookDelta().y * Time.deltaTime * _sensitivityY;

        _yRotation += lookX;
        _xRotation -= lookY;

        _xRotation = Mathf.Clamp(_xRotation, _viewAngleMinMax.x, _viewAngleMinMax.y);
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        _playerOrientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }

}
