using System.Collections;
using System.Collections.Generic;
using Habillage;
using UnityEngine;

namespace Habillage
{
    public class NotificationTimeSettingUI : MonoBehaviour
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
    }
}
