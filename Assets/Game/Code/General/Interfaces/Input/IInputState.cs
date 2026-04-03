using System;
using Cedar.Core;

namespace Game.General
{
    public interface IInputState : IInitializable, IDisposable
    {
        InputStateType StateType { get; }
        void Enable();
        void Disable();
    }
}