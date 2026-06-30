using System;
using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public static class DebugData
    {
        public static HabitData AlarmData()
        {
            var _modeData = new AlarmData(new TimeData { Minute = 30 }, new TimeData { Hours = 11, Minute = 8});
            var _habitData = new HabitData
            {
                Title = "Alarm",
                ModeData = _modeData,
                Description = "Alarm description",
                ScheduleData = new Daily(),
                Color = ColorPresetEnum.Aquamarine,
                // Streak =
                // {
                //     Total = 7,
                //     BestStreak = 5,
                //     CurrentStreak = 3,
                //     RecentCompletion = DateTime.Now
                // }
            };
            var _stopTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 11, 50, 0);
            
            var _result = new AlarmResultData();
            //_habitData.AddDayData(DateTime.Now, new DayData(_result));

            return _habitData;
        }

        public static HabitData TimerData()
        {
            var _duration = new DurationData { Hours = 1, Minute = 35 };
            var _modeData = new GoalTimerData(new TimeData { Minute = 30 }, _duration);

            var _habitData = new HabitData
            {
                Title = "Test Data: Timer Habit",
                ModeData = _modeData,
                Description = "Timer debug description",
                ScheduleData = new Weekly(new List<DayOfWeek>
                {
                    DayOfWeek.Monday, DayOfWeek.Friday
                }),
                Color = ColorPresetEnum.Turquoise,
                // Streak =
                // {
                //     Total = 7,
                //     BestStreak = 5,
                //     CurrentStreak = 3,
                //     RecentCompletion = DateTime.Now
                // }
            };

            var _result = new GoalTimerResultData(_duration)
            {
                RemainingDuration = new DurationData(){Second = 500}
            };
            
            _habitData.DaysData[DateTime.Today.ToShortDateString()] = new DayData(_result);
            //_habitData.AddDayData(DateTime.Now, new DayData(_result));
            //PlayerData.Data.AddHabit(_habitData);
            return _habitData;
        }

        public static HabitData QuotaData()
        {
            var _ideal = new Vector2(1000, 2300.5f);
            var _modeData = new QuotaData(new TimeData { Minute = 30 }, _ideal);
            
            var _habitData = new HabitData
            {
                Title = "Test Data: Quota Habit",
                ModeData = _modeData,
                Description = "Quota debug description",
                ScheduleData = new Monthly(new List<int>(), true),
                Color = ColorPresetEnum.Magenta,
                // Streak =
                // {
                //     Total = 7,
                //     BestStreak = 5,
                //     CurrentStreak = 3,
                //     RecentCompletion = DateTime.Now
                // }
            };

            var _result = new QuotaResultData()
            {
                CurrentValue = 200f,
                Ideal = _ideal
            };
            
            _habitData.DaysData[DateTime.Today.ToShortDateString()] = new DayData(_result);
            
            return _habitData;
        }

        public static HabitData CheckListData()
        {
            var _task = new List<string>() { "task1", "task2", "task3" };
            var _modeData = new CheckListData(new TimeData { Minute = 30 }, _task);
            
            var _habitData = new HabitData
            {
                Title = "Test Data: CheckList Habit",
                ModeData = _modeData,
                Description = "CheckList debug description",
                ScheduleData = new Daily(),
                Color = ColorPresetEnum.Turquoise,
                // Streak =
                // {
                //     Total = 7,
                //     BestStreak = 5,
                //     CurrentStreak = 3,
                //     RecentCompletion = DateTime.Now
                // }
            };

            var _result = new CheckListResultData(_task)
            {
                FinishedTasks = new List<string>() { "task1" }
            };
            _habitData.DaysData[DateTime.Today.ToShortDateString()] = new DayData(_result);

            return _habitData;
        }
    }
}