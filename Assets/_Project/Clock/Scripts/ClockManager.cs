using Dev.Nicklaj.Butter;
using TMPro;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI TextAsset;
    public FloatVariable TimeVariable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
    }

    public void UpdateClock()
    {
        if (!TimeVariable) return;
        TextAsset.text = FormatTime(TimeVariable.Value);
    }
    
    public static string FormatTime(float secondsFloat)
    {
        // Calculate whole minutes and leftover seconds
        int minutes = Mathf.FloorToInt(secondsFloat / 60f);
        int seconds = Mathf.FloorToInt(secondsFloat % 60f);
        
        // Format the string to ensure two digits (e.g., "02")
        return $"{minutes:00}:{seconds:00}";
    }
}

