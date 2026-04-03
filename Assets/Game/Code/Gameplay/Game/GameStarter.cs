using System;
using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    internal class GameStarter : IDisposable
    {
        private readonly PlayerController _playerController;
        private readonly IInputManager _inputManager;
        private readonly ICedarLogger _logger;

        public GameStarter(PlayerController playerController, IInputManager inputManager, ICedarLogger logger)
        {
            _playerController = playerController;
            _inputManager = inputManager;
            _logger = logger;
        }

        public void Dispose()
        {
            _playerController.DestroyPlayer();
        }

        public void Start()
        {
            var spawnData = new SpawnData
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity
            };
            
            _inputManager.SetState(InputStateType.Gameplay);
            _playerController.CreatePlayer(spawnData);
            
            _logger.Line();
            _logger.Success(SystemTag.Gameplay, "Game started.");
            _logger.Line();
        }
    }
}