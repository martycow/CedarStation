using System;
using Cedar.Core;
using Game.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public sealed class GameplayInputState : BaseInputState, IGameplayInputEvents, 
        InputActions.IMovingActions,
        InputActions.IViewActions,
        InputActions.IActionsActions
    {
        public event Action<Vector2> MoveStarted;
        public event Action<Vector2> Move;
        public event Action<Vector2> MoveEnded;
        public event Action<Vector2> MoveCamera;
        public event Action<float> ZoomCamera;
        public event Action Jump;
        public event Action Tool;
        public event Action Interact;
        public event Action Kick;
        public event Action Crouch;
        public event Action OpenMenu;
        
        public override InputStateType StateType => InputStateType.Gameplay;
        
        public GameplayInputState(InputActions inputActions, ICedarLogger logger) : base(inputActions, logger) { }
        
        public override void Initialize()
        {
            InputActions.Moving.SetCallbacks(this);
            InputActions.View.SetCallbacks(this);
            InputActions.Actions.SetCallbacks(this);
            Disable();
            
            Logger.Success(SystemTag.Input, $"Initialized {StateType} Input State.");
        }

        public override void Dispose()
        {
            InputActions.Moving.SetCallbacks(null);
            InputActions.View.SetCallbacks(null);
            InputActions.Actions.SetCallbacks(null);
        }

        public override void Enable()
        {
            InputActions.Moving.Enable();
            InputActions.View.Enable();
            InputActions.Actions.Enable();
        }

        public override void Disable()
        {
            InputActions.Moving.Disable();
            InputActions.View.Disable();
            InputActions.Actions.Disable();
        }

        #region Moving
        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            TrackDevice(context);
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Logger.Info(SystemTag.Input, $"Move Started: {value}");
                    MoveStarted?.Invoke(value);
                    break;
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, $"Move: {value}");
                    Move?.Invoke(value);
                    break;
                case InputActionPhase.Canceled:
                    Logger.Info(SystemTag.Input, $"Move Ended: {value}");
                    MoveEnded?.Invoke(value);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Jump");
                Jump?.Invoke();
            }
        }
        #endregion

        #region Look
        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                var value = context.ReadValue<Vector2>();
                Logger.Info(SystemTag.Input, $"Move Camera: {value}");
                MoveCamera?.Invoke(value);
            }
        }

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                var value = context.ReadValue<float>();
                Logger.Info(SystemTag.Input, $"Zoom Camera: {value}");
                ZoomCamera?.Invoke(value);
            }
        }
        #endregion

        #region Actions
        public void OnTool(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Tool");
                Tool?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Interact");
                Interact?.Invoke();
            }
        }

        public void OnKick(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Kick");
                Kick?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Crouch");
                Crouch?.Invoke();
            }
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            TrackDevice(context);
            if (context.phase == InputActionPhase.Performed)
            {
                Logger.Info(SystemTag.Input, "Open Menu");
                OpenMenu?.Invoke();
            }
        }
        #endregion
    }
}