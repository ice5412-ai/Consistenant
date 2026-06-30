using System;
using SimpleJSON;

namespace Habillage
{
    public class AlarmData : ModeData
    {
        public override GameModeType Type => GameModeType.Alarm;

        public TimeData AlarmTime;

        public AlarmData(TimeData _notify, TimeData _time) : base(_notify)
        {
            AlarmTime = _time;
        }

        public override string ToString()
        {
            return AlarmTime.ToString();
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();

            _json.Add("time", AlarmTime.SerializeData());

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
            AlarmTime.DeserializeData(_json["time"].AsObject);
        }
    }
}