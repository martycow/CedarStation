using System;
using System.Collections.Generic;
using Cedar.Core;

namespace Game.General
{
    public sealed class EventBus : IInitializable, IDisposable
    {
        private readonly Dictionary<Type, Action<IGameEvent>> _handlers = new();
        private readonly Dictionary<Delegate, Action<IGameEvent>> _wrappers = new();
        private readonly ICedarLogger _logger;

        public EventBus(ICedarLogger logger)
        {
            _logger = logger;
        }
        
        public void Initialize()
        {
        }

        public void Dispose()
        {
            if (_handlers.Count > 0)
                _logger.Warn(SystemTag.EventBus, $"Disposing with {_handlers.Count} event handlers still registered.");
            
            _handlers.Clear();
            _wrappers.Clear();
        }

        public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            var key = typeof(TEvent);
            if (!_handlers.TryGetValue(key, out var handler))
            {
                _logger.Warn(SystemTag.EventBus, $"Event {key.Name} doesn't have subscribers. Event ignored.");
                return;
            }
            
            var handlerCopy = handler;
            _logger.Info(SystemTag.EventBus, $"Publishing event {key.Name}");
            handlerCopy.Invoke(gameEvent);
        }
        
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            if (_wrappers.ContainsKey(handler))
            {
                _logger.Warn(SystemTag.EventBus, $"Handler already subscribed to {typeof(TEvent).Name}. Subscription ignored.");
                return;
            }
            
            void MethodInstance(IGameEvent e) => handler((TEvent)e);
            
            var key = typeof(TEvent);
            _handlers.TryGetValue(key, out var existingHandler);
            
            _wrappers[handler] = MethodInstance;
            _handlers[key] = existingHandler + MethodInstance;
            _logger.Info(SystemTag.EventBus, $"Subscriber added for event {key.Name}");
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            if (!_wrappers.TryGetValue(handler, out var wrapper))
            {
                _logger.Warn(SystemTag.EventBus, $"Handler not found for {typeof(TEvent).Name}. Unsubscription ignored.");
                return;
            }
            
            var key = typeof(TEvent);
            if (_handlers.TryGetValue(key, out var existingHandler))
            {
                var updated = existingHandler - wrapper;
                if (updated == null)
                    _handlers.Remove(key);
                else
                    _handlers[key] = updated;
            }
            
            _wrappers.Remove(handler);
            _logger.Info(SystemTag.EventBus, $"Subscriber removed from event {key.Name}");
        }
    }
}
