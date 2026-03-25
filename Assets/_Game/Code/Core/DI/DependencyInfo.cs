using System;
using CedarStation.Helpers;

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
        
        public void SetInstance(object instance, ILogger logger)
        {
            if (Lifetime != Lifetime.Singleton)
            {
                logger.Error("Lifetime must be singleton", LogType.Container);
                return;
            }

            if (Instance != null)
            {
                logger.Error("Instance already set", LogType.Container);
                return;
            }
            
            Instance = instance;
        }
    }
}