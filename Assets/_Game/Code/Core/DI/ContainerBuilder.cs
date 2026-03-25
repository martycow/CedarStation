using System.Collections.Generic;
using CedarStation.Helpers;

namespace CedarStation.Core.DI
{
    public sealed class ContainerBuilder
    {
        private readonly List<DependencyInfo> _dependencies = new();
        private readonly ICedarLogger _logger;

        public ContainerBuilder(ICedarLogger cedarLogger)
        {
            _logger = cedarLogger;
        }
        
        public ContainerBuilder Register<T>(Lifetime lifetime = Lifetime.Singleton)
        {
            return Register<T, T>(lifetime);
        }
        
        public ContainerBuilder Register<TContract, TImplementation>(Lifetime lifetime = Lifetime.Singleton) where TImplementation : TContract
        {
            var info = new DependencyInfo(typeof(TContract), typeof(TImplementation), lifetime, _logger);
            
            _dependencies.Add(info);
            return this;
        }
        
        public ContainerBuilder RegisterInstance<TContract>(TContract instance)
        {
            var info = new DependencyInfo(typeof(TContract), instance.GetType(), Lifetime.Singleton, _logger);
            info.SetInstance(instance);
            
            _dependencies.Add(info);
            return this;
        }

        public Container Build()
        {
            return new Container(_dependencies, _logger);
        }
    }
}