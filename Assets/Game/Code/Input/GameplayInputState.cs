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
        public event Action<Vector2> OnPlayerMoveChanged;
        public event Action<Vector2> OnCameraMoveChanged;
        public event Action<float> OnCameraZoomChanged;
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
            switch (context.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                case InputActionPhase.Canceled:
                    var value = context.ReadValue<Vector2>();
                    OnPlayerMoveChanged?.Invoke(value);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Jump");
                    Jump?.Invoke();
                    break;
            }
        }
        #endregion

        #region Look
        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                case InputActionPhase.Started:
                case InputActionPhase.Canceled:
                    var value = context.ReadValue<Vector2>();
                    OnCameraMoveChanged?.Invoke(value);
                    break;
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
            }
        }

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                case InputActionPhase.Canceled:
                    var value = context.ReadValue<float>();
                    OnCameraZoomChanged?.Invoke(value);
                    break;
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
            }
        }
        #endregion

        #region Actions
        public void OnTool(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case  InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Tool");
                    Tool?.Invoke();
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Interact");
                    Interact?.Invoke();
                    break;
            }
        }

        public void OnKick(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Kick");
                    Kick?.Invoke();
                    break;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Crouch");
                    Crouch?.Invoke();
                    break;
            }
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, "Open Menu");
                    OpenMenu?.Invoke();
                    break;
            }
        }
        #endregion
    }
}