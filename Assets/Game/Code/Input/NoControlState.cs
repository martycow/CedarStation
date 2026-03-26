using Cedar.Core;

namespace Game.Input
{
    public sealed class NoControlState : BaseInputState
    {
        public override InputStateType StateType => InputStateType.NoControl;
        
        public NoControlState(InputActions inputActions, ICedarLogger logger) : base(inputActions, logger) { }
        
        public override void Initialize()
        {
            Disable();
            
            Logger.Success(SystemTag.Input, $"Initialized {StateType} Input State.");
        }

        public override void Dispose() { }

        public override void Enable()
        {
            InputActions.Moving.Disable();
            InputActions.View.Disable();
            InputActions.Actions.Disable();
            InputActions.Menu.Disable();
        }

        public override void Disable() { }
    }
}