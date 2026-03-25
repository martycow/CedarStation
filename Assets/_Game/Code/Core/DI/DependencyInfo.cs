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
        
        private readonly ICedarLogger _logger;

        public DependencyInfo(Type contract, Type implementation, Lifetime lifetime, ICedarLogger logger)
        {
            ContractType = contract;
            ImplementationType = implementation;
            Lifetime = lifetime;
            _logger = logger;
        }
        
        public void SetInstance(object instance)
        {
            if (Lifetime != Lifetime.Singleton)
            {
                _logger.Error(LogTag.Container, "Lifetime must be singleton.");
                return;
            }

            if (Instance == null)
            {
                Instance = instance;
                return;
            }
            
            _logger.Error(LogTag.Container,"Instance already set.");
        }
    }
}