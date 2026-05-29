using UnityEngine;
using UnityEngine.Events;

public class CartCargoManager : MonoBehaviour
{
    // Custom event class that passes the GameObject that was thrown in
    [System.Serializable]
    public class CargoEvent : UnityEvent<GameObject> { }

    [Header("Custom Unity Events")]
    public CargoEvent onCargoAdded;
    public CargoEvent onCargoRemoved;

    [Header("Settings")]
    [Tooltip("Only objects with this tag will attach to the cart.")]
    public string cargoTag = "Cargo";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object thrown in is actually cargo
        
        if (!other.CompareTag(cargoTag)) return;
        
        // Parent the object to the cart so it moves with it
        other.transform.SetParent(transform);
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Fire your custom event
        onCargoAdded.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // Use the same clean early return here!
        if (!other.CompareTag(cargoTag)) return;

        // Remove the cart as the parent
        other.transform.SetParent(null);

        // Reverse the physics changes so it can fall/bounce normally again
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // Fire your custom event
        onCargoRemoved.Invoke(other.gameObject);
    }
}