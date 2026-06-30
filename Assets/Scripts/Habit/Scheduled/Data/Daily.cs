using System;

namespace Habillage
{
    public class Daily : ScheduleData
    {
        public override ScheduleType Type => ScheduleType.Daily;
        public override string ToString()
        {
            return "Daily";
        }

        public override bool ValidToday()
        {
            return true;
        }

        public override bool ValidThisDay(DateTime _date)
        {
            return true;
        }
    }
}