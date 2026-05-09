namespace Game.General
{
    public class ConstParam<T> : Param<T>
    {
        public ConstParam(T initialValue) : base(initialValue) { }

        public ConstParam(T initialValue, T value) : base(initialValue, value) { }

        public override void SetValue(T newValue) { }
        public override void ResetValue() { }
    }
}