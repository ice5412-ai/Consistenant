using System;
using System.Linq;
using JimmysUnityUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class HabitCreationUI : MonoBehaviour
    {
        public TMP_InputField TitleInputField;
        public HabitModeSettingUI HabitModeUI;
        public TMP_InputField DescriptionInputField;
        public ScheduleSelectionUI ScheduleUI;
        public ColorPresetEnum currentColor;
        public ColorPicker colorPicker;
        public Room CurrentRoom;
        public GameObject ErrorSaveWindow;
        public TextMeshProUGUI errorOutput;
        [SerializeField] private ScrollRect scrollRect;
        public GameModeSelection gameModeSelection;
        public SelectionBox scheduleSelectionBox;
        public ScheduleSelection scheduleSelection;
        public ScheduleWeeklySelectionUI scheduleWeeklySelectionUI;
        public ScheduleMonthlySelectionUI scheduleMonthlySelectionUI;
        [SerializeField] AndroidNotificationControl androidNotificationControl;
        private void OnEnable()
        {
            /*if (CurrentRoom = null)
            {
                gameObject.SetActive(false);
            }*/ // TODO
        }

        public void OpenHabitCreation(Room _room)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            currentColor = ColorPresetEnum.Turquoise;
            colorPicker.ColorPick((int)currentColor);
            CurrentRoom = _room;
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            CurrentRoom = null;
            TitleInputField.text = "";
            gameModeSelection.pagedRect.DefaultPage = 1;
            DescriptionInputField.text = "";
            scheduleSelectionBox.ForceSetInitialBeginWithTextNumber(0);
            scheduleSelection.UpdateSelectionMode();
            scheduleWeeklySelectionUI.ResetSelectedDays();
            scheduleMonthlySelectionUI.ResetSelectedDays();
            currentColor = ColorPresetEnum.Turquoise;
        }

        public void CreateHabit()
        {
            if (TitleInputField.text.IsNullOrWhiteSpace())
            {
                ErrorSaveWindow.SetActive(true);
                errorOutput.text = ($"Title is invalid, Please check it again!");
                return;
            }

            if (PlayerData.Data.CreatedHabits.ContainsKey(TitleInputField.text))
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

            var _newHabit = new HabitData
            {
                Title = TitleInputField.text == "" ? DateTime.Now.ToString() : TitleInputField.text,
                ModeData = CreateMode(HabitModeUI),
                Description = DescriptionInputField.text,
                ScheduleData = CreateSchedule(ScheduleUI),
                Color = currentColor,
                notificationId = PlayerData.Data.CreatedHabits.Count + 1,
                //Streak = new HabitStreakData()
            };

            PlayerData.Data.AddHabit(_newHabit);

            CurrentRoom.HabitTitle = _newHabit.Title;
            DormManager.Current?.Save();

            Debug.Log(_newHabit.SerializeData().ToString());
            PlayerData.WriteSave();

            if (_newHabit.ScheduleData.ValidToday())
            {
                var _notiTime = _newHabit.ModeData.Notify.ToTimeSpan();
                Debug.Log("_notiTime = " + _notiTime);
#if UNITY_ANDROID
                androidNotificationControl.SendNotification(_newHabit.Title, _newHabit.Description, DateTime.Today.Add(_notiTime), IconListEnum.small, IconListEnum.large, _newHabit.notificationId);
#endif
            }

            gameObject.SetActive(false);
        }

        public void UpdateMode(HabitModeSettingUI _modeUI)
        {
            HabitModeUI = _modeUI;
        }

        public void UpdateSchedule(ScheduleSelectionUI _scheduleUI)
        {
            ScheduleUI = _scheduleUI;
        }

        public void UpdateColor(ColorPresetEnum _color)
        {
            currentColor = colorPicker.selectedColor;
        }

        public ModeData CreateMode(HabitModeSettingUI _setting)
        {
            ModeData _data = _setting.GetModeData();

            return _data;
        }

        public ScheduleData CreateSchedule(ScheduleSelectionUI _setting)
        {
            ScheduleData _data = _setting.GetScheduleData();
            // switch (_setting.Type)
            // {
            //     case ScheduleType.Daily:
            //         _data = new Daily();
            //         break;
            //     case ScheduleType.Weekly:
            //         var _weeklyUI = (ScheduleWeeklySelectionUI)_setting;
            //         _data = new Weekly(_weeklyUI.DaysOfWeek);
            //         break;
            //     case ScheduleType.Monthly:
            //         var _monthlyUI = (ScheduleMonthlySelectionUI)_setting;
            //         _data = new Monthly(_monthlyUI.DaysOfMonth, _monthlyUI.LastDay);
            //         break;
            //     case ScheduleType.Repeatedly:
            //         var _repeatUI = (ScheduleRepeatedlySelectionUI)_setting;
            //         _data = new Repeatedly(_repeatUI.Days, DateTime.Today);
            //         break;
            // }

            return _data;
        }
    }
}