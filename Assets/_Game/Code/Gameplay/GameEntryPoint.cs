using CedarStation.Core.DI;
using UnityEngine;

namespace CedarStation.Gameplay
{
    public class GameEntryPoint : MonoBehaviour, IContainerHandler
    {
        public Container Container { get; private set; }
        
        public void Initialize()
        {
            var builder = new ContainerBuilder();
            
            Container = builder.Build();
        }
        
        public void Dispose()
        {
            
        }
    }
}