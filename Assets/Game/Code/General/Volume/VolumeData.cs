using System;
using Cedar.Core;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.General
{
    [Serializable]
    public sealed class VolumeData : BaseGameData<VolumeShape>
    {
        [JsonProperty("center")]
        public SerializableVector3 Center;
        
        [JsonProperty("size")]
        public SerializableVector3 Size;

        [JsonProperty("is_trigger")]
        public bool IsTrigger;
        
        protected override DataType ConcreteDataType => DataType.Volume;
        
        public VolumeData(
            Guid id, 
            string techName,
            VolumeShape subShape,
            ICedarLogger logger) : base(id, techName, subShape, logger)
        {
            Center = Vector3.zero;
            Size = Vector3.one;
            IsTrigger = false;
        }

        public Vector3 GetRandomPosition()
        {
            return new Vector3(
                Center.x + Random.Range(-Size.x / 2f, Size.x / 2f), 
                Center.y + Random.Range(-Size.y / 2f, Size.y / 2f), 
                Center.z + Random.Range(-Size.z / 2f, Size.z / 2f));
        }

        protected override string SerializeInternal()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void DeserializeInternal(BaseGameData<VolumeShape> inputData)
        {
            if (inputData is not VolumeData volumeData)
            {
                Logger.Error(SystemTag.Data, $"Failed to deserialize VolumeData. Input data is not of type VolumeData. Input: {inputData}");
                return;
            }
            
            Center = volumeData.Center;
            Size = volumeData.Size;
            IsTrigger = volumeData.IsTrigger;
        }
    }
}