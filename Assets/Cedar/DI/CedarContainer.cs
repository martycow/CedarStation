using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cedar.Core
{
    public sealed class CedarContainer : ICedarContainer
    {
        public Dictionary<Type, IDependency> RegisteredDependencies { get; } = new();
        
        private readonly string _containerName;
        private readonly ICedarLogger _logger;
        private readonly ICedarContainer? _parent;
        private readonly HashSet<Type> _resolving = new();
        private readonly List<IInitializable>  _initializers = new();
        private readonly List<IDisposable> _disposables = new();
        private readonly Dictionary<Type, (ConstructorInfo Ctor, ParameterInfo[] Params)> _constructorCache = new();
        private readonly Dictionary<Type, (MethodInfo Method, ParameterInfo[] Params)[]> _injectCache = new();

        public CedarContainer(string containerName, IEnumerable<IDependency> dependencies, ICedarLogger logger, ICedarContainer? parent = null)
        {
            _containerName = containerName;
            _logger = logger;
            _parent = parent;
            
            logger.Info(SystemTag.Container, $"[{_containerName}] Registering dependencies...");
            foreach (var dependency in dependencies)
            {
                if (RegisteredDependencies.TryAdd(dependency.ContractType, dependency))
                {
                    _logger.Info(SystemTag.Container, $"[{_containerName}] Registered {dependency.ContractType.Name} -> {dependency.ImplementationType.Name} ({dependency.Lifetime}).");
                    CacheConstructor(dependency);
                    continue;
                }

                _logger.Error(SystemTag.Container, $"[{_containerName}] Type {dependency.ContractType.Name} is already registered.");
            }
            
            logger.Info(SystemTag.Container, $"[{_containerName}] Registered {RegisteredDependencies.Count} dependencies.");
        }
        
        public void Initialize()
        {
            _logger.Info(SystemTag.Container, $"[{_containerName}] Initializing dependencies ({RegisteredDependencies.Count})...");

            foreach (var dependency in RegisteredDependencies.Values)
            {
                if (dependency.Lifetime == DependencyLifetime.Singleton && dependency.SingletonInstance == null)
                    Resolve(dependency.ContractType);
            }

            foreach (var t in _initializers)
            {
                try
                {
                    _logger.Info(SystemTag.Container, $"[{_containerName}] Initializing {t.GetType().Name}...");
                    t.Initialize();
                    _logger.Success(SystemTag.Container, $"[{_containerName}] {t.GetType().Name} initialized.");

                }
                catch (Exception e)
                {
                    _logger.Fail(SystemTag.Container, $"[{_containerName}] Error initializing {t.GetType().Name}: {e.Message}.");
                }
            }
            
            _logger.Success(SystemTag.Container, $"[{_containerName}] Container Initialized!");
        }

        public void Dispose()
        {
            _logger.Info(SystemTag.Container, $"[{_containerName}] Disposing...");
            for (var i = _disposables.Count - 1; i >= 0; i--)
            {
                var t = _disposables[i];
                try
                {
                    _logger.Info(SystemTag.Container, $"[{_containerName}] Disposing {t.GetType().Name}...");
                    _disposables[i].Dispose();
                    _logger.Success(SystemTag.Container, $"[{_containerName}] {t.GetType().Name} disposed.");
                }
                catch (Exception e)
                {
                    _logger.Fail(SystemTag.Container, $"[{_containerName}] Error disposing {_disposables[i].GetType().Name}: {e.Message}.");
                }
            }
            
            _disposables.Clear();
            RegisteredDependencies.Clear();
            _logger.Success(SystemTag.Container, $"[{_containerName}] Container Disposed!");
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                _logger.Error(SystemTag.Container, $"[{_containerName}] Error resolving {typeof(T).Name}: {e.Message}");
                throw;
            }
        }
        
        public void Inject(object target)
        {
            var targetType = target.GetType();

            if (!_injectCache.TryGetValue(targetType, out var methods))
            {
                //Looking for methods with [Inject] attribute
                methods = targetType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttribute<InjectAttribute>() != null)
                    .Select(m => (m, m.GetParameters()))
                    .ToArray();
                
                _injectCache[targetType] = methods;
            }

            try
            {
                foreach (var (methodInfo, paramInfos) in methods)
                {
                    var parameters = paramInfos
                        .Select(p => Resolve(p.ParameterType))
                        .ToArray();
                
                    methodInfo.Invoke(target, parameters);
                }
            }
            catch (Exception e)
            {
                _logger.Error(SystemTag.Container, $"[{_containerName}] Error injecting dependencies into {target.GetType().Name}: {e.Message}");
                throw;
            }
        }
        
        private void CacheConstructor(IDependency dependency)
        {
            if (dependency.SingletonInstance != null)
                return;

            var ctor = dependency.ImplementationType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            _constructorCache[dependency.ImplementationType] = (ctor, ctor.GetParameters());
        }
        
        public object Resolve(Type type)
        {
            if (!RegisteredDependencies.TryGetValue(type, out var entry))
            {
                if(_parent != null)
                    return _parent.Resolve(type);
                
                throw new InvalidOperationException($"[{_containerName}] Type {type} is not registered in the container.");
            }

            if (entry.Lifetime == DependencyLifetime.Singleton && entry.SingletonInstance != null)
                return entry.SingletonInstance;

            if (!_resolving.Add(type))
            {
                var cycle = string.Join(" -> ", _resolving.Select(t => t.Name));
                throw new InvalidOperationException($"[{_containerName}] Circular dependency detected: {cycle} -> {type.Name}");
            }

            try
            {
                var (ctorInfo, argsInfo) = _constructorCache[entry.ImplementationType];

                var args = argsInfo
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();

                var instance = ctorInfo.Invoke(args);
                
                if (instance is IDisposable disposable)
                    _disposables.Add(disposable);
                
                if (instance is IInitializable initializable)
                    _initializers.Add(initializable);

                if (entry.Lifetime == DependencyLifetime.Singleton)
                {
                    entry.SetSingletonInstance(instance);

                    foreach (var other in RegisteredDependencies.Values)
                    {
                        if (other != entry
                            && other.ImplementationType == entry.ImplementationType
                            && other.Lifetime == DependencyLifetime.Singleton
                            && other.SingletonInstance == null)
                        {
                            other.SetSingletonInstance(instance);
                        }
                    }
                }

                return instance;
            }
            finally
            {
                _resolving.Remove(type);
            }
        }
    }
}