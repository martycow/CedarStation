using System;
using Game.General;

namespace Game.Core
{
    public sealed class Dependency : IDependency
    {
        public Type ContractType { get; }
        public Type ImplementationType { get; }
        public DependencyLifetime Lifetime { get; }
        public object SingletonInstance { get; private set; }

        private readonly CedarLogger _logger;

        public Dependency(Type contract, Type implementation, DependencyLifetime lifetime, CedarLogger logger)
        {
            ContractType = contract;
            ImplementationType = implementation;
            Lifetime = lifetime;
            _logger = logger;
        }
        
        public void SetSingletonInstance(object instance)
        {
            if (Lifetime != DependencyLifetime.Singleton)
            {
                _logger.Error(LogTag.Container, "Lifetime must be singleton when setting instance.");
                return;
            }

            if (SingletonInstance == null)
            {
                SingletonInstance = instance;
                return;
            }
            
            _logger.Error(LogTag.Container,"Instance already set.");
        }
    }
}