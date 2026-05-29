using System;
using Sisus.Init;
using UnityEngine;

namespace _Project.CharacterController.Scripts
{
    public class PlayerAnimationController : MonoBehaviour<PlayerMovement, Animator>
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int DirectionForward = Animator.StringToHash("DirectionForward");
        private static readonly int DirectionRight = Animator.StringToHash("DirectionRight");
        private static readonly int NormalizedSpeed = Animator.StringToHash("NormalizedSpeed");

        
        private PlayerMovement playerMovementComponent;
        private Animator playerAnimator;
        
        protected override void Init(PlayerMovement playerMovementArgument, Animator playerAnimatorService)
        {
            playerMovementComponent = playerMovementArgument;
            playerAnimator = playerAnimatorService;
        }

        private void Update()
        {
            playerAnimator.SetFloat(Speed, playerMovementComponent.GetCurrentSpeedMagnitude());
            playerAnimator.SetFloat(NormalizedSpeed, playerMovementComponent.GetNormalizedSpeed());
            Vector2 dir = playerMovementComponent.GetDirectionVector();
            playerAnimator.SetFloat(DirectionForward, dir.y);
            playerAnimator.SetFloat(DirectionRight, dir.x);
        }


    }
}