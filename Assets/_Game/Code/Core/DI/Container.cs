using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CedarStation.Helpers;

namespace CedarStation.Core.DI
{
    public sealed class Container : IDisposable
    {
        private readonly Dictionary<Type, DependencyInfo> _registered = new();
        private readonly HashSet<Type> _resolving = new();
        
        private readonly List<IInitializable>  _initializers = new();
        private readonly List<IDisposable> _disposables = new();
        
        private readonly ICedarLogger _logger;
        
        private readonly Dictionary<Type, (ConstructorInfo Ctor, ParameterInfo[] Params)> _constructorCache = new();
        private readonly Dictionary<Type, (MethodInfo Method, ParameterInfo[] Params)[]> _injectCache = new();
        
        public Container(IEnumerable<DependencyInfo> entries, ICedarLogger logger)
        {
            foreach (var entry in entries)
            {
                _registered[entry.ContractType] = entry;
                
                if (entry.Instance != null)
                    continue;

                var ctor = entry.ImplementationType
                    .GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .First();

                _constructorCache[entry.ImplementationType] = (ctor, ctor.GetParameters());
            }

            _logger = logger;
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                _logger.Error(LogTag.Container, $"Error resolving {typeof(T).Name}: {e.Message}");
                throw;
            }
        }

        public void Initialize()
        {
            for (var i = 0; i < _initializers.Count; i++)
            {
                try
                {
                    _initializers[i].Initialize();
                }
                catch (Exception e)
                {
                    _logger.Error(LogTag.Container, $"Error initializing {_initializers[i].GetType().Name}: {e.Message}.");
                }
            }
        }

        public void Dispose()
        {
            for (var i = _disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    _disposables[i].Dispose();
                }
                catch (Exception e)
                {
                    _logger.Error(LogTag.Container, $"Error disposing {_disposables[i].GetType().Name}: {e.Message}.");
                }
            }
            
            _disposables.Clear();
            _registered.Clear();
        }
        
        public void Inject(object target)
        {
            var targetType = target.GetType();

            if (_injectCache.TryGetValue(targetType, out var methods) == false)
            {
                methods = targetType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttribute<Inject>() != null)
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
                _logger.Error(LogTag.Container, $"Error injecting dependencies into {target.GetType().Name}: {e.Message}");
                throw;
            }
        }
        
        private object Resolve(Type type)
        {
            if (_registered.TryGetValue(type, out var entry) == false)
                throw new InvalidOperationException($"Type {type} is not registered in the container.");

            if (entry.Lifetime == Lifetime.Singleton && entry.Instance != null)
                return entry.Instance;

            if (_resolving.Add(type) == false)
            {
                var cycle = string.Join(" -> ", _resolving.Select(t => t.Name));
                throw new InvalidOperationException($"Circular dependency detected: {cycle} -> {type.Name}");
            }

            try
            {
                var (constructor, paramInfos) = _constructorCache[entry.ImplementationType];

                var parameters = paramInfos
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();

                var instance = constructor.Invoke(parameters);

                switch (instance)
                {
                    case IDisposable disposable:
                        _disposables.Add(disposable);
                        break;
                    case IInitializable initializable:
                        _initializers.Add(initializable);
                        break;
                }

                if (entry.Lifetime == Lifetime.Singleton)
                    entry.SetInstance(instance);

                return instance;
            }
            finally
            {
                _resolving.Remove(type);
            }
        }
    }
}