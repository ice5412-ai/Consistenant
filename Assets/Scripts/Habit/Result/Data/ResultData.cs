using System;
using SimpleJSON;

namespace Habillage
{
    public abstract class ResultData : ISerializableData
    {
        public abstract GameModeType Type { get; }
        public abstract HabitCompletion CompleteStat();
        public float Score;
        public bool Completed;

        public abstract float GetValue();
        
        public virtual JSONObject SerializeData()
        {
            var _json = new JSONObject();
            
            _json.Add("type", (int)Type);
            _json.Add("score", Score);
            _json.Add("completed", Completed);
            
            return _json;
        }

        public virtual void DeserializeData(JSONObject _json)
        {
            Score = _json["score"];
            Completed = _json["completed"];
        }
    }
}