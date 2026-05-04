using System;
using Cedar.Core;
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
        public DataType DataType => ConcreteDataType;
        
        [JsonProperty("sub_type")] 
        public TSubData SubType;

        [JsonIgnore]
        public ICedarLogger Logger { get; private set; }

        protected BaseGameData() { }

        protected BaseGameData(Guid id, string techName, TSubData subType, ICedarLogger logger)
        {
            ID = id;
            TechName = techName;
            SubType = subType;
            Logger = logger;
        }
        
        protected abstract DataType ConcreteDataType { get; }

        public string Serialize()
        {
            var result = SerializeInternal();
            Logger.Success(SystemTag.Data, $"Data serialization completed for {GetType().Name}. Result: {result}");
            return result;
        }

        public void Deserialize(string inputData)
        {
            var deserializedData = JsonConvert.DeserializeObject<BaseGameData<TSubData>>(inputData);
            if (deserializedData == null)
            {
                Logger.Error(SystemTag.Data, $"Failed to deserialize data {GetType().Name} with input: {inputData}");
                return;
            }
            
            ID = deserializedData.ID;
            TechName = deserializedData.TechName;
            SubType = deserializedData.SubType;
            
            DeserializeInternal(deserializedData);
        }

        protected abstract string SerializeInternal();
        protected abstract void DeserializeInternal(BaseGameData<TSubData> deserializedData);
    }
}