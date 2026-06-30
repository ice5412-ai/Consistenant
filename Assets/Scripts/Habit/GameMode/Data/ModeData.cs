using System;
using SimpleJSON;

namespace Habillage
{
    public abstract class ModeData : ISerializableData
    {
        public abstract GameModeType Type { get; }

        public TimeData Notify = new();

        public abstract override string ToString();

        public ModeData(TimeData _notify)
        {
            Notify = _notify;
        }
        
        public virtual JSONObject SerializeData()
        {
            var _json = new JSONObject();
            
            _json.Add("type", (int)Type);
            _json.Add("notify", Notify.SerializeData());

            return _json;
        }

        public virtual void DeserializeData(JSONObject _json)
        {
            Notify.DeserializeData(_json["notify"].AsObject);
        }
    }
}