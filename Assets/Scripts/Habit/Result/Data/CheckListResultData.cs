using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Habillage
{
    public class CheckListResultData : ResultData
    {
        public override GameModeType Type => GameModeType.CheckList;
        public override HabitCompletion CompleteStat()
        {
            //return FinishedTasks.Count >= Tasks.Count ? HabitCompletion.Succeed : HabitCompletion.Failed;
            return Completed ? HabitCompletion.Succeed : HabitCompletion.Failed;
        }

        public List<string> Tasks = new();
        public List<string> FinishedTasks = new();
        public bool OnTime;

        public CheckListResultData(List<string> _tasks)
        {
            Tasks = new List<string>(_tasks);
        }

        public override float GetValue()
        {
            return FinishedTasks.Count;
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            _json.Add("ontime", OnTime);

            var _taskArr = new JSONArray();
            foreach (var _task in Tasks)
            {
                _taskArr.Add(_task);
            }

            _json.Add("task", _taskArr);

            var _finishedArr = new JSONArray();
            foreach (var _task in FinishedTasks)
            {
                _finishedArr.Add(_task);
            }

            _json.Add("finished", _finishedArr);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);

            OnTime = _json["ontime"];
            Tasks = new List<string>();
            foreach (var _taskNode in _json["task"])
            {
                Tasks.Add(_taskNode.Value);
            }

            FinishedTasks = new List<string>();
            foreach (var _taskNode in _json["finished"])
            {
                FinishedTasks.Add(_taskNode.Value);
            }
        }
    }
}