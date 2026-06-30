using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Habillage
{
    public class Monthly : ScheduleData
    {
        public override ScheduleType Type => ScheduleType.Monthly;

        public List<int> DaysOfMonth;
        public bool LastDay;
        
        public Monthly(List<int> _daysOfMonth, bool _lastDay)
        {
            DaysOfMonth = new List<int>(_daysOfMonth);
            LastDay = _lastDay;
        }
        
        public bool ContainDay(int _day)
        {
            return DaysOfMonth.Contains(_day);
        }

        public override string ToString()
        {
            var _result = "";

            foreach (var _day in DaysOfMonth)
            {
                _result += $"{_day} ";
            }

            if (LastDay)
            {
                _result += "LastDay";
            }

            return _result;
        }

        public override bool ValidToday()
        {
            return ValidThisDay(DateTime.Now);
        }

        public override bool ValidThisDay(DateTime _date)
        {
            if (LastDay)
            {
                if (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) == _date.Day)
                {
                    return true;
                }
            }

            return DaysOfMonth.Contains(_date.Day);
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();

            var _dayArr = new JSONArray();

            foreach (var _day in DaysOfMonth)
            {
                _dayArr.Add(_day);
            }
            
            _json.Add("days", _dayArr);
            _json.Add("last", LastDay);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);
                
            DaysOfMonth.Clear();
            foreach (var _day in _json["days"].Values)
            {
                DaysOfMonth.Add(_day);
            }

            LastDay = _json["last"];
        }


    }
}