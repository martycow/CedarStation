using System;
using Game.General;
using Unity.Cinemachine;

namespace Game.Gameplay
{
    public sealed class CameraController : IInitializable, IDisposable
    {
        private readonly CinemachineBrain _cinemachineBrain;
        private readonly EventBus _eventBus;

        public CameraController(CinemachineBrain cinemachineBrain, EventBus eventBus)
        {
            _cinemachineBrain = cinemachineBrain;
            _eventBus = eventBus;
        }
        
        public void Initialize()
        {
            _eventBus.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
            _eventBus.Subscribe<PlayerKilledEvent>(OnPlayerKilled);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
        }
        
        private void OnPlayerSpawned(PlayerSpawnedEvent evt)
        {
        }
        
        private void OnPlayerKilled(PlayerKilledEvent obj)
        {
            
        }
    }
}