using System;
using PrimeTween;
using UnityEngine;
using VInspector;

[RequireComponent(typeof(MeshRenderer))]
public class EmissionController : MonoBehaviour
{
    private static readonly int emissionMultiplier = Shader.PropertyToID("_EmissionMultiplier");
    private static readonly int transparency = Shader.PropertyToID("_Transparency");
    [Range(0f, 1f)] public float MaxEmission = 1f;
    public bool UseTransparency = true;
    [ShowIf("UseTransparency")] 
    public float MaxTransparency = 1f;

    [EndIf] 
    [Range(0f, 1f)] public float Emission = 1f;

    public TweenSettings Settings;
    
    private MeshRenderer _renderer;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    [OnValueChanged("Emission")]
    public void SetEmissionProxy() => SetEmission(Emission);
    
    public void SetEmission(float f)
    {
        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(emissionMultiplier, f * MaxEmission);
        _propertyBlock.SetFloat(transparency, f * MaxTransparency);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    public void LerpTo(float value, float time)
    {
        Tween.Custom(Emission, value, Settings, (f) => SetEmission(f));
    }
}
