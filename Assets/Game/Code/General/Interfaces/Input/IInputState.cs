using System;
using Cedar.Core;

namespace Game.General
{
    public interface IInputState : IInitializable, IDisposable
    {
        event Action<InputDeviceType> OnDeviceChanged;
        InputStateType StateType { get; }
        void Enable();
        void Disable();
    }
}