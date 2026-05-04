using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    [CreateAssetMenu(menuName = "Cedar Station/Storage/Create Level Data Storage", fileName = "Storage_Level")]
    public sealed class LevelDataStorage : ScriptableObject
    {
        [SerializeField]
        private LevelData[] levelDataArray;

        public LevelData DefaultLevel => levelDataArray.Length > 0 ? levelDataArray[0] : null;
        public readonly Dictionary<Guid, LevelData> LevelDataById = new();
        public readonly Dictionary<string, LevelData> LevelDataByTechName = new();

        public void Init()
        {
            LevelDataById.Clear();
            LevelDataByTechName.Clear();
            
            for (var i = 0; i < levelDataArray.Length; i++)
            {
                var levelData = levelDataArray[i];
                LevelDataById[levelData.ID] = levelData;
                LevelDataByTechName[levelData.TechName] = levelData;
            }
        }
    }
}