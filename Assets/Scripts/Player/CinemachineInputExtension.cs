using Cinemachine;
using ProjectArtic.InputSystem;
using UnityEngine;

public class CinemachineInputExtension : CinemachineExtension
{

    [SerializeField] private float _horizontalSpeed = 10f;
    [SerializeField] private float _verticalSpeed = 10f;
    [SerializeField] private float _clampAngle = 80f;
    private InputManager _inputManager;
    private Vector3 _startingRotation;
    protected override void Awake()
    {
        _inputManager = InputManager.Instance;
        _startingRotation = transform.localRotation.eulerAngles;
        base.Awake();
    }


    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if(stage == CinemachineCore.Stage.Aim)
            {
                if (_startingRotation == null) 
                { 
                    _startingRotation = transform.localRotation.eulerAngles;
                }
                Vector2 deltaInput = _inputManager.GetLookDelta();
                _startingRotation.x += deltaInput.x * Time.deltaTime;
                _startingRotation.y += deltaInput.y * Time.deltaTime;
                _startingRotation.y = Mathf.Clamp(_startingRotation.y,-_clampAngle, _clampAngle);
                state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
            }
        }
    }
}
