using System;

namespace Cedar.Core
{
    public interface IContainerScope : IDisposable
    {
        public ICedarContainer CedarContainer { get; }
        public void Initialize(ICedarLogger logger);
    }
}