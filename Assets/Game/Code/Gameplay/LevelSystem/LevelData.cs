using System;
using System.Collections.Generic;
using Cedar.Core;
using Game.General;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Gameplay
{
    [Serializable]
    public sealed class LevelData : BaseGameData<LevelType>
    {
        [JsonProperty("display_name")]
        public string DisplayName;
        
        [JsonProperty("prefab_name")]
        public string PrefabName;
        
        [JsonProperty("player_spawn_zone")]
        public VolumeData[] PlayerSpawnZones;
        
        [JsonProperty("teleports")]
        public LevelTeleportData[] Teleports;
        
        [JsonProperty("other_spawn_zones")]
        public VolumeData[] OtherSpawnZones;
        
        protected override DataType ConcreteDataType => DataType.Level;
        
        // Generate
        public LevelData(
            Guid levelID, 
            string techName, 
            LevelType subType, 
            ICedarLogger logger) : base(levelID, techName, subType, logger)
        {
            DisplayName = "Unnamed Level";
            PrefabName = string.Empty;
            PlayerSpawnZones = Array.Empty<VolumeData>();
            OtherSpawnZones = Array.Empty<VolumeData>();
            Teleports = Array.Empty<LevelTeleportData>();
        }
        
        protected override string SerializeInternal()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void DeserializeInternal(BaseGameData<LevelType> deserializedData)
        {
            if (deserializedData is not LevelData levelData)
            {
                Logger.Error(SystemTag.Data, $"Failed to deserialize LevelData. Input data is not of type LevelData. Input: {deserializedData}");
                return;
            }
            
            DisplayName = levelData.DisplayName;
            PrefabName = levelData.PrefabName;
            PlayerSpawnZones = levelData.PlayerSpawnZones;
            OtherSpawnZones = levelData.OtherSpawnZones;
            Teleports = levelData.Teleports;
        }
    }
}