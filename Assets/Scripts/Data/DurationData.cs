using SimpleJSON;

namespace Habillage
{
    public class DurationData : ISerializableData
    {
        public int Hours;
        public int Minute;
        public int Second;

        public override string ToString()
        {
            return $"{Hours:00}:{Minute:00}:{Second:00}";
        }

        public int GetSeconds()
        {
            int seconds = Hours * 3600 + Minute *60 + Second;
            return (int) seconds;
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("hours", Hours);
            _json.Add("minute", Minute);
            _json.Add("second", Second);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Hours = _json["hours"];
            Minute = _json["minute"];
            Second = _json["second"];
        }
    }
}