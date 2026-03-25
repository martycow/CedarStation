using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CedarStation.Input
{
    public class InputManager : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions inputActions;
        private InputSystem_Actions.PlayerActions playerActionMap;
        
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            playerActionMap = inputActions.Player;
            playerActionMap.AddCallbacks(this);
            inputActions.Enable();
        }

        private void OnEnable()
        {
            playerActionMap.Enable();
        }

        private void OnDisable()
        {
            playerActionMap.Disable();
        }

        private void OnDestroy()
        {
            inputActions.Dispose();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            
            context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}