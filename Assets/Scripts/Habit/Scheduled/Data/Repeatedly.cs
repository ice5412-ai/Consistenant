using System;
using SimpleJSON;

namespace Habillage
{
    public class Repeatedly : ScheduleData
    {
        public override ScheduleType Type => ScheduleType.Repeatedly;

        public int Days;
        public DateTime CreationDate;
        
        public Repeatedly(int _days, DateTime _creationDate)
        {
            Days = _days;
            CreationDate = _creationDate;
        }

        public override string ToString()
        {
            return Days >= 2 ? $"Every {Days} days" : $"Every {Days} day";
        }

        public override bool ValidToday()
        {
            return ValidThisDay(DateTime.Now);
        }

        public override bool ValidThisDay(DateTime _date)
        {
            return (_date - CreationDate).Days % Days == 0;
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            
            _json.Add("days", Days);
            _json.Add("create", CreationDate);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);

            Days = _json["days"];
            CreationDate = _json["create"];
        }
    }
}