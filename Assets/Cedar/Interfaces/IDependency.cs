using System;

namespace Cedar.Core
{
    public interface IDependency
    {
        Type ContractType { get; }
        Type ImplementationType { get; }
        DependencyLifetime Lifetime { get; }
        
        object SingletonInstance { get; }
        void SetSingletonInstance(object instance);
    }
}