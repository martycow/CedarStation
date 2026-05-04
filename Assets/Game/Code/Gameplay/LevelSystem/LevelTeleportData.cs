using System;
using Cedar.Core;
using Game.General;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Gameplay
{
    [Serializable]
    public sealed class LevelTeleportData : BaseGameData<TeleportType>
    {
        [JsonProperty("destination_id")]
        public SerializableGuid DestinationID;
        
        [JsonProperty("teleport_zone")]
        public VolumeData TeleportZone;
        
        protected override DataType ConcreteDataType => DataType.LevelTeleport;
        
        // Generate new
        public LevelTeleportData(
            Guid id,
            string techName,
            TeleportType subType,
            ICedarLogger logger) : base(id, techName, subType, logger)
        {
            DestinationID = Guid.Empty;
            TeleportZone = new VolumeData(
                Guid.NewGuid(),
                techName,
                VolumeShape.Box,
                logger);
        }

        protected override string SerializeInternal()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void DeserializeInternal(BaseGameData<TeleportType> deserializedData)
        {
            if (deserializedData is not LevelTeleportData teleportData)
            {
                Logger.Error(SystemTag.Data, $"Failed to deserialize LevelTeleportData. Input data is not of type LevelTeleportData. Input: {deserializedData}");
                return;
            }
            
            DestinationID = teleportData.DestinationID;
            TeleportZone = teleportData.TeleportZone;
        }
    }
}