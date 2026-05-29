using UnityEngine;

public class Hovering : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("Target distance from the ground.")]
    public float hoverHeight = 1.0f;
    public float hoverSpeed = 2f;
    public float hoverAmount = 0.15f;

    [Tooltip("How fast it lowers if it is higher than the target height.")]
    public float descentSpeed = 3f;

    [Tooltip("How strongly it resists tipping over and rights itself (Higher = faster).")]
    public float rightingSpeed = 5f;

    [Tooltip("How far down to check for the floor.")]
    public float floorCheckDistance = 3f;

    public LayerMask GroundMask;
    
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false; 
    }

    void FixedUpdate()
    {
        // --- 1. HOVER & HEIGHT LOGIC ---
        Vector3 rayStart = transform.position + (Vector3.up * 0.5f);
        
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, floorCheckDistance, layerMask: GroundMask))
        {
            _rb.useGravity = false;

            float targetY = hit.point.y + hoverHeight + (Mathf.Sin(Time.time * hoverSpeed) * hoverAmount);
            float heightDifference = targetY - _rb.position.y;

            float requiredVelocityY;

            if (heightDifference > 0)
            {
                // We are below the target height: Use instant upward correction to hold the weight
                requiredVelocityY = heightDifference / Time.fixedDeltaTime;
            }
            else
            {
                // We are above the target height: Use smooth proportional velocity to gently float down
                requiredVelocityY = heightDifference * descentSpeed; 
            }

            Vector3 currentVelocity = _rb.linearVelocity;
            currentVelocity.y = requiredVelocityY;
            _rb.linearVelocity = currentVelocity;
        }
        else
        {
            _rb.useGravity = true;
        }

        // --- 2. ROTATION STABILIZATION LOGIC ---
        // Capture the current rotation, but force the X and Z tilt to be completely flat (0)
        Quaternion flatTargetRotation = Quaternion.Euler(0f, _rb.rotation.eulerAngles.y, 0f);
        
        // Smoothly blend (Slerp) from our current tilted rotation towards the flat target rotation
        _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, flatTargetRotation, Time.fixedDeltaTime * rightingSpeed));
    }
}