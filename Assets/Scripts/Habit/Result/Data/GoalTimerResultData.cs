using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

namespace Habillage
{
    public class GoalTimerResultData : ResultData
    {
        public override GameModeType Type => GameModeType.GoalTimer;
        public override HabitCompletion CompleteStat()
        {
            //return RemainingDuration.GetSeconds() <= 0 ? HabitCompletion.Succeed : HabitCompletion.Failed;
            return Completed ? HabitCompletion.Succeed : HabitCompletion.Failed;
        }

        public DurationData IdealDuration = new();
        public DurationData RemainingDuration = new();
        //public List<TimerHistory> Histories = new();

        public GoalTimerResultData(DurationData _idealDuration)
        {
            IdealDuration = _idealDuration;
        }
        
        
        public int GetRemainingDuration()
        {
            return RemainingDuration.GetSeconds();
        }

        public override float GetValue()
        {
            var _value = 0f;
            
            _value = RemainingDuration.GetSeconds() - IdealDuration.GetSeconds();

            return _value;
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            
            _json.Add("ideal", IdealDuration.SerializeData());
            _json.Add("remain", RemainingDuration.SerializeData());
            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
            IdealDuration.DeserializeData(_json["ideal"].AsObject);
            RemainingDuration.DeserializeData(_json["remain"].AsObject);
        }
    }
}