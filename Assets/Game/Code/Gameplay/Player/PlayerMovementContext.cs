using System;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerMovementContext
    {
        public event Action OnJumpRequested;

        public readonly ConstParam<float> Height;
        public readonly Param<Vector2> Speed;
        public readonly Param<float> JumpCooldown;
        public readonly Param<float> JumpForce;

        public PlayerMovementContext(
            float height,
            float jumpCooldown,
            float jumpForce)
        {
            Height = new ConstParam<float>(height);
            Speed = new Param<Vector2>();
            JumpCooldown = new Param<float>(jumpCooldown);
            JumpForce = new Param<float>(jumpForce);
        }
        
        public void RequestJump()
        {
            OnJumpRequested?.Invoke();
        }
    }
}