using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class ScheduleSelection : MonoBehaviour
    {
        public ScheduleType scheduleSelectionMode = ScheduleType.Daily;
        [SerializeField] private SelectionBox selectionBox;
        
        public SerializedDictionary<ScheduleType, ScheduleSelectionUI> ScheduleSelectionUis = new();
        [SerializeField] private ScheduleDailySelectionUI daily;
        [SerializeField] private ScheduleWeeklySelectionUI weekly;
        [SerializeField] private ScheduleMonthlySelectionUI monthly;
        [SerializeField] private ScheduleRepeatedlySelectionUI repeatedly;

        public UnityEvent<ScheduleSelectionUI> OnChangedMode;

        void Start()
        {
            UpdateSelectionMode();
        }

        public void UpdateSelectionMode()
        {
            int temp = selectionBox.CurrentTextNumber();
            switch (temp)
            {
                case 0:
                    scheduleSelectionMode = ScheduleType.Daily;
                    break;
                case 1:
                    scheduleSelectionMode = ScheduleType.Weekly;
                    break;
                case 2:
                    scheduleSelectionMode = ScheduleType.Monthly;
                    break;
                case 3:
                    scheduleSelectionMode = ScheduleType.Repeatedly;
                    break;
            }

            // Debug.Log("UpdateSelection");
            
            switch (scheduleSelectionMode)
            {
                case ScheduleType.Daily:
                    daily.gameObject.SetActive(true);
                    weekly.gameObject.SetActive(false);
                    monthly.gameObject.SetActive(false);
                    repeatedly.gameObject.SetActive(false);
                    break;
                case ScheduleType.Weekly:
                    daily.gameObject.SetActive(false);
                    weekly.gameObject.SetActive(true);
                    monthly.gameObject.SetActive(false);
                    repeatedly.gameObject.SetActive(false);
                    break;
                case ScheduleType.Monthly:
                    daily.gameObject.SetActive(false);
                    weekly.gameObject.SetActive(false);
                    monthly.gameObject.SetActive(true);
                    repeatedly.gameObject.SetActive(false);
                    break;
                case ScheduleType.Repeatedly:
                    daily.gameObject.SetActive(false);
                    weekly.gameObject.SetActive(false);
                    monthly.gameObject.SetActive(false);
                    repeatedly.gameObject.SetActive(true);
                    break;
            }
            Canvas.ForceUpdateCanvases();
            
            OnChangedMode?.Invoke(ScheduleSelectionUis[scheduleSelectionMode]);
        }
    }
}
