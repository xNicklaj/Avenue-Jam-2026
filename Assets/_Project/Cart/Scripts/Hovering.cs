using UnityEngine;

public class Hovering : MonoBehaviour
{
[Header("Hover Settings")]
    [Tooltip("How fast the object hovers up and down.")]
    public float hoverSpeed = 2f;
    
    [Tooltip("How high the object goes from its center point.")]
    public float hoverAmount = 0.5f;

    private Rigidbody _rb;
    private float _previousOffset;

    void Start()
    {
        // Cache the rigidbody and ensure it is set to kinematic
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; 
    }

    void FixedUpdate()
    {
        // 1. Calculate the sine wave offset using the current time
        float currentOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;

        // 2. Find the delta between this physics frame and the last one
        float deltaMovement = currentOffset - _previousOffset;

        // 3. Additively apply the movement using the Rigidbody's physics-safe method
        _rb.MovePosition(_rb.position + new Vector3(0f, deltaMovement, 0f));

        // 4. Save the current offset to use next physics frame
        _previousOffset = currentOffset;
    }
}