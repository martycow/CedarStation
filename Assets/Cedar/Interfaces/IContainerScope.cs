using System;

namespace Cedar.Core
{
    public interface IContainerScope : IDisposable
    {
        public ICedarContainer RootContainer { get; }
    }
}