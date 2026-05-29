using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Slot in your Movement Settings ScriptableObject here.")]
    public MovementSettings settings;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;
    
    public bool isMoving => controller.isGrounded && GetNormalizedSpeed() > 0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (settings == null) return;
        
        HandleMovement();
        HandleGravityAndJump();
        
        Vector3 finalVelocity = currentVelocity + verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        bool isSprinting = sprintAction.action.IsPressed();

        float targetSpeed = isSprinting ? settings.sprintSpeed : settings.walkSpeed;

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        if (moveDirection.magnitude > 1f) 
        {
            moveDirection.Normalize();
        }

        Vector3 targetVelocity = moveDirection * targetSpeed;

        // Weight simulation: Lerp toward the target speed
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, settings.acceleration * Time.deltaTime);
    }

    private void HandleGravityAndJump()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; 
        }

        verticalVelocity.y += settings.gravity * Time.deltaTime;
    }
    
    public float GetCurrentSpeedMagnitude()
    {
        return Mathf.Abs(currentVelocity.magnitude);
    }

    public float GetNormalizedSpeed()
    {
        return Mathf.Abs(currentVelocity.magnitude / settings.walkSpeed);
    }

    public Vector2 GetDirectionVector()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(controller.velocity);
        return new Vector2(localVelocity.x, localVelocity.z).normalized;
    }
}