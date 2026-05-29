using System;
using UnityEngine;
using VInspector;

public class EmissionController : MonoBehaviour
{
    [Range(0f, 1f)] public float MaxEmission = 1f;
    public bool UseTransparency = true;
    [ShowIf("UseTransparency")] 
    public float MaxTransparency = 1f;
    [EndIf]
    
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }
}
