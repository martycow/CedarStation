using System;
using System.Collections.Generic;

namespace Cedar.Core
{
    public interface ICedarContainer : IInitializable, IDisposable
    {
        Dictionary<Type, IDependency> RegisteredDependencies { get; }
        T Resolve<T>();
        object Resolve(Type type);
        void Inject(object target);
    }
}