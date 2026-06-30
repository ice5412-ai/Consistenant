using System.Collections.Generic;
using SimpleJSON;

namespace Habillage
{
    public class CheckListData : ModeData
    {
        public override GameModeType Type => GameModeType.CheckList;

        public List<string> Tasks = new();

        public CheckListData(TimeData _notify, List<string> _tasks) : base(_notify)
        {
            Tasks = new List<string>(_tasks);
        }

        public override string ToString()
        {
            return string.Join(",", Tasks);
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            var _taskArr = new JSONArray();

            foreach (var _task in Tasks)
            {
                _taskArr.Add(_task);
            }
            _json.Add("task", _taskArr);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);

            Tasks = new List<string>();
            foreach (var _taskNode in _json["task"])
            {
                Tasks.Add(_taskNode.Value);
            }
        }
    }
}