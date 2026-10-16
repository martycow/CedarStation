using System;
using Game.Core;
using Game.General;
using Newtonsoft.Json;

namespace Game.Gameplay
{
    [Serializable]
    public sealed class LevelData : BaseGameData<LevelType>
    {
        [JsonProperty("display_name")]
        public string DisplayName;
        
        [JsonProperty("scene_name")]
        public string SceneName;
        
        [JsonProperty("player_spawn_zone")]
        public VolumeData[] PlayerSpawnZones;
        
        [JsonProperty("teleports")]
        public LevelTeleportData[] Teleports;
        
        [JsonProperty("other_spawn_zones")]
        public VolumeData[] OtherSpawnZones;
        
        protected override GameDataType ConcreteDataType => GameDataType.Level;
        
        // Generate
        public LevelData(
            Guid levelID, 
            string techName, 
            LevelType subType) : base(levelID, techName, subType)
        {
            DisplayName = "Unnamed Level";
            SceneName = string.Empty;
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
                return;
            
            DisplayName = levelData.DisplayName;
            SceneName = levelData.SceneName;
            PlayerSpawnZones = levelData.PlayerSpawnZones;
            OtherSpawnZones = levelData.OtherSpawnZones;
            Teleports = levelData.Teleports;
        }
    }
}