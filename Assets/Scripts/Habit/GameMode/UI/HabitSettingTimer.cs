using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public class HabitSettingTimer : HabitModeSettingUI
    {
        public TimeSettingUI TimeSetting;

        public DurationData GetTimer()
        {
            return TimeSetting.GetDuration();
        }

        public override GameModeType Type => GameModeType.GoalTimer;
        public override ModeData GetModeData()
        {
            return new GoalTimerData(GetNotifyData(), GetTimer());
        }

        public override bool IsValid()
        {
            return GetTimer().GetSeconds() >= 30;
        }
    }
}
