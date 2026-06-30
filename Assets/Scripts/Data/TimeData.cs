using System;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class TimeData : ISerializableData
    {
        public int Hours;
        public int Minute;
        public int Second;
        public bool IsAm;

        public override string ToString()
        {
            var _am = IsAm ? "AM" : "PM";
            return $"{Hours:00}:{Minute:00}:{Second:00} {_am}";
        }

        public TimeSpan ToTimeSpan()
        {
            var _hours = 0;
            if (IsAm)
            {
                if (Hours == 12)
                {
                    _hours = 0;
                }
                else
                {
                    _hours = Hours;
                }
            }
            else
            {
                if (Hours == 12)
                {
                    _hours = 12;
                }
                else
                {
                    _hours = Hours + 12;
                }
            }

            return new TimeSpan(_hours, Minute, Second);
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("hours", Hours);
            _json.Add("minute", Minute);
            _json.Add("second", Second);
            _json.Add("am", IsAm);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Hours = _json["hours"];
            Minute = _json["minute"];
            Second = _json["second"];
            IsAm = _json["am"];
        }
    }
}