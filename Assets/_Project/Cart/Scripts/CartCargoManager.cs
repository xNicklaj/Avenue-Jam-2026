using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using UnityEngine;
using UnityEngine.Events;

public class CartCargoManager : MonoBehaviour
{
    [System.Serializable]
    public class CargoEvent : UnityEvent<GameObject> { }

    [Header("Custom Unity Events")]
    public CargoEvent onCargoAdded;
    public CargoEvent onCargoRemoved;

    [Header("Settings")]
    public string cargoTag = "Cargo";

    public Dictionary<Item, int> cargoItems;

    private void Awake()
    {
        cargoItems = new Dictionary<Item, int>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(cargoTag)) return;
        
        // If it's already a child, do nothing
        if (other.transform.IsChildOf(transform)) return;

        // --- VR GRAB CHECK (CRITICAL) ---
        // You MUST check if the player is holding the object here.
        // Example for XR Interaction Toolkit:
        // if (other.TryGetComponent(out UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable grabObj))
        // {
        //     if (grabObj.isSelected) return; 
        // }
        // Replace the above with whatever framework you are using!

        if (other.TryGetComponent(out HVRGrabbable grabbable) && grabbable.IsHandGrabbed) return;

        StartCoroutine(AttachCargo(other));
    }

    private IEnumerator AttachCargo(Collider other)
    {
        yield return new WaitForFixedUpdate();
        if (other == null) yield break;

        // Parent and freeze
        other.transform.SetParent(transform);
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Dictionary Logic
        if (other.TryGetComponent(out CargoTag ct))
        {
            if (cargoItems.ContainsKey(ct.ItemData))
                cargoItems[ct.ItemData]++;
            else
                cargoItems.Add(ct.ItemData, 1);

            Debug.Log($"Added 1x {ct.ItemData.name} to the cargo. Total: {cargoItems[ct.ItemData]}");
        }

        onCargoAdded.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(cargoTag)) return;

        // THE FIX: If it is STILL a child, this is just a phantom physics exit. Ignore it!
        //if (other.transform.IsChildOf(transform)) return;

        // Remove the cart as the parent (mostly a fallback, as VR usually unparents it for you)
        other.transform.SetParent(null);

        // Reverse the physics changes
        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        
        // Dictionary Logic
        if (other.TryGetComponent(out CargoTag ct))
        {
            if (cargoItems.ContainsKey(ct.ItemData))
            {
                cargoItems[ct.ItemData]--; 
                
                if (cargoItems[ct.ItemData] <= 0)
                {
                    cargoItems.Remove(ct.ItemData); 
                }
                Debug.Log($"Removed 1x {ct.ItemData.name} from the cargo.");
            }
        }

        onCargoRemoved.Invoke(other.gameObject);
    }
}