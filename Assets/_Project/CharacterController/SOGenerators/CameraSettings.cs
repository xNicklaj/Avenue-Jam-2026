using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraSettings", menuName = "RPG/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Tooltip("Horizontal look sensitivity.")]
    public float sensitivityX = 15f;
    [Tooltip("Vertical look sensitivity.")]
    public float sensitivityY = 15f;
    [Tooltip("Maximum angle the player can look up or down.")]
    public float verticalLookLimit = 80f;

    public NoiseSettings walkingNoise;
}