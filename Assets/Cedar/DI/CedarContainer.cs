using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cedar.Core
{
    public sealed class CedarContainer : ICedarContainer
    {
        private readonly Dictionary<Type, IDependency> _dependencyMap = new();
        private readonly HashSet<Type> _resolving = new();
        
        private readonly List<IInitializable>  _initializers = new();
        private readonly List<IDisposable> _disposables = new();
        
        private readonly ICedarLogger _logger;
        
        private readonly Dictionary<Type, (ConstructorInfo Ctor, ParameterInfo[] Params)> _constructorCache = new();
        private readonly Dictionary<Type, (MethodInfo Method, ParameterInfo[] Params)[]> _injectCache = new();
        
        public CedarContainer(IEnumerable<IDependency> dependencies, ICedarLogger logger)
        {
            _logger = logger;
            
            logger.Info(SystemTag.Container, "Registering dependencies...");
            foreach (var dependency in dependencies)
            {
                if (_dependencyMap.TryAdd(dependency.ContractType, dependency))
                {
                    _logger.Info(SystemTag.Container, $"Registered {dependency.ContractType.Name} -> {dependency.ImplementationType.Name} ({dependency.Lifetime}).");
                    CacheConstructor(dependency);
                    continue;
                }

                _logger.Error(SystemTag.Container, $"Type {dependency.ContractType.Name} is already registered.");
            }
            
            logger.Info(SystemTag.Container, $"Registered {_dependencyMap.Count} dependencies.");
        }
        
        public void Initialize()
        {
            _logger.Info(SystemTag.Container, $"Initializing ({_dependencyMap.Count})...");

            foreach (var dependency in _dependencyMap.Values)
            {
                if (dependency.Lifetime == DependencyLifetime.Singleton && dependency.SingletonInstance == null)
                    Resolve(dependency.ContractType);
            }

            foreach (var t in _initializers)
            {
                try
                {
                    _logger.Info(SystemTag.Container, $"Initializing {t.GetType().Name}...");
                    t.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Fail(SystemTag.Container, $"Error initializing {t.GetType().Name}: {e.Message}.");
                }
            }
            _logger.Success(SystemTag.Container, "Initialized.");
        }

        public void Dispose()
        {
            _logger.Info(SystemTag.Container, "Disposing...");
            for (var i = _disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    _disposables[i].Dispose();
                }
                catch (Exception e)
                {
                    _logger.Fail(SystemTag.Container, $"Error disposing {_disposables[i].GetType().Name}: {e.Message}.");
                }
            }
            
            _disposables.Clear();
            _dependencyMap.Clear();
            _logger.Success(SystemTag.Container, "Disposed.");
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                _logger.Error(SystemTag.Container, $"Error resolving {typeof(T).Name}: {e.Message}");
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
                _logger.Error(SystemTag.Container, $"Error injecting dependencies into {target.GetType().Name}: {e.Message}");
                throw;
            }
        }
        
        public Dictionary<Type, IDependency> GetRegisteredDependencies()
        {
            return _dependencyMap;
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
        
        private object Resolve(Type type)
        {
            if (!_dependencyMap.TryGetValue(type, out var entry))
                throw new InvalidOperationException($"Type {type} is not registered in the container.");

            if (entry.Lifetime == DependencyLifetime.Singleton && entry.SingletonInstance != null)
                return entry.SingletonInstance;

            if (!_resolving.Add(type))
            {
                var cycle = string.Join(" -> ", _resolving.Select(t => t.Name));
                throw new InvalidOperationException($"Circular dependency detected: {cycle} -> {type.Name}");
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

                    foreach (var other in _dependencyMap.Values)
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