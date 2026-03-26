using System.Collections.Generic;

namespace Cedar.Core
{
    public class MockedContainerBuilder : ICedarContainerBuilder
    {
        public IEnumerable<IDependency> Dependencies { get; }
        public ICedarContainerBuilder Register<T>(DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return this;
        }

        public ICedarContainerBuilder Register<TContract, TImplementation>(DependencyLifetime lifetime = DependencyLifetime.Singleton) where TImplementation : TContract
        {
            return this;
        }

        public ICedarContainerBuilder RegisterInstance<TContract>(TContract instance)
        {
            return this;
        }

        public ICedarContainer Build()
        {
            return null;
        }
    }
}