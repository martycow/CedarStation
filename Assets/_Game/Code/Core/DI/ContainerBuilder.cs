using System.Collections.Generic;
using CedarStation.Helpers;

namespace CedarStation.Core.DI
{
    public class ContainerBuilder
    {
        private readonly List<DependencyInfo> dependencies = new();

        public ContainerBuilder Register<T>(Lifetime lifetime = Lifetime.Singleton)
        {
            return Register<T, T>(lifetime);
        }
        
        public ContainerBuilder Register<TContract, TImplementation>(Lifetime lifetime = Lifetime.Singleton) where TImplementation : TContract
        {
            dependencies.Add(new DependencyInfo(typeof(TContract), typeof(TImplementation), lifetime));
            return this;
        }

        public Container Build(ILogger logger)
        {
            return new Container(dependencies, logger);
        }
    }
}