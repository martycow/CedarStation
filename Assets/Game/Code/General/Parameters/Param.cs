using System;

namespace Game.General
{
    public class Param<T>
    {
        public event Action<T> OnValueChanged;
        
        public T Value { get; private set; }
        public T InitialValue { get; protected set; }
        
        public Param(T initialValue = default)
        {
            InitialValue = initialValue;
            Value = initialValue;
        }

        public Param(T initialValue, T value)
        {
            InitialValue = initialValue;
            Value = value;
        }
        
        public virtual void SetValue(T newValue)
        {
            if (Equals(Value, newValue))
                return;

            Value = newValue;
            OnValueChanged?.Invoke(newValue);
        }
        
        public virtual void ResetValue()
        {
            SetValue(InitialValue);
        }
    }
}