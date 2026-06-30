using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AYellowpaper.SerializedCollections;
using JimmysUnityUtilities;
using TMPro;
using UI.Pagination;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class HabitEditUI : MonoBehaviour
    {
        public TMP_InputField TitleInputField;
        public HabitModeSettingUI HabitModeUI;
        public TMP_InputField DescriptionInputField;
        public ScheduleSelectionUI ScheduleUI;

        public SerializedDictionary<GameModeType, HabitModeSettingUI> SettingUis = new();
        public SerializedDictionary<ScheduleType, ScheduleSelectionUI> ScheduleUis = new();

        public ColorPicker colorPicker;

        public TextMeshProUGUI WindowTitle;
        public GameModeSelection gameModeSelection;
        public SelectionBox ScheduleSelectionBox;
        public TextMeshProUGUI OGRepeated, OGAlarm, OGTimer, OGNoti;
        public HabitSettingChecklist checklist;
        public TMP_InputField QuotaMin, QuotaMax;

        public GameObject ErrorSaveWindow;
        public GameObject ConfirmationOnEdit;
        public TextMeshProUGUI errorOutput;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] AndroidNotificationControl androidNotificationControl;

        [SerializeField] HabitStatsUI habitStatsUI;
        [SerializeField] private ContentSizeFitter root;
        [SerializeField] private RecordScores recordScores;

        public void LoadHabit()
        {
            scrollRect.verticalNormalizedPosition = 1f;
            var _data = PlayerData.Data.CurrentHabit;
            var _gameModeType = _data.ModeData.Type;
            var _scheduleType = _data.ScheduleData.Type;

            TitleInputField.text = _data.Title;
            UpdateTitle();

            gameModeSelection.pagedRect.DefaultPage = (int)_gameModeType + 1;
            gameModeSelection.pagedRect.SetCurrentPage((int)_gameModeType + 1);

            switch (_gameModeType)
            {
                case GameModeType.Alarm:
                    var _alarm = (AlarmData)_data.ModeData;
                    OGAlarm.transform.parent.gameObject.SetActive(true);
                    OGAlarm.text = _alarm.AlarmTime.ToString();
                    break;

                case GameModeType.GoalTimer:
                    var _timer = (GoalTimerData)_data.ModeData;
                    OGTimer.transform.parent.gameObject.SetActive(true);
                    OGTimer.text = _timer.IdealTime.ToString();
                    break;

                case GameModeType.CheckList:
                    var _checklist = (CheckListData)_data.ModeData;
                    checklist.SyncCheckList(_checklist.Tasks);
                    break;

                case GameModeType.Quota:
                    var _quota = (QuotaData)_data.ModeData;
                    QuotaMin.text = _quota.IdealValue.x.ToString(CultureInfo.InvariantCulture);
                    QuotaMax.text = _quota.IdealValue.y.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            DescriptionInputField.text = _data.Description;

            ScheduleSelectionBox.ForceSetInitialBeginWithTextNumber((int)_scheduleType);

            switch (_scheduleType)
            {
                case ScheduleType.Daily:
                    break;
                case ScheduleType.Weekly:
                    var _weekly = (ScheduleWeeklySelectionUI)ScheduleUis[_scheduleType];
                    var _weeklyData = (Weekly)_data.ScheduleData;
                    _weekly.SyncSelectedDays(_weeklyData.DaysOfWeek);
                    break;
                case ScheduleType.Monthly:
                    var _monthly = (ScheduleMonthlySelectionUI)ScheduleUis[_scheduleType];
                    var _monthlyData = (Monthly)_data.ScheduleData;
                    _monthly.SyncSelectedDays(_monthlyData.DaysOfMonth, _monthlyData.LastDay);
                    break;
                case ScheduleType.Repeatedly:
                    OGRepeated.transform.parent.gameObject.SetActive(true);
                    var _repeatedly = (ScheduleRepeatedlySelectionUI)ScheduleUis[_scheduleType];
                    var _repeatedlyData = (Repeatedly)_data.ScheduleData;
                    _repeatedly.Day.currentCenter = _repeatedlyData.Days;
                    OGRepeated.text = _repeatedlyData.Days.ToString();
                    break;
            }

            ScheduleUI = ScheduleUis[_scheduleType];
            HabitModeUI = SettingUis[_gameModeType];

            OGNoti.transform.parent.gameObject.SetActive(true);
            OGNoti.text = _data.ModeData.Notify.ToString();

            colorPicker.ColorPick((int)_data.Color);
            root.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // Also save or overwrite saved Data of same Habit before

        public void ConfirmationWindow()
        {
            if (TitleInputField.text.IsNullOrWhiteSpace())
            {
                ErrorSaveWindow.SetActive(true);
                errorOutput.text = ($"Title is invalid, Please check it again!");
                return;
            }

            if (PlayerData.Data.CurrentHabit.Title != TitleInputField.text && PlayerData.Data.CreatedHabits.ContainsKey(TitleInputField.text))
            {
                ErrorSaveWindow.SetActive(true);
                errorOutput.text = ($"Title already exist!");
                return;
            }

            if (!ScheduleUI.IsValid())
            {
                ErrorSaveWindow.SetActive(true);
                errorOutput.text = ($"Schedule is invalid, Please check it again!");
                return;
            }

            if (!HabitModeUI.IsValid())
            {
                ErrorSaveWindow.SetActive(true);
                errorOutput.text = ($"Setting is invalid, Please check it again!");
                return;
            }
            ConfirmationOnEdit.SetActive(true);
        }

        public void save()
        {
            var _currData = PlayerData.Data.CurrentHabit;
            var _prevTitle = _currData.Title;

            _currData.Title = TitleInputField.text;
            _currData.ModeData = CreateMode(HabitModeUI, _currData.ModeData);
            _currData.Description = DescriptionInputField.text;
            _currData.ScheduleData = CreateSchedule(ScheduleUI, _currData.ScheduleData);
            _currData.Color = colorPicker.selectedColor;
            _currData.notificationId = PlayerData.Data.CurrentHabit.notificationId;

            PlayerData.Data.CreatedHabits.Remove(_prevTitle);
            PlayerData.Data.AddHabit(_currData);
            if (PlayerData.Data.CurrentRoom)
            {
                PlayerData.Data.CurrentRoom.HabitTitle = _currData.Title;
            }
            ResultData _resetData = null;
            if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
            {
                recordScores.ScoreAdd(-_dayData.Score);
            }
            PlayerData.Data.CurrentHabit.DaysData.Remove(DateTime.Today.ToShortDateString());
            PlayerData.WriteSave();
            gameObject.SetActive(false);
            habitStatsUI.ReturnedToHabitStatsUI();
            Debug.Log(PlayerData.Data.SerializeData().ToString());

            if (_currData.ScheduleData.ValidToday())
            {
                var _notiTime = _currData.ModeData.Notify.ToTimeSpan();
                // Debug.Log(_notiTime);
#if UNITY_ANDROID
                androidNotificationControl.SendNotification(_currData.Title, _currData.Description, DateTime.Today.Add(_notiTime), IconListEnum.small, IconListEnum.large, _currData.notificationId);
#endif
            }
        }

        public void UpdateTitle()
        {
            WindowTitle.text = $"Edit: {TitleInputField.text}";
        }

        public void UpdateMode(HabitModeSettingUI _modeUI)
        {
            HabitModeUI = _modeUI;
        }

        public void UpdateSchedule(ScheduleSelectionUI _scheduleUI)
        {
            ScheduleUI = _scheduleUI;
        }

        public ModeData CreateMode(HabitModeSettingUI _setting, ModeData _originalData)
        {
            ModeData _data = _setting.GetModeData();

            switch (_data.Type)
            {
                case GameModeType.Alarm:
                    if (OGAlarm.gameObject.activeInHierarchy)
                    {
                        //Debug.Log("use og");
                        _data = _originalData;
                    }
                    break;
                case GameModeType.GoalTimer:
                    if (OGTimer.gameObject.activeInHierarchy)
                    {
                        _data = _originalData;
                    }
                    break;
                case GameModeType.CheckList:
                    break;
                case GameModeType.Quota:
                    break;
            }

            _data.Notify = OGNoti.gameObject.activeInHierarchy ? _originalData.Notify : _setting.GetNotifyData();

            return _data;
        }

        public ScheduleData CreateSchedule(ScheduleSelectionUI _setting, ScheduleData _originalData)
        {
            ScheduleData _data = null;

            switch (_originalData.Type)
            {
                case ScheduleType.Daily:
                    break;
                case ScheduleType.Weekly:
                    break;
                case ScheduleType.Monthly:
                    break;
                case ScheduleType.Repeatedly:
                    if (OGRepeated.gameObject.activeInHierarchy)
                    {
                        return _originalData;
                    }
                    break;
            }

            _data = _setting.GetScheduleData();

            return _data;
        }
    }
}
