using System.Collections.Generic;

namespace Cedar.Core
{
    public interface ICedarContainerBuilder
    {
        IEnumerable<IDependency> Dependencies { get; }
        
        ICedarContainerBuilder Register<T>(DependencyLifetime lifetime = DependencyLifetime.Singleton);

        ICedarContainerBuilder Register<TContract, TImplementation>(DependencyLifetime lifetime = DependencyLifetime.Singleton) 
            where TImplementation : TContract;

        ICedarContainerBuilder RegisterInstance<TContract>(TContract instance);
        ICedarContainer Build();
    }
}