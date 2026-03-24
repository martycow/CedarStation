using System;

namespace CedarStation.Core.DI
{
    public class DependencyInfo
    {
        public Type ContractType { get; }
        public Type ImplementationType { get; }
        public Lifetime Lifetime { get; }
        
        public object Instance { get; private set; }
        
        public DependencyInfo(Type contract, Type implementation, Lifetime lifetime)
        {
            ContractType = contract;
            ImplementationType = implementation;
            Lifetime = lifetime;
        }
        
        public void SetInstance(object instance)
        {
            if (Lifetime != Lifetime.Singleton)
                throw new InvalidOperationException("Lifetime must be singleton");

            if (Instance != null)
                throw new InvalidOperationException("Instance is already set");
            
            Instance = instance;
        }
    }
}