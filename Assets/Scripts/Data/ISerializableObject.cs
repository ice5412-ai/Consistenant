namespace Habillage
{
    public interface ISerializableObject : ISerializableData
    {
        public string ID { get; set; }
    }
}