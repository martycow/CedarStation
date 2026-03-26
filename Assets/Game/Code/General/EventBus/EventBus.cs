using System;
using Cedar.Core;

namespace Game.General
{
    public sealed class EventBus : IInitializable, IDisposable
    {
        private readonly ICedarLogger _logger;

        public EventBus(ICedarLogger logger)
        {
            _logger = logger;
        }
        
        public void Initialize()
        {
            _logger.Success(SystemTag.EventBus, "Initialized.");
        }

        public void Dispose()
        {
            _logger.Success(SystemTag.EventBus, "Disposed.");
        }

        public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            
        }
        
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            
        }
    }
}
