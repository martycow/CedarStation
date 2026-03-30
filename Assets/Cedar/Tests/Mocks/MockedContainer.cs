using System;
using System.Collections.Generic;

namespace Cedar.Core
{
    public class MockedContainer : ICedarContainer
    {
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }

        Dictionary<Type, IDependency> ICedarContainer.RegisteredDependencies { get; }

        public T Resolve<T>()
        {
            return default;
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public void Inject(object target)
        {
            
        }
    }
}