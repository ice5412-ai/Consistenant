using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public class HabitSettingAlarm : HabitModeSettingUI
    {
        public ScrollMechanic Hours;
        public ScrollMechanic Minute;
        public ScrollMechanic Second;
        public ScrollMechanic AMPM;

        public TimeData GetTime()
        {
            return new TimeData()
            {
                Hours = Hours.GetCurrentValue() + 1,
                Minute = Minute.GetCurrentValue(),
                Second = Second.GetCurrentValue(),
                IsAm = AMPM.GetCurrentValue().Equals(0)
            };
        }

        public override GameModeType Type => GameModeType.Alarm;
        public override ModeData GetModeData()
        {
            //Debug.Log(GetTime());
            return new AlarmData(GetNotifyData(), GetTime());
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
