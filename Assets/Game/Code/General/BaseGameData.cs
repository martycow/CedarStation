using System;
using Newtonsoft.Json;

namespace Game.General
{
    /// <summary>
    /// Base class for all game data types
    /// Every data in the game has its own unique ID, tech name, and type
    /// </summary>
    [Serializable]
    public abstract class BaseGameData<TSubData> : ISerializableData where TSubData : Enum
    {
        [JsonProperty("id")]
        public SerializableGuid ID;

        [JsonProperty("tech_name")] 
        public string TechName;

        [JsonProperty("data_type")]
        public GameDataType DataType => ConcreteDataType;
        
        [JsonProperty("sub_type")] 
        public TSubData SubType;

        protected BaseGameData() { }

        protected BaseGameData(Guid id, string techName, TSubData subType)
        {
            ID = id;
            TechName = techName;
            SubType = subType;
        }
        
        protected abstract GameDataType ConcreteDataType { get; }

        public string Serialize()
        {
            var result = SerializeInternal();
            return result;
        }

        public void Deserialize(string inputData)
        {
            var deserializedData = JsonConvert.DeserializeObject<BaseGameData<TSubData>>(inputData);
            if (deserializedData == null)
                return;
            
            ID = deserializedData.ID;
            TechName = deserializedData.TechName;
            SubType = deserializedData.SubType;
            
            DeserializeInternal(deserializedData);
        }

        protected abstract string SerializeInternal();
        protected abstract void DeserializeInternal(BaseGameData<TSubData> deserializedData);
    }
}