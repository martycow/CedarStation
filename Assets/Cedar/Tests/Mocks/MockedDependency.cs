using System;

namespace Cedar.Core
{
    public class MockedDependency : IDependency
    {
        public Type ContractType { get; }
        public Type ImplementationType { get; }
        public DependencyLifetime Lifetime { get; }
        public object SingletonInstance { get; }
        public void SetSingletonInstance(object instance)
        {
            
        }
    }
}