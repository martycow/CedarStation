using System;

namespace Cedar.Core
{
    public interface IContainerScope : IDisposable
    {
        ICedarContainer Container { get; }
        ICedarContainer CreateAndInitContainer(ICedarLogger logger, ICedarContainer parent);
        ICedarContainerBuilder CreateBuilder(string containerName, ICedarLogger logger, ICedarContainer parent);
    }
}