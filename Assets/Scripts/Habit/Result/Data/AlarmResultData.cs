using System;
using SimpleJSON;

namespace Habillage
{
    public class AlarmResultData : ResultData
    {
        public override GameModeType Type => GameModeType.Alarm;
        public override HabitCompletion CompleteStat()
        {
            if (MarkedDateTime < TimesUpDateTime)
                return HabitCompletion.Succeed;
            else
                return HabitCompletion.Failed;
        }

        public DateTime StartDateTime;
        public DateTime MarkedDateTime;
        public DateTime TimesUpDateTime;
        
        public int AlarmRemaining => (int)(TimeSpan.FromTicks(TimesUpDateTime.Ticks).TotalSeconds - TimeSpan.FromTicks(MarkedDateTime.Ticks).TotalSeconds);

        public AlarmResultData()
        {
            
        }

        public override float GetValue()
        {
            return AlarmRemaining;
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            _json.Add("start", StartDateTime);
            _json.Add("stop",MarkedDateTime);
            _json.Add("timesup", TimesUpDateTime);
            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
            StartDateTime = _json["start"];
            MarkedDateTime = _json["stop"];
            TimesUpDateTime = _json["timesup"];
        }
    }
}