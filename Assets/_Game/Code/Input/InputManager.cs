using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CedarStation.Input
{
    public class InputManager : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _inputActions;
        private InputSystem_Actions.PlayerActions _playerActionMap;
        
        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _playerActionMap = _inputActions.Player;
            _playerActionMap.AddCallbacks(this);
            _inputActions.Enable();
        }

        private void OnEnable()
        {
            _playerActionMap.Enable();
        }

        private void OnDisable()
        {
            _playerActionMap.Disable();
        }

        private void OnDestroy()
        {
            _inputActions.Dispose();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            context.ReadValueAsButton();
        }
    }
}