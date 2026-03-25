using System;
using CedarStation.Helpers;

namespace CedarStation.Core.DI
{
    public interface IContainerScope : IDisposable
    {
        public Container Container { get; }
        public void Initialize(ICedarLogger logger);
    }
}