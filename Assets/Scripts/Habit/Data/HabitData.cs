using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class HabitData : ISerializableData
    {
        public string Title;
        public ModeData ModeData;
        public string Description;
        public ScheduleData ScheduleData;
        public ColorPresetEnum Color;
        public DateTime CreationDate = DateTime.Now;

        public int notificationId;
        
        public Dictionary<string, DayData> DaysData = new();

        public HabitStreakData CalculateStreak()
        {
            var _streakData = new HabitStreakData();
            var _best = 0;
            var _updatedRecent = false;
            
            for (DateTime date = DateTime.Today; date >= CreationDate.Date; date = date.AddDays(-1))
            {
                if (!ScheduleData.ValidThisDay(date)) continue;
                
                if (DaysData.TryGetValue(date.ToShortDateString(), out var _dayData))
                {
                    if (_dayData.ResultData.CompleteStat() == HabitCompletion.Succeed)
                    {
                        if (!_updatedRecent)
                        {
                            _streakData.RecentCompletion = date;
                            _updatedRecent = true;
                        }
                        
                        _streakData.CurrentStreak++;
                        _streakData.Total++;
                        
                        if (_streakData.CurrentStreak > _best)
                        {
                            _best = _streakData.CurrentStreak;
                        }
                    }
                    else
                    {
                        _streakData.CurrentStreak = 0;
                    }
                }
                else
                {
                    _streakData.CurrentStreak = 0;
                }
            }

            _streakData.BestStreak = _best;

            return _streakData;
        }

        public void AddDayData(DateTime _day, DayData _data)
        {
            DaysData.Add(_day.ToShortDateString(), _data);
        }

        public float GetMaxValue()
        {
            return DaysData.Any() ? DaysData.Values.Max(_d => _d.ResultData.GetValue()) : 0f;
        }

        public HabitCompletion GetCompletion(DateTime _day)
        {
            //Before create habit or today, not reach yet
            if (_day < CreationDate.Date || _day > DateTime.Today)
            {
                return HabitCompletion.Unfilled;
            }
            else
            {
                if (DaysData.TryGetValue(_day.ToShortDateString(), out var _dayData))
                {
                    return _dayData.ResultData.CompleteStat();
                }

                if (_day == DateTime.Today)
                {
                    return HabitCompletion.Unfilled;
                }
                //Doesn't have day data, Failed
                else
                {
                    return HabitCompletion.Failed;
                }
            }
            return HabitCompletion.Unfilled;
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("title", Title);
            _json.Add("mode", ModeData.SerializeData());
            _json.Add("description", Description);
            _json.Add("schedule", ScheduleData.SerializeData());
            _json.Add("color", (int)Color);
            _json.Add("create", CreationDate);

            var _daysData = new JSONObject();
            foreach (var _kvp in DaysData)
            {
                _daysData.Add(_kvp.Key, _kvp.Value.SerializeData());
            }
            _json.Add("days_data", _daysData);
            _json.Add("notificationId", notificationId);
            
            // _json.Add("streak", Streak.SerializeData());

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Title = _json["title"];
            ModeData = LoadMode(_json["mode"].AsObject);
            Description = _json["description"];
            ScheduleData = LoadSchedule(_json["schedule"].AsObject);
            Color = (ColorPresetEnum)_json["color"].AsInt;
            CreationDate = _json["create"];
            DaysData = new Dictionary<string, DayData>();

            foreach (var _dayNode in _json["days_data"])
            {
                var _data = new DayData(null);
                _data.DeserializeData(_dayNode.Value.AsObject);
                DaysData.Add(_dayNode.Key, _data);
            }
            notificationId = _json["notificationId"];
            
            // var _streak = new HabitStreakData();
            // _streak.DeserializeData(_json["streak"].AsObject);
            // Streak = _streak;
        }

        private ModeData LoadMode(JSONObject _json)
        {
            ModeData _data = null;

            switch ((GameModeType)_json["type"].AsInt)
            {
                case GameModeType.Alarm:
                    _data = new AlarmData(new TimeData(), new TimeData());
                    break;
                case GameModeType.GoalTimer:
                    _data = new GoalTimerData(new TimeData(), new DurationData());
                    break;
                case GameModeType.CheckList:
                    _data = new CheckListData(new TimeData(), new List<string>());
                    break;
                case GameModeType.Quota:
                    _data = new QuotaData(new TimeData(), Vector2.zero);
                    break;
            }
            _data?.DeserializeData(_json);

            return _data;
        }
        
        private ScheduleData LoadSchedule(JSONObject _json)
        {
            ScheduleData _scheduleData = null;
            switch ((ScheduleType)_json["type"].AsInt)
            {
                case ScheduleType.Daily:
                    _scheduleData = new Daily();
                    break;
                case ScheduleType.Weekly:
                    _scheduleData = new Weekly(new List<DayOfWeek>());
                    break;
                case ScheduleType.Monthly:
                    _scheduleData = new Monthly(new List<int>(), false);
                    break;
                case ScheduleType.Repeatedly:
                    _scheduleData = new Repeatedly(0, DateTime.Today);
                    break;
            }

            _scheduleData?.DeserializeData(_json);
            
            return _scheduleData;
        }
    }
}
