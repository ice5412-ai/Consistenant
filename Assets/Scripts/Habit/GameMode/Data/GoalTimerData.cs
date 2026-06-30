using SimpleJSON;

namespace Habillage
{
    public class GoalTimerData : ModeData
    {
        public override GameModeType Type => GameModeType.GoalTimer;

        public DurationData IdealTime;

        public GoalTimerData(TimeData _notify, DurationData _duration) : base(_notify)
        {
            IdealTime = _duration;
        }

        public override string ToString()
        {
            return $"Ideal time {IdealTime.ToString()}";
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            
            _json.Add("ideal", IdealTime.SerializeData());

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
            
            IdealTime = new DurationData();
            IdealTime.DeserializeData(_json["ideal"].AsObject);
        }
    }
}