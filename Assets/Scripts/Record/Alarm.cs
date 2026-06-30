using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;
using TMPro;
using System;
using MoreMountains.Feedbacks;
using System.Linq;


namespace Habillage
{
    public class Alarm : BaseRecord
    {
        [Header("Timer UI references : ")]
        [SerializeField] private MPImage uiFillImage;
        [SerializeField] private TextMeshProUGUI uiAlarmMessage;
        [SerializeField] private TextMeshProUGUI uiShowText;
        // [SerializeField] private GameObject setClock;
        // [SerializeField] private ScrollMechanic setHours, setMinutes, setSeconds;
        [SerializeField] private Button StopAlarmButton, QuickQuitButton;
        [SerializeField] private TextMeshProUGUI uiStatusText;
        [SerializeField] private int TimesUpAfter;
        [SerializeField] private ParticleSystem ConfettiFX_PS;
        [SerializeField] private bool timesup = false;
        private bool IsFinished = false;
        //[SerializeField] private int DebugAddSeconds = 10;
        private DateTime StartDateTime;
        private DateTime TimesUpDateTime;
        private DateTime MarkedDateTime;
        //private int remainingDuration;

        private Vector3 accelarationDir;

        private float StopAlarmProgressActionGauge = 0f;

        private Coroutine UpdateTimerCoroutine = null;
        private Coroutine LoadSceneCoroutine = null;
        public MMF_Player soundFX_clock;
        public MMF_Player soundFX_confetti;

        [SerializeField] RecordScores recordScores;
        [SerializeField] HabitStatsUI habitStatsUI;

        [SerializeField] GameObject ScoreContainer;
        [SerializeField] TextMeshProUGUI ScoreText;

        [SerializeField] AndroidNotificationControl androidNotificationControl;
        [SerializeField] NotificationController notificationController;
        public float ThisScore;

        private void OnEnable()
        {
            ScoreContainer.SetActive(false);
            StopAlarmProgressActionGauge = 0f;
            timesup = false;
            IsFinished = false;
            
            StopAlarmButton.onClick.AddListener(StopAlarmAction);
            QuickQuitButton.onClick.AddListener(QuickQuit);

            if (PlayerData.Data.CurrentHabit?.ModeData is AlarmData _alarmData)
            {
                if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                {
                    ScoreContainer.SetActive(true);
                    ThisScore = _dayData.Score;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                }
                else
                {
                    ThisScore = 0;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                }

                var _alarmTime = _alarmData.AlarmTime.ToTimeSpan();
                //var _remaining = DateTime.Now - DateTime.Now.Add(_alarmTime);
                //Debug.Log(DateTime.Today.Add(_alarmTime));
                SetAlarm(PlayerData.Data.CurrentHabit.Title, DateTime.Today.Add(_alarmTime));
                StartAlarm();
            }

            // Debug
            // if (DebugAddSeconds > 0)
            // {
            //     
            //     SetAlarm("debug mode", DateTime.Now.AddSeconds(DebugAddSeconds));
            //     StartAlarm();
            // }
            UpdateUI();
            UpdateFillUI();
        }
        void Update()
        {
            if (DateTime.Now < TimesUpDateTime && !IsFinished)
            {
                accelarationDir = Input.acceleration;

                if (accelarationDir.sqrMagnitude >= 5f)
                {
                    StopAlarmProgressActionGauge += Mathf.RoundToInt(accelarationDir.sqrMagnitude);
                    StopAlarmProgressActionGauge = StopAlarmProgressActionGauge > 100 ? 100 : StopAlarmProgressActionGauge;
                }
                if (StopAlarmProgressActionGauge >= 100)
                {
                    StopAlarmProgressActionGauge = 100;
                    timesup = false;
                    Debug.Log("Disable Alarm with in time limit.");
                    StopAlarm();
                }
                else if (StopAlarmProgressActionGauge > 0)
                {
                    StopAlarmProgressActionGauge -= Time.deltaTime * 20;
                    StopAlarmProgressActionGauge = StopAlarmProgressActionGauge < 0 ? 0 : StopAlarmProgressActionGauge;
                }
            }
            UpdateFillUI();
        }

        public void SetAlarm(string message, DateTime alarmDateTime)
        {
            uiFillImage.fillAmount = 0f;
            timesup = false;
            uiAlarmMessage.text = message;
            StartDateTime = alarmDateTime;
            TimesUpDateTime = alarmDateTime.AddSeconds(TimesUpAfter);
            Debug.Log($"start {StartDateTime} up{TimesUpDateTime}");
        }

        public override void SaveData(bool _complete = false)
        {
            // Done: Return progress duration to Habit screen: (+)Remaining duration / (-)Overtime duration
            // Save current stats: Remaining duration / Overtime duration, Used Duration, Recorded Time. Stackable with old stats of same progression (Can view history)
            // Class History { DurationData RemainingDuration; DateTime RecordedTime; }

            if (PlayerData.Data.CurrentHabit == null ||
                PlayerData.Data.CurrentHabit.ModeData.Type != GameModeType.Alarm) return;

            var _result = new AlarmResultData
            {
                StartDateTime = StartDateTime,
                MarkedDateTime = DateTime.Now,
                TimesUpDateTime = TimesUpDateTime,
                Completed = _complete
            };

            PlayerData.Data.CurrentHabit.AddDayData(DateTime.Now, new DayData(_result) { Score = ThisScore });

            DormManager.Current?.Save();
            PlayerData.WriteSave();

#if UNITY_ANDROID
            if (PlayerData.Data.CurrentHabit.ScheduleData.ValidToday())
            {
                if (!_complete)
                {
                    var _notiTime = PlayerData.Data.CurrentHabit.ModeData.Notify.ToTimeSpan();
                    androidNotificationControl.SendNotification(PlayerData.Data.CurrentHabit.Title, PlayerData.Data.CurrentHabit.Description, DateTime.Today.Add(_notiTime), IconListEnum.small, IconListEnum.large, PlayerData.Data.CurrentHabit.notificationId);
                }
                else
                {
                    androidNotificationControl.CancelNotification(PlayerData.Data.CurrentHabit.notificationId);
                }
            }

            notificationController.DailyResetNotification();
#endif
        }

        public void StopAlarmAction()
        {
            if (DateTime.Now < TimesUpDateTime && !IsFinished)
            {
                int random = UnityEngine.Random.Range(5, 10);
                StopAlarmProgressActionGauge += random;
                StopAlarmProgressActionGauge = StopAlarmProgressActionGauge > 100 ? 100 : StopAlarmProgressActionGauge;
                UpdateFillUI();
            }
        }

        public void StartAlarm()
        {
            StopAlarmButton.gameObject.SetActive(true);
            QuickQuitButton.gameObject.SetActive(false);
            UpdateTimerCoroutine = StartCoroutine(UpdateTimer());
        }

        private void StopAlarm()
        {
            IsFinished = true;
            if (UpdateTimerCoroutine != null)
            {
                StopCoroutine(UpdateTimerCoroutine);
            }
            UpdateUI();
            StopAlarmButton.gameObject.SetActive(false);

            if (!timesup)
            {
                ConfettiFX_PS.Play();
                soundFX_confetti.PlayFeedbacks();
                uiStatusText.text = string.Format("Well done!");
                ThisScore = (((float)(GetAlarmTimeRemaining() / GetAlarmFullDuration()) * 20) + 100) * 50;
                recordScores.ScoreAdd(ThisScore);
            }
            else
            {
                uiStatusText.text = string.Format("Time's up. That's too bad!");
            }

            MarkAsCompleted();

            LoadSceneCoroutine = StartCoroutine(ShowButtonAfterSecond(3));
        }


        public override bool MarkAsCompleted()
        {
            bool isSuccessful = !timesup;
            SaveData(isSuccessful);

            // Score
            if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
            {
                ScoreContainer.SetActive(true);
                ThisScore = _dayData.Score;
                ScoreText.text = $"+{Mathf.Round(ThisScore)}";
            }
            else
            {
                ScoreText.text = $"+{Mathf.Round(ThisScore)}";
            }

            return isSuccessful;
        }

        private IEnumerator ShowButtonAfterSecond(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            QuickQuitButton.gameObject.SetActive(true);
        }

        public void QuickQuit()
        {
            habitStatsUI.gameObject.SetActive(true);
            Parent.SetActive(false);
            habitStatsUI.ReturnedToHabitStatsUI();
        }


        private IEnumerator UpdateTimer()
        {
            while (DateTime.Now < TimesUpDateTime)
            {
                MarkedDateTime = DateTime.Now;
                UpdateUI();
                yield return new WaitForSeconds(1f);
                soundFX_clock.PlayFeedbacks();
                if (DateTime.Now >= TimesUpDateTime)
                {
                    timesup = true;
                    Debug.Log("Failed to disable alarm within time limit.");
                    UpdateUI();
                    StopAlarm();
                }
            }
        }

        private void UpdateUI()
        {
            uiShowText.text = $"{MarkedDateTime.Hour:D2}:{MarkedDateTime.Minute:D2}:{MarkedDateTime.Second:D2}";
            uiStatusText.text = $"{GetAlarmTimeRemaining()} seconds until Time's up!";

            if (timesup)
            {
                uiStatusText.text = string.Format("Time's up. That's too bad!");
            }
        }

        private void UpdateFillUI()
        {
            uiFillImage.fillAmount = Mathf.InverseLerp(0, 1, StopAlarmProgressActionGauge / 100);
        }

        private void OnDisable()
        {
            StopAlarmButton.onClick.RemoveListener(StopAlarm);
            QuickQuitButton.onClick.AddListener(QuickQuit);
        }

        public int GetAlarmTimeRemaining()
        {
            // Calculate the difference between the two DateTimes
            TimeSpan difference = TimesUpDateTime - MarkedDateTime;

            // Check if the difference is negative, indicating the timespan crosses over midnight
            if (difference.TotalSeconds < 0)
            {
                // Add one day (24 hours) to the TimesUpDateTime and recalculate the difference
                difference = TimesUpDateTime.AddDays(1) - MarkedDateTime;
            }

            // Get the total seconds from the TimeSpan
            int alarmTimeRemaining = (int)difference.TotalSeconds;

            // Log the details for debugging
            Debug.Log("TimesUpDateTime: " + TimesUpDateTime);
            Debug.Log("MarkedDateTime: " + MarkedDateTime);
            Debug.Log("Difference: " + difference);
            Debug.Log("Total Seconds: " + alarmTimeRemaining);

            return alarmTimeRemaining;
        }


        public int GetAlarmFullDuration()
        {
            // Calculate the difference between the two DateTimes
            TimeSpan difference = TimesUpDateTime - StartDateTime;

            // Check if the difference is negative, indicating the timespan crosses over midnight
            if (difference.TotalSeconds < 0)
            {
                // Add one day (24 hours) to the TimesUpDateTime and recalculate the difference
                difference = TimesUpDateTime.AddDays(1) - StartDateTime;
            }

            // Get the total seconds from the TimeSpan
            int alarmFullDuration = (int)difference.TotalSeconds;

            // Log the details for debugging
            Debug.Log("TimesUpDateTime: " + TimesUpDateTime);
            Debug.Log("StartDateTime: " + StartDateTime);
            Debug.Log("Difference: " + difference);
            Debug.Log("Total Seconds: " + alarmFullDuration);

            return alarmFullDuration;
        }
    }
}
