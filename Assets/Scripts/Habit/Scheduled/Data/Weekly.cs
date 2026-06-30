using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Habillage
{
    public class Weekly : ScheduleData
    {
        public override ScheduleType Type => ScheduleType.Weekly;

        //0 = SunDay, 6 = Saturday https://learn.microsoft.com/en-us/dotnet/api/system.dayofweek?view=net-8.0
        public List<DayOfWeek> DaysOfWeek { get; set; }
        
        public Weekly(List<DayOfWeek> _daysOfWeek)
        {
            DaysOfWeek = new List<DayOfWeek>(_daysOfWeek);
        }
        
        public bool ContainDay(int _day)
        {
            return DaysOfWeek.Contains((DayOfWeek)_day);
        }
        
        public bool ContainDay(DayOfWeek _day)
        {
            return DaysOfWeek.Contains(_day);
        }

        public override string ToString()
        {
            var _result = "";

            foreach (var _day in DaysOfWeek)
            {
                _result += _day;
                _result += " ";
            }

            return _result;
        }

        public override bool ValidToday()
        {
            return ValidThisDay(DateTime.Now);
        }

        public override bool ValidThisDay(DateTime _date)
        {
            return DaysOfWeek.Contains(_date.DayOfWeek);
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            var _dayArr = new JSONArray();

            foreach (var _day in DaysOfWeek)
            {
                _dayArr.Add((int)_day);
            }
            
            _json.Add("days", _dayArr);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
            
            DaysOfWeek.Clear();
            foreach (var _day in _json["days"].Values)
            {
                DaysOfWeek.Add((DayOfWeek)_day.AsInt);
            }
        }


    }
}