using System;
using Game.Core;
using Game.General;

namespace Game.Input
{
    public abstract class BaseInputState : IInputState
    {
        public abstract InputStateType StateType { get; }
        protected readonly CedarLogger Logger;
        protected readonly InputActions InputActions;

        protected BaseInputState(InputActions inputActions, CedarLogger logger)
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