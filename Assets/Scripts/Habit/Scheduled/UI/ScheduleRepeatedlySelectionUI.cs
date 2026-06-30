using System;
using UnityEngine;

namespace Habillage
{
    public class ScheduleRepeatedlySelectionUI : ScheduleSelectionUI
    {
        public ScrollMechanic Day;
        public int Days => Day.GetCurrentValue() + 1;

        public override ScheduleType Type => ScheduleType.Repeatedly;
        public override ScheduleData GetScheduleData()
        {
            return new Repeatedly(Days, DateTime.Today);
        }

        public override bool IsValid()
        {
            return Days > 0;
        }
    }
}