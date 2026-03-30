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