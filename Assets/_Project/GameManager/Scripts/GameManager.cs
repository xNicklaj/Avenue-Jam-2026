using System;
using Dev.Nicklaj.Butter;
using ImprovedTimers;
using UnityEngine;
using VInspector;

public class GameManager : MonoBehaviour
{
    [Tooltip("Game Duration in minutes.")]
    public float GameDuration = 20f;
    public FloatVariable TimeVariable;
    public bool AutoStart = true;
    
    private CountdownTimer _timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _timer = new CountdownTimer(GameDuration * 60f);
    }

    private void Start()
    {
        if(AutoStart) _timer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameTime();
    }

    public void UpdateGameTime()
    {
        if (!TimeVariable) return;
        TimeVariable.Value = GetCurrentTime();
    }

    [Button("Start Timer")]
    public void StartTimer() => _timer.Start();
    
    public float GetCurrentTime() => _timer.CurrentTime;
}
