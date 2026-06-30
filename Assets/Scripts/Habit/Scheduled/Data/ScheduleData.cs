using System;
using SimpleJSON;

namespace Habillage
{
    public abstract class ScheduleData : ISerializableData
    {
        public abstract ScheduleType Type { get; }

        public abstract override string ToString();
        public abstract bool ValidToday();
        public abstract bool ValidThisDay(DateTime _date);

        public virtual JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("type", (int)Type);

            return _json;
        }

        public virtual void DeserializeData(JSONObject _json)
        {
            
        }
    }
}