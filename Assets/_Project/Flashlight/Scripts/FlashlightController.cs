using System;
using HurricaneVR.Framework.ControllerInput;
using PrimeTween;
using Sisus.Init;
using UnityEngine;
using VInspector;

public class FlashlightController : MonoBehaviour<EmissionController, Light>
{
    private EmissionController _emissionController;
    private Light _light;
    
    public float MaxLightIntensity = 0.2f;
    [Range(0f, 1f)] public float Value = 1f;
    public TweenSettings Settings;

    private Tween _tween;
    private bool state;
    
    protected override void Init(EmissionController emissionController, Light light)
    {
        _emissionController = emissionController;
        _light = light;
    }

    [OnValueChanged("Value")]
    public void SetIntensityProxy() => SetValue(Value);

    private void Start()
    {
        SetValue(0f);
        state = false;
    }
    
    public void SetValue(float value)
    {
        _light.intensity = value * MaxLightIntensity;
        _emissionController.SetEmission(value);
    }

    public void Update()
    {
        if (HVRInputManager.Instance.RightController.PrimaryButtonState.JustActivated) 
            Toggle();
    }

    [Button("Toggle")]
    public void Toggle()
    {
        if (state) TurnOff();
        else TurnOn();
    }

    [Button("Turn On")]
    public void TurnOn()
    {
        if (state) return;
        if (_tween.isAlive) return;

        _tween = Tween.Custom(0, 1, Settings, f => SetValue(f));
    }

    [Button("Turn Off")]
    public void TurnOff()
    {
        if (!state) return;
        if (_tween.isAlive) return;

        _tween = Tween.Custom(1, 0, Settings, f => SetValue(f));
    }
}
