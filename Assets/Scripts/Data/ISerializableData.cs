using SimpleJSON;

public interface ISerializableData
{
    public JSONObject SerializeData();
    public void DeserializeData(JSONObject _json);
}