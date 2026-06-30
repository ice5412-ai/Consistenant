using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.FeedbacksForThirdParty;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Habillage
{
    public class HabitStatsUI : MonoBehaviour
    {
        [SerializeField] private List<MPImage> elements;
        [SerializeField] private TextMeshProUGUI TitleText;
        [SerializeField] private TextMeshProUGUI DescriptionText;
        [SerializeField] private CalenderController calenderController;
        [SerializeField] private ChartUI chartUI;
        [SerializeField] private HabitStreak habitStreak;
        [SerializeField] private HabitDataDisplayUI habitDataDisplayUI;
        [SerializeField] private ColorPresetEnum ColorPick;
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] private HabitEditUI habitEditUI;
        [SerializeField] private bool DebugModeData;
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI playButtonText;
        [SerializeField] private ParticleSystem playButton_FX;
        [SerializeField] private List<GameObject> playHabitClosedUI;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] TextMeshProUGUI ScoreText;

        public SerializedDictionary<GameModeType, GameObject> GameModesUI = new();

        // Done: Navigate from this window back to HabitEdit safely: EditHabit
        // Done: Make Close Window work safely

        // Add Listener that lead to Gameplay Scene [AlarmScreen, TimerScreen, Checklist, Quota]
        // make sure to navigate there with all Data those Scene need.
        public void PlayHabit()
        {
            // Done: if habit is alarm, if time is earlier than 30 minute before alarm ring >> show prompt saying that it is not yet time

            var _data = PlayerData.Data.CurrentHabit;

            if (!_data.ScheduleData.ValidToday())
            {
                return;
            }

            if (_data.ModeData is AlarmData _alarmData)
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                TimeSpan alarmTime = _alarmData.AlarmTime.ToTimeSpan();

                // Calculate the difference between alarmTime and currentTime
                double timeDifference = (currentTime - alarmTime).TotalMinutes;  // Use currentTime - alarmTime here
                double nextDayDifference = (alarmTime + new TimeSpan(1, 0, 0, 0) - currentTime).TotalMinutes;

                if ((timeDifference < -30 && timeDifference > 10) || (nextDayDifference < -30 && nextDayDifference > 10))
                {
                    return;
                }
            }

            gameObject.SetActive(false);
            foreach (var go in playHabitClosedUI)
            {
                go.SetActive(false);
            }

            //Open play habit ui by set active prefabs
            foreach (var _modeObject in GameModesUI)
            {
                _modeObject.Value.SetActive(_modeObject.Key == _data.ModeData.Type);
            }

            //Debug.Log($"Play {_data.Title}");
        }
        void OnEnable()
        {
            scrollRect.verticalNormalizedPosition = 1f;
            LoadHabitStats(PlayerData.Data.CurrentHabit);
        }

        public void ReturnedToHabitStatsUI()
        {
            gameObject.SetActive(true);
            foreach (var go in playHabitClosedUI)
            {
                go.SetActive(true);
            }
        }

        public void LoadHabitStats(HabitData _data)
        {
            TitleText.text = _data.Title;
            DescriptionText.text = _data.Description;
            uiColor = _data.Color.FromEnum();

            calenderController.uiColor = uiColor;
            calenderController.UpdateCalendar(DateTime.Now);

            if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayDataS))
            {
                var _score = _dayDataS.Score;
                ScoreText.text = $"+{Mathf.Round(_score)}";
            }
            else
            {
                ScoreText.text = "0";
            }

            chartUI.uiColor = uiColor;
            // Done: Add info to Chart
            chartUI.UpdateChart(_data);

            habitStreak.uiColor = uiColor;
            habitStreak.UpdateStreak();

            habitDataDisplayUI.uiColor = uiColor;

            //done: set setting
            habitDataDisplayUI.UpdateDisplay(_data.ModeData.Type,
                _data.ScheduleData.Type.ToString(),
                _data.ScheduleData.ToString(),
                _data.ModeData.ToString(),
                _data.ModeData.Notify.ToString(),
                _data.Color.ToString(),
                _data.Color.FromEnum());

            if (elements != null)
            {
                foreach (MPImage element in elements)
                {
                    element.color = uiColor;
                }
            }

            playButtonText.text = _data.ScheduleData.ValidToday() ? "Play Habit" : "Not available today";
            playButtonText.color = _data.ScheduleData.ValidToday() ? uiColor : Color.gray;
            playButton.interactable = _data.ScheduleData.ValidToday();

            if (_data.ModeData is AlarmData _alarmData)
            {
                //Debug.Log(DateTime.Now.TimeOfDay.TotalMinutes - _alarmData.AlarmTime.ToTimeSpan().TotalMinutes);

                if (_data.ScheduleData.ValidToday())
                {
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                    TimeSpan alarmTime = _alarmData.AlarmTime.ToTimeSpan();

                    // Calculate the difference between alarmTime and currentTime
                    double timeDifference = (currentTime - alarmTime).TotalMinutes;  // Use currentTime - alarmTime here
                    double nextDayDifference = (alarmTime + new TimeSpan(1, 0, 0, 0) - currentTime).TotalMinutes;

                    // Initialize the button state and text
                    playButton.interactable = false;
                    string buttonText = "Not available";
                    Color buttonTextColor = Color.white;

                    // Check if the current time is within the allowed window (30 minutes before to 10 minutes after the alarm time)
                    if (timeDifference >= -30 && timeDifference <= 10)
                    {
                        playButton.interactable = true;
                        buttonText = "Play Habit";
                        buttonTextColor = uiColor;
                    }
                    // Check if the current time is within the allowed window (30 minutes before to 10 minutes after the alarm time on the next day)
                    else if (nextDayDifference >= -30 && nextDayDifference <= 10)
                    {
                        playButton.interactable = true;
                        buttonText = "Play Habit";
                        buttonTextColor = uiColor;
                    }
                    // Check if the current time is more than 30 minutes before the alarm time
                    else if (timeDifference < -30)
                    {
                        buttonText = "Available soon";
                        buttonTextColor = Color.white;
                    }
                    // Check if the current time is more than 10 minutes after the alarm time
                    else if (timeDifference > 10)
                    {
                        // Check if the creation date is today or not
                        if (_data.CreationDate.Date != DateTime.Today.Date)
                        {
                            buttonText = "Time is up";
                            buttonTextColor = Color.gray;
                        }
                        else
                        {
                            buttonText = "Not available today";
                            buttonTextColor = Color.grey;
                        }
                    }
                    // Check if the current time is more than 30 minutes before the alarm time on the next day
                    else if (nextDayDifference < -30)
                    {
                        buttonText = "Available soon";
                        buttonTextColor = Color.white;
                    }
                    // Check if the current time is more than 10 minutes after the alarm time on the next day
                    else if (nextDayDifference > 10)
                    {
                        // Check if the creation date is today or not
                        if (_data.CreationDate.Date != DateTime.Today.Date)
                        {
                            buttonText = "Time is up";
                            buttonTextColor = Color.gray;
                        }
                        else
                        {
                            buttonText = "Not available today";
                            buttonTextColor = Color.grey;
                        }
                    }

                    // Update the button text
                    playButtonText.text = buttonText;
                    playButtonText.color = buttonTextColor;

                    //Debug.Log($"currentTime: {currentTime}  alarmTime: {alarmTime}  timeDifference: {timeDifference}  nextDayDifference: {nextDayDifference}");

                    //Can't play again if completed
                    if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                    {
                        playButton.interactable = !_dayData.ResultData.Completed;
                        playButtonText.text = "Completed";
                    }
                }
            }

            playButton_FX.gameObject.SetActive(playButton.interactable);
        }

        public void EditHabit()
        {
            gameObject.SetActive(false);
            habitEditUI.gameObject.SetActive(true);
            habitEditUI.LoadHabit();
        }
    }


}
