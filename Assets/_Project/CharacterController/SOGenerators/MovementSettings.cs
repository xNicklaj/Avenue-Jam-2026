using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementSettings", menuName = "RPG/Movement Settings")]
public class MovementSettings : ScriptableObject
{
    [Tooltip("Normal walking speed.")]
    public float walkSpeed = 2.5f;
    [Tooltip("Sprinting speed.")]
    public float sprintSpeed = 5.5f;
    [Tooltip("Lower values mean heavier, slower acceleration.")]
    public float acceleration = 6f; 
    
    [Header("Physics")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;
}