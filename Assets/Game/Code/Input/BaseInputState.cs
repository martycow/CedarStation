using System;
using Cedar.Core;
using Game.General;

namespace Game.Input
{
    public abstract class BaseInputState : IInputState
    {
        public abstract InputStateType StateType { get; }
        protected readonly ICedarLogger Logger;
        protected readonly InputActions InputActions;

        protected BaseInputState(InputActions inputActions, ICedarLogger logger)
        {
            Logger = logger;
            InputActions = inputActions;
        }

        public abstract void Initialize();
        public abstract void Dispose();
        public abstract void Enable();
        public abstract void Disable();
    }
}