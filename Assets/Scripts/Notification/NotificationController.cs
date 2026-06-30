using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

// https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.0/manual/index.html

namespace Habillage
{
    public class NotificationController : MonoBehaviour
    {
        [SerializeField] AndroidNotificationControl androidNotificationControl;

        void Start()
        {
#if UNITY_ANDROID
            androidNotificationControl.RequestAuthorization();
            androidNotificationControl.RegisterNotificationChannel();
#endif
        }

        public void DailySetNotification()
        {
#if UNITY_ANDROID
            // Check if habit will need to be done today
            // Those habit where needed to be done today, set notifcation equal to Actual Time - Notify me before...
            // androidNotificationControl.SendNotification("", "[]", DateTime.Now.AddSeconds(5));

            foreach (var _habitData in PlayerData.Data.CreatedHabits.Values)
            {
                if (_habitData.ScheduleData.ValidToday())
                {
                    var _notiTime = _habitData.ModeData.Notify.ToTimeSpan();
                    androidNotificationControl.SendNotification(_habitData.Title, _habitData.Description, DateTime.Today.Add(_notiTime), IconListEnum.small, IconListEnum.large, 0);
                }
            }
#endif
            foreach (var _habitData in PlayerData.Data.CreatedHabits.Values)
            {

                var _notiTime = _habitData.ModeData.Notify.ToTimeSpan();
                Debug.Log("set-up notification named " + _habitData.Title.ToString() + " at " + _notiTime.ToString());
            }
        }

        public void DailyResetNotification()
        {
#if UNITY_ANDROID
            var totalHabit = PlayerData.Data.CreatedHabits.Where(_t => _t.Value.ScheduleData.ValidToday()).Count();
            var _doneToday = PlayerData.Data.CreatedHabits.Where(_h =>
                            _h.Value.DaysData.ContainsKey(DateTime.Today.ToShortDateString())).Count(_h =>
                            _h.Value.DaysData[DateTime.Today.ToShortDateString()].ResultData.CompleteStat() ==
                            HabitCompletion.Succeed);

            if (_doneToday >= totalHabit)
            {
                androidNotificationControl.SendNotification("Daily habit reset!", "Perfect score, Goodjob! Let's you keep this consistency!", DateTime.Today, IconListEnum.small, IconListEnum.happy, 0);
            }
            else if (_doneToday > 0 && _doneToday < totalHabit)
            {
                androidNotificationControl.SendNotification("Daily habit reset!", "Let's do it better today!", DateTime.Today, IconListEnum.small, IconListEnum.smile, 0);
            }
            else if (_doneToday <= 0)
            {
                androidNotificationControl.SendNotification("Daily habit reset!", "We miss you! Come back and start correcting your routine!", DateTime.Today, IconListEnum.small, IconListEnum.sad, 0);
            }
#endif
        }
    }
}

