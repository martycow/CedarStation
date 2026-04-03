using System;
using Cedar.Core;
using Game.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public sealed class MenuInputState : BaseInputState, IMenuInputEvents, 
        InputActions.IMenuActions
    {
        public event Action<Vector2> Navigate;
        public event Action Confirm;
        public event Action Back;
        public event Action CloseMenu;
        
        public override InputStateType StateType => InputStateType.Menu;

        public MenuInputState(InputActions inputActions, ICedarLogger logger) : base(inputActions, logger) { }
        
        public override void Initialize()
        {
            InputActions.Menu.SetCallbacks(this);
            Disable();
        }

        public override void Dispose()
        {
            InputActions.Menu.SetCallbacks(null);
        }

        public override void Enable()
        {
            InputActions.Menu.Enable();
        }

        public override void Disable()
        {
            InputActions.Menu.Disable();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    var value = context.ReadValue<Vector2>();
                    Logger.Info(SystemTag.Input, $"Navigate: {value}");
                    Navigate?.Invoke(value);
                    break;
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    var value = context.ReadValue<Vector2>();
                    Logger.Info(SystemTag.Input, $"Point: {value}");
                    Navigate?.Invoke(value);
                    break;
            }
        }

        public void OnConfirm(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, $"Confirm");
                    Confirm?.Invoke();
                    break;
            }
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, $"Back");
                    Back?.Invoke();
                    break;
            }
        }

        public void OnCloseMenu(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Logger.Info(SystemTag.Input, $"CloseMenu");
                    CloseMenu?.Invoke();
                    break;
            }
        }
    }
}