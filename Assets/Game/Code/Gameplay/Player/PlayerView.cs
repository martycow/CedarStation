using System;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerView : BaseView<PlayerContext>
    {
        [SerializeField, AutoAssign] private CharacterController characterController;
        
        public override void UpdateView()
        {
            if (Context.MoveInput == Vector2.zero)
                return;

            var moveDirection = Context.MoveInput.ToVector3();
            characterController.Move(moveDirection * Context.MoveSpeed * Time.deltaTime);
        }
    }
}