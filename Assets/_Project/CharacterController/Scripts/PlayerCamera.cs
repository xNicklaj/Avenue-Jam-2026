using _Project.CharacterController.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Sisus.Init;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Slot in your Camera Settings ScriptableObject here.")]
    public CameraSettings settings;

    [Header("References")]
    public Transform cameraPivot; 
    
    [Header("Input")]
    public InputActionReference lookAction;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (settings == null) return;

        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        // Read Vector2 from the New Input System
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        // Multiply by sensitivity and deltaTime for smooth, frame-independent movement
        float mouseX = lookInput.x * settings.sensitivityX * Time.deltaTime;
        float mouseY = lookInput.y * settings.sensitivityY * Time.deltaTime;

        // Calculate and clamp vertical rotation (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -settings.verticalLookLimit, settings.verticalLookLimit);

        // Apply vertical rotation to the pivot
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Apply horizontal rotation (Yaw) directly to the player body
        transform.Rotate(Vector3.up * mouseX);
    }
}

