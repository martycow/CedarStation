using System;
using System.Collections.Generic;
using Cedar.Core;
using Game.General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public sealed class InputManager : IInputManager
    {
        private const double DeviceSwitchCooldown = 0.3; 
        
        public event Action<InputStateType> OnStateChanged;
        public event Action<InputDeviceType> OnDeviceChanged;
        
        public InputStateType CurrentStateType { get; private set; }
        public InputDeviceType CurrentDevice { get; private set; }
        public readonly Dictionary<InputStateType, IInputState> States = new();
        
        private readonly ICedarLogger _logger;
        private double _lastDeviceSwitch;

        public InputManager(
            GameplayInputState gameplayInputState,
            MenuInputState menuInputEvents,
            NoControlState noControlState,
            ICedarLogger logger)
        {
            States[InputStateType.Gameplay] = gameplayInputState;
            States[InputStateType.Menu] = menuInputEvents;
            States[InputStateType.NoControl] = noControlState;
            _logger = logger;
        }

        public void Initialize()
        {
            InputSystem.onActionChange += OnActionChange;
        }

        public void Dispose()
        {
            InputSystem.onActionChange -= OnActionChange;
        }

        public void SetState(InputStateType stateType)
        {
            if (!States.ContainsKey(stateType))
            {
                _logger.Error(SystemTag.Input, $"State {stateType} not found in InputManager.");
                return;
            }
            
            States[CurrentStateType].Disable();
            CurrentStateType = stateType;
            States[CurrentStateType].Enable();
            
            _logger.Info(SystemTag.Input, $"State changed to: {CurrentStateType}");
            OnStateChanged?.Invoke(stateType);
        }
        
        public void SetDevice(InputDeviceType deviceType)
        {
            if (CurrentDevice == deviceType)
                return;

            // Cooldown to prevent frequent device changes
            var now = Time.realtimeSinceStartupAsDouble;
            if (now - _lastDeviceSwitch < DeviceSwitchCooldown)
                return;
            
            CurrentDevice = deviceType;
            _lastDeviceSwitch = now;
            
            _logger.Info(SystemTag.Input, $"Input device changed to: {CurrentDevice}");
            OnDeviceChanged?.Invoke(deviceType);
        }
        
        private void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed)
                return;

            if (obj is not InputAction action)
                return;

            var device = action.activeControl?.device;

            if (device == null || device.synthetic)
                return;
            
            var newDevice = device is Gamepad
                ? InputDeviceType.Gamepad
                : InputDeviceType.KeyboardMouse;
            
            SetDevice(newDevice);
        }
    }
}
