using System;
using Unity.Cinemachine;
using UnityEngine;
using Sisus.Init;

namespace _Project.CharacterController.Scripts
{
    public class PlayerCameraNoiseManager : MonoBehaviour<CinemachineBasicMultiChannelPerlin, PlayerMovement, PlayerCamera>
    {
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
        private PlayerMovement playerMovementComponent;
        private PlayerCamera playerCameraComponent;
        
        protected override void Init(CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlinService, PlayerMovement playerMovementService, PlayerCamera playerCameraService)
        {
            cinemachineBasicMultiChannelPerlin = cinemachineBasicMultiChannelPerlinService;
            playerMovementComponent = playerMovementService;
            playerCameraComponent = playerCameraService;
        }
        
        private void UpdateCameraNoise()
        {
            if (playerMovementComponent == null) return;

            if (playerMovementComponent.GetNormalizedSpeed() > 0.1f)
            {
                cinemachineBasicMultiChannelPerlin.FrequencyGain = playerMovementComponent.GetNormalizedSpeed();
                SetNoiseState(NoiseState.Walking);
            }
            else
                SetNoiseState(NoiseState.Idle);
        }
    
        public void SetNoiseState(NoiseState state)
        {
            switch (state)
            {
                case NoiseState.Idle:
                    cinemachineBasicMultiChannelPerlin.NoiseProfile = null;
                    break;
                case NoiseState.Walking:
                    cinemachineBasicMultiChannelPerlin.NoiseProfile = playerCameraComponent.settings.walkingNoise;
                    break;
            }
        }

        private void Update()
        {
            UpdateCameraNoise();
        }
    }
}

public enum NoiseState
{
    Idle,
    Walking,
}