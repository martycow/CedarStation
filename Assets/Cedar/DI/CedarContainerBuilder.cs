using System.Collections.Generic;

namespace Cedar.Core
{
    public sealed class CedarContainerBuilder : ICedarContainerBuilder
    {
        public IEnumerable<IDependency> Dependencies => _dependencies;
        
        private readonly List<IDependency> _dependencies = new();
        private readonly ICedarLogger _logger;

        public CedarContainerBuilder(ICedarLogger cedarLogger)
        {
            _logger = cedarLogger;
        }

        public ICedarContainerBuilder Register<T>(DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return Register<T, T>(lifetime);
        }
        
        public ICedarContainerBuilder Register<TContract, TImplementation>(DependencyLifetime lifetime = DependencyLifetime.Singleton) where TImplementation : TContract
        {
            var info = new Dependency(typeof(TContract), typeof(TImplementation), lifetime, _logger);
            
            _dependencies.Add(info);
            return this;
        }
        
        public ICedarContainerBuilder RegisterInstance<TContract>(TContract instance)
        {
            var info = new Dependency(typeof(TContract), instance.GetType(), DependencyLifetime.Singleton, _logger);
            info.SetSingletonInstance(instance);
            
            _dependencies.Add(info);
            return this;
        }

        public ICedarContainer Build()
        {
            return new CedarContainer(_dependencies, _logger);
        }
    }
}