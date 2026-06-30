using UnityEngine;

namespace Habillage
{
    public abstract class ScheduleSelectionUI : MonoBehaviour
    {
        public abstract ScheduleType Type { get; }

        public abstract ScheduleData GetScheduleData();

        public abstract bool IsValid();
    }
}