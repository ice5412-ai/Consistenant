using UnityEngine;

namespace Habillage
{
    public class TimeSettingUI : MonoBehaviour
    {
        public ScrollMechanic Hours;
        public ScrollMechanic Minute;
        public ScrollMechanic Second;

        public int GetHours() => Hours.GetCurrentValue();
        public int GetMinute() => Minute.GetCurrentValue();
        public int GetSecond() => Second.GetCurrentValue();

        public DurationData GetDuration()
        {
            return new DurationData()
            {
                Hours = GetHours(),
                Minute = GetMinute(),
                Second = GetSecond()
            };
        }
    }
}