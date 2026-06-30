using System;
using SimpleJSON;

namespace Habillage
{
    public class HabitStreakData : ISerializableData
    {
        public int Total;
        public int CurrentStreak;
        public DateTime RecentCompletion;
        public int BestStreak;
        
        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("total", Total);
            _json.Add("current", CurrentStreak);
            _json.Add("recent", RecentCompletion);
            _json.Add("best", BestStreak);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Total = _json["total"];
            CurrentStreak = _json["current"];
            RecentCompletion = _json["recent"];
            BestStreak = _json["best"];
        }
    }
}