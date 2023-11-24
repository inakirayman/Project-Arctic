using ProjectArtic.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerInfo))]
public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private LayerMask _layer;

    private IsInteractable _currentInteractable;
    private PlayerInfo _playerInfo;
    private InputManager _inputManager; 

    void Start()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        _inputManager = InputManager.Instance;
        _playerCamera = Camera.main;
    }

    void Update()
    {
        // Raycast to check for interactable objects on the specified layer
        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _interactionRange, _layer))
        {
            
            IsInteractable interactable = hit.collider.GetComponent<IsInteractable>();

            
            _currentInteractable = interactable;
        }
        else
        {
            // No interactable object is currently looked at
            _currentInteractable = null;
        }

        // Check for interaction input (e.g., a button press)
        if (_inputManager.PlayerIntracting())
        {
            Debug.Log("Intract");
            TryInteract();
        }
    }

    void TryInteract()
    {
        // Check if there is a currently looked-at interactable object
        if (_currentInteractable != null)
        {
            // Call the Interact method on the currently looked-at interactable object
            _currentInteractable.Interact(_playerInfo);
        }
    }

    
}
