using System;
using Dev.Nicklaj.Butter;
using TabbyStudios;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

[RequireComponent(typeof(BoxCollider))]
public class PlayEventOnTriggerEnter : MonoBehaviour
{
    public UnityEvent Event;
    public GameEvent GameEvent;
    public LayerMask layerMask;
    public bool UseTag;
    [ShowIf("UseTag")]
    public string Tag;
    [EndIf] 
    public bool RaiseOnce = true;

    private bool HasBeenRaised = false;
    
    private void Awake()
    {
        var collider =  GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.includeLayers = layerMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (RaiseOnce && HasBeenRaised) return;
        if (UseTag && !other.gameObject.CompareTag(Tag)) return;
        HasBeenRaised = true;
        Event.Invoke();
        GameEvent?.Raise();
    }
}
