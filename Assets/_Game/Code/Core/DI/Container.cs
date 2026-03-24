using System;
using System.Collections.Generic;
using System.Linq;

namespace CedarStation.Core.DI
{
    public class Container
    {
        private readonly Dictionary<Type, DependencyInfo> registered = new();
        
        public Container(IEnumerable<DependencyInfo> entries)
        {
            foreach (var entry in entries)
                registered.Add(entry.ContractType, entry);
        }

        public T Resolve<T>()
        {
            try
            {
                return (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private object Resolve(Type type)
        {
            if (!registered.TryGetValue(type, out var entry))
            {
                throw new ArgumentException($"[Container] Type {type} is not registered in the container.");
            }

            if (entry.Lifetime == Lifetime.Singleton && entry.Instance != null)
                return entry.Instance;

            var constructor = entry.ImplementationType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var parameters = constructor
                .GetParameters()
                .Select(p => Resolve(p.ParameterType))
                .ToArray();

            var instance = constructor.Invoke(parameters);

            if (entry.Lifetime == Lifetime.Singleton)
                entry.SetInstance(instance);

            return instance;
        }
    }
}