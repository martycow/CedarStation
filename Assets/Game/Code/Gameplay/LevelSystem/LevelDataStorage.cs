using System;
using System.Collections.Generic;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    [CreateAssetMenu(menuName = "Cedar Station/Storage/Create Level Data Storage", fileName = "Storage_Level")]
    public sealed class LevelDataStorage : ScriptableObject, IInitializable
    {
        [SerializeField]
        private LevelData[] levelDataArray;
        
        public readonly Dictionary<Guid, LevelData> LevelDataById = new();

        private CedarLogger _logger;
        
        [Inject]
        public void Inject(CedarLogger logger)
        {
            _logger = logger;
        }
        
        public void Initialize()
        {
            LevelDataById.Clear();

            if (levelDataArray == null || levelDataArray.Length == 0)
            {
                _logger.Error(LogTag.Level, "Level storage is empty.");
                return;
            }
            
            foreach (var levelData in levelDataArray)
                LevelDataById[levelData.ID] = levelData;
        }
        
        public LevelData GetFirstLevel()
        {
            if (levelDataArray.Length > 0)
                return levelDataArray[0];

            _logger.Error(LogTag.Level,"Level storage is empty.");
            return null;
        }
    }
}