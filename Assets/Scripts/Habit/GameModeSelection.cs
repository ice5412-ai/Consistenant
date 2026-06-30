using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UI.Pagination;
using UnityEngine;
using UnityEngine.Events;

namespace Habillage
{
    public class GameModeSelection : MonoBehaviour
    {
        public GameModeType gameModeType {get; private set;} = GameModeType.Alarm;
        [SerializeField] public PagedRect pagedRect;
        [SerializeField] private HabitSettingAlarm alarm;
        [SerializeField] private HabitSettingTimer goalTimer; 
        [SerializeField] private HabitSettingChecklist checkList;
        [SerializeField] private HabitSettingQuota quota;

        public SerializedDictionary<GameModeType, HabitModeSettingUI> SettingUis = new();
        public UnityEvent<HabitModeSettingUI> OnChangedMode;

        void Start()
        {
            UpdateSelectionMode();
        }

        public void UpdateSelectionMode()
        {
            int temp = pagedRect.GetPageNumber(pagedRect.GetCurrentPage())-1;
            switch (temp)
            {
                case 0:
                    gameModeType = GameModeType.Alarm;
                    break;
                case 1:
                    gameModeType = GameModeType.GoalTimer;
                    break;
                case 2:
                    gameModeType = GameModeType.CheckList;
                    break;
                case 3:
                    gameModeType = GameModeType.Quota;
                    break;
            }

            switch (gameModeType)
            {
                case GameModeType.Alarm:
                    alarm.gameObject.SetActive(true);
                    goalTimer.gameObject.SetActive(false);
                    checkList.gameObject.SetActive(false);
                    quota.gameObject.SetActive(false);
                    break;
                case GameModeType.GoalTimer:
                    alarm.gameObject.SetActive(false);
                    goalTimer.gameObject.SetActive(true);
                    checkList.gameObject.SetActive(false);
                    quota.gameObject.SetActive(false);
                    break;
                case GameModeType.CheckList:
                    alarm.gameObject.SetActive(false);
                    goalTimer.gameObject.SetActive(false);
                    checkList.gameObject.SetActive(true);
                    quota.gameObject.SetActive(false);
                    break;
                case GameModeType.Quota:
                    alarm.gameObject.SetActive(false);
                    goalTimer.gameObject.SetActive(false);
                    checkList.gameObject.SetActive(false);
                    quota.gameObject.SetActive(true);
                    break;
            }
            Canvas.ForceUpdateCanvases();
            
            OnChangedMode?.Invoke(SettingUis[gameModeType]);
        }
    }
}
