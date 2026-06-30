using System;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class QuotaResultData : ResultData
    {
        public override GameModeType Type => GameModeType.Quota;
        public override HabitCompletion CompleteStat()
        {
            /*
            if (Ideal.x == 0)
            {
                return CurrentValue <= Ideal.y ? HabitCompletion.Succeed : HabitCompletion.Failed;
            }

            if (Ideal.y == 0)
            {
                return CurrentValue >= Ideal.x ? HabitCompletion.Succeed : HabitCompletion.Failed;
            }
            return Ideal.x <= CurrentValue && CurrentValue <= Ideal.y ? HabitCompletion.Succeed : HabitCompletion.Failed;
            */
            return Completed ? HabitCompletion.Succeed : HabitCompletion.Failed;
        }

        public Vector2 Ideal;
        public float CurrentValue;

        public override float GetValue()
        {
            return CurrentValue;
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            _json.Add("current", CurrentValue);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);

            CurrentValue = _json["current"];
        }
    }
}