using System;
using System.Collections;
using System.Collections.Generic;
using Habillage;
using UnityEngine;

namespace Consistenant
{
    public class DailyControl : MonoBehaviour
    {
        [SerializeField] private NotificationController notificationController;
        [SerializeField] private RecordScores recordScores;
        [SerializeField] public GameObject RewardsRedAlert;
        [SerializeField] public DailyRewards dailyRewards;
        [SerializeField] GoalTimer goalTimer;
        [SerializeField] Checklist checklist;
        [SerializeField] Quota quota;
        void Start()
        {
            dailyRewards.gameObject.SetActive(!PlayerData.Data.DRtaken && PlayerData.Data.Daily_Rewards >= dailyRewards.Rewards.Count);
        }
        void Update()
        {
            if (DateTime.Today > PlayerData.Data.NotedDay)
            {
                notificationController.DailySetNotification();
                notificationController.DailyResetNotification();
                recordScores.DailyReset();
                PlayerData.Data.DRtaken = false;
                PlayerData.WriteSave();
                PlayerData.Data.NotedDay = DateTime.Today;
                if (goalTimer.Parent.activeSelf)
                {
                    goalTimer.Back();
                }
                if (checklist.Parent.activeSelf)
                {
                    checklist.Back();
                }
                if (quota.Parent.activeSelf)
                {
                    quota.Back();
                }
            }
            RewardsRedAlert.SetActive(!PlayerData.Data.DRtaken);
        }
    }
}
