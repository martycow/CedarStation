using System;

namespace CedarStation.Core.DI
{
    public interface IContainerHandler : IDisposable
    {
        public Container Container { get; }
        public void Initialize();
    }
}