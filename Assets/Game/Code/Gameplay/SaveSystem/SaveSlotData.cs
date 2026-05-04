using System;
using Cedar.Core;
using Game.General;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class SaveSlotData : BaseGameData<GameDifficulty>
    {
        [JsonProperty("level_id")]
        public SerializableGuid LevelID;
        
        [JsonProperty("spawn_position")]
        public SerializableVector3 SpawnPosition;
        
        [JsonProperty("spawn_rotation")]
        public SerializableQuaternion SpawnRotation;

        [JsonProperty("is_fresh")]
        public bool IsFresh;
        
        protected override DataType ConcreteDataType => DataType.SaveSlot;
        
        [JsonConstructor]
        private SaveSlotData() { }

        public SaveSlotData(
            Guid id,
            GameDifficulty difficulty,
            Guid levelID,
            Vector3 spawnPosition,
            Quaternion spawnRotation,
            ICedarLogger logger) : base(id, GenerateSaveSlotName(id, difficulty), difficulty, logger)
        {
            LevelID = levelID;
            SpawnPosition = spawnPosition;
            SpawnRotation = spawnRotation;
            IsFresh = true;
        }

        private static string GenerateSaveSlotName(Guid id, GameDifficulty difficulty)
        {
            return $"cedar_station_save_slot_{difficulty}_{id}";
        }

        protected override string SerializeInternal()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void DeserializeInternal(BaseGameData<GameDifficulty> deserializedData)
        {
            if (deserializedData is not SaveSlotData saveSlot)
            {
                Logger.Error(SystemTag.Save, $"Failed to deserialize save slot with ID {ID}.");
                return;
            }
            
            LevelID = saveSlot.LevelID;
            SpawnPosition = saveSlot.SpawnPosition;
            SpawnRotation = saveSlot.SpawnRotation;
            IsFresh = saveSlot.IsFresh;
        }
    }
}