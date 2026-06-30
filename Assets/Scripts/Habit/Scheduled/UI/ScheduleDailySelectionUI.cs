namespace Habillage
{
    public class ScheduleDailySelectionUI : ScheduleSelectionUI
    {
        public override ScheduleType Type => ScheduleType.Daily;
        public override ScheduleData GetScheduleData()
        {
            return new Daily();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}