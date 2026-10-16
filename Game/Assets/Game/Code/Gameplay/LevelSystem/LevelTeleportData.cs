using System;
using Game.Core;
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
        
        protected override GameDataType ConcreteDataType => GameDataType.LevelTeleport;
        
        // Generate new
        public LevelTeleportData(
            Guid id,
            string techName,
            TeleportType subType) : base(id, techName, subType)
        {
            DestinationID = Guid.Empty;
            TeleportZone = new VolumeData(
                Guid.NewGuid(),
                techName,
                VolumeShape.Box);
        }

        protected override string SerializeInternal()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void DeserializeInternal(BaseGameData<TeleportType> deserializedData)
        {
            if (deserializedData is not LevelTeleportData teleportData)
                return;
            
            DestinationID = teleportData.DestinationID;
            TeleportZone = teleportData.TeleportZone;
        }
    }
}