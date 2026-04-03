using System;
using Cedar.Core;

namespace Game.General
{
     public interface IInputManager : IInitializable, IDisposable
    {
        event Action<InputStateType> OnStateChanged;
        event Action<InputDeviceType> OnDeviceChanged;
        
        InputStateType CurrentStateType { get; }
        InputDeviceType CurrentDevice { get; }

        void SetState(InputStateType mode);
        void SetDevice(InputDeviceType device);
    }
}