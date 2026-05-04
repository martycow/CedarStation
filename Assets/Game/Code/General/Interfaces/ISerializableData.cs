namespace Game.General
{
    public interface ISerializableData
    {
        string Serialize();
        void Deserialize(string inputData);
    }
}