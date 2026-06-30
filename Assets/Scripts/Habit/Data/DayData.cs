using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class DayData : ISerializableData
    {
        public DateTime Date = DateTime.Now;
        public ResultData ResultData;

        public float Score;

        public DayData(ResultData _result)
        {
            ResultData = _result;
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("date", Date);

            _json.Add("result", ResultData.SerializeData());

            _json.Add("score", Score);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Date = _json["date"];

            ResultData = LoadResult(_json["result"].AsObject);

            Score = _json["score"];
        }

        private ResultData LoadResult(JSONObject _json)
        {
            ResultData _mode = null;

            switch ((GameModeType)_json["type"].AsInt)
            {
                case GameModeType.Alarm:
                    _mode = new AlarmResultData();
                    break;
                case GameModeType.GoalTimer:
                    _mode = new GoalTimerResultData(new DurationData());
                    break;
                case GameModeType.CheckList:
                    _mode = new CheckListResultData(new List<string>());
                    break;
                case GameModeType.Quota:
                    _mode = new QuotaResultData();
                    break;
            }

            _mode?.DeserializeData(_json);

            return _mode;
        }
    }
}