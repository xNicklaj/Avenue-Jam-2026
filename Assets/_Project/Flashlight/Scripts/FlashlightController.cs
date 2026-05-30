using System;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using PrimeTween;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class FlashlightController : MonoBehaviour<EmissionController, Light, HVRGrabbable>
{
    private EmissionController _emissionController;
    private Light _light;
    private HVRGrabbable _grabbable;
    
    public float MaxLightIntensity = 0.2f;
    [Range(0f, 1f)] public float Value = 1f;
    public TweenSettings Settings;

    private Tween _tween;
    [ShowInInspector, ReadOnly] private bool state;

    public UnityEvent OnTurnOn;
    public UnityEvent OnTurnOff;
    
    protected override void Init(EmissionController emissionController, Light light, HVRGrabbable grabbable)
    {
        _emissionController = emissionController;
        _light = light;
        _grabbable = grabbable;
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
        if (HVRInputManager.Instance.RightController.PrimaryButtonState.JustActivated && _grabbable.IsHandGrabbed) 
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
        state = true;
        OnTurnOn?.Invoke();
    }

    [Button("Turn Off")]
    public void TurnOff()
    {
        if (!state) return;
        if (_tween.isAlive) return;

        _tween = Tween.Custom(1, 0, Settings, f => SetValue(f));
        state = false;
        OnTurnOff?.Invoke(); 
    }
}
