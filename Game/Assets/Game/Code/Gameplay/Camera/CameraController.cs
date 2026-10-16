using System;
using Game.General;
using Unity.Cinemachine;

namespace Game.Gameplay
{
    public sealed class CameraController : IInitializable, IDisposable
    {
        private readonly CinemachineBrain _cinemachineBrain;
        private readonly EventBus _eventBus;

        private CinemachineCamera _activeCamera;

        public CameraController(CinemachineBrain cinemachineBrain, EventBus eventBus)
        {
            _cinemachineBrain = cinemachineBrain;
            _eventBus = eventBus;
        }

        public void Initialize()
        {
            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Subscribe<PlayerKilledEvent>(OnPlayerKilled);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Unsubscribe<PlayerKilledEvent>(OnPlayerKilled);
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _activeCamera = evt.GameContext.Level.SceneRoot.CinemachineCamera;
            
            var playerTransform = evt.GameContext.Player.Movement.Transform;
            _activeCamera.Follow = playerTransform;
            _activeCamera.LookAt = playerTransform;
        }

        private void OnPlayerKilled(PlayerKilledEvent evt)
        {
            if (_activeCamera == null)
                return;

            _activeCamera.Follow = null;
            _activeCamera.LookAt = null;
        }
    }
}
