using UnityEngine;

namespace Habillage
{
    public abstract class HabitModeSettingUI : MonoBehaviour
    {
        public abstract GameModeType Type { get; }
        public NotificationTimeSettingUI NotifySetting; //TODO: create new ui and assign ref in scene

        public abstract ModeData GetModeData();
        public TimeData GetNotifyData() => NotifySetting.GetTime();

        public abstract bool IsValid();
    }
}