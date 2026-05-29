using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UprightRotationLock : MonoBehaviour
{
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 1. Kill any tipping momentum the VR hands (or collisions) try to add
        Vector3 currentAngularVel = _rb.angularVelocity;
        currentAngularVel.x = 0f;
        currentAngularVel.z = 0f;
        _rb.angularVelocity = currentAngularVel;

        // 2. Force the object to stay perfectly flat, only allowing it to turn left/right (Y axis)
        Vector3 currentEuler = _rb.rotation.eulerAngles;
        _rb.MoveRotation(Quaternion.Euler(0f, currentEuler.y, 0f));
    }
}