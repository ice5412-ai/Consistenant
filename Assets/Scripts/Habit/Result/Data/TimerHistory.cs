using System;
using SimpleJSON;

namespace Habillage
{
    public class TimerHistory : ISerializableData
    {
        public DurationData RemainingDuration = new();
        public DateTime RecordedTime = DateTime.Now;

        public TimerHistory(DurationData _remaining)
        {
            RemainingDuration = _remaining;
        }
        
        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("remain", RemainingDuration.SerializeData());
            _json.Add("time", RecordedTime);
            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            RemainingDuration.DeserializeData(_json["remain"].AsObject);
            RecordedTime = _json["time"];
        }
    }
}