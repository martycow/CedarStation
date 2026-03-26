using System;
using System.Collections.Generic;

namespace Cedar.Core
{
    public interface ICedarContainer : IInitializable, IDisposable
    {
        Dictionary<Type, IDependency> GetRegisteredDependencies();
        T Resolve<T>();
        void Inject(object target);
    }
}