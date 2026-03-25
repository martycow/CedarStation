using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CedarStation.Helpers;

namespace CedarStation.Core.DI
{
    public sealed class Container : IDisposable
    {
        private readonly Dictionary<Type, DependencyInfo> registered = new();
        private readonly HashSet<Type> resolving = new();
        private readonly List<IDisposable> disposables = new();
        private readonly ILogger logger;
        
        public Container(IEnumerable<DependencyInfo> entries, ILogger logger)
        {
            foreach (var entry in entries)
                registered.Add(entry.ContractType, entry);

            this.logger = logger;
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                logger.Error($"Error resolving {typeof(T).Name}: {e.Message}", LogType.Container);
                throw;
            }
        }

        public void Inject(object target)
        {
            var methods = target.GetType()
                .GetMethods((BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttribute<Inject>() != null);

            try
            {
                foreach (var methodInfo in methods)
                {
                    var parameters = methodInfo.GetParameters()
                        .Select(p => Resolve(p.ParameterType))
                        .ToArray();
                
                    methodInfo.Invoke(target, parameters);
                }
            }
            catch (Exception e)
            {
                logger.Error($"Error injecting dependencies into {target.GetType().Name}: {e.Message}", LogType.Container);
                throw;
            }
        }

        private object Resolve(Type type)
        {
            if (!registered.TryGetValue(type, out var entry))
            {
                logger.Error($"Type {type} is not registered in the container.", LogType.Container);
                return null;
            }

            if (entry.Lifetime == Lifetime.Singleton && entry.Instance != null)
                return entry.Instance;

            if (!resolving.Add(type))
            {
                var cycle = string.Join(" -> ", resolving.Select(t => t.Name));
                logger.Error($"Circular dependency detected: {cycle} -> {type.Name}", LogType.Container);
            }

            try
            {
                var constructor = entry.ImplementationType
                    .GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .First();

                var parameters = constructor
                    .GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();

                var instance = constructor.Invoke(parameters);

                if (instance is IDisposable disposable)
                    disposables.Add(disposable);

                if (entry.Lifetime == Lifetime.Singleton)
                    entry.SetInstance(instance, logger);

                return instance;
            }
            finally
            {
                resolving.Remove(type);
            }
        }

        public void Dispose()
        {
            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    disposables[i].Dispose();
                }
                catch (Exception e)
                {
                    logger.Error($"Error disposing {disposables[i].GetType().Name}: {e.Message}", LogType.Container);
                }
            }
            
            disposables.Clear();
            registered.Clear();
        }
    }
}