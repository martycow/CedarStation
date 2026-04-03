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
        public event Action<InputStateType> OnStateChanged;
        public event Action<InputDeviceType> OnDeviceChanged;
        
        public InputStateType CurrentStateType { get; private set; }
        public InputDeviceType CurrentDevice { get; private set; }
        public readonly Dictionary<InputStateType, IInputState> States = new();
        
        private readonly ICedarLogger _logger;

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
            foreach (var (_, state) in States)
                state.OnDeviceChanged += DeviceChangeHandler;
        }

        public void Dispose()
        {
            foreach (var (_, state) in States)
                state.OnDeviceChanged -= DeviceChangeHandler;
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
        
        private void DeviceChangeHandler(InputDeviceType deviceType)
        {
            if (CurrentDevice == deviceType)
                return;
            
            CurrentDevice = deviceType;
            _logger.Info(SystemTag.Input, $"Input device changed to: {CurrentDevice}");
            OnDeviceChanged?.Invoke(deviceType);
        }
    }
}
