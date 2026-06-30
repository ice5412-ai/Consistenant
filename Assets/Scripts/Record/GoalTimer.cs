using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;
using TMPro;
using System;
using Unity.VisualScripting;
using MoreMountains.Feedbacks;
using System.Linq;

namespace Habillage
{
    public class GoalTimer : BaseRecord
    {
        [Header("Timer UI references : ")]
        [SerializeField] private MPImage uiFillImage;
        [SerializeField] private TextMeshProUGUI uiShowText;
        // [SerializeField] private GameObject setClock;
        // [SerializeField] private ScrollMechanic setHours, setMinutes, setSeconds;
        private enum TimerState { idle, play, pause, }
        private TimerState timerState = TimerState.idle;
        [SerializeField] private Button StartTimerButton, PauseTimerButton, ResumeTimerButton, ReturnButton;
        [SerializeField] private TextMeshProUGUI uiStatusText;
        [SerializeField] private ParticleSystem ConfettiFX_PS;
        [SerializeField] public MMF_Player soundFX_confetti;
        [SerializeField] int DebugSecond;
        private bool overtime = false;
        public int duration { get; private set; } = 10;
        public int IdealTime = 10;
        private int remainingDuration = 10;
        private Coroutine UpdateTimerCoroutine = null;
        //private Coroutine LoadSceneCoroutine = null;
        public MMF_Player soundFX_clock;

        [SerializeField] RecordScores recordScores;
        [SerializeField] HabitStatsUI habitStatsUI;

        //private HabitData CurrentHabit;
        [SerializeField] TextMeshProUGUI ScoreText;
        public float ThisScore;
        public bool Bonus;

        [SerializeField] AndroidNotificationControl androidNotificationControl;
        [SerializeField] NotificationController notificationController;

        private void OnEnable()
        {
            StartTimerButton.onClick.AddListener(StartTimer);
            PauseTimerButton.onClick.AddListener(PauseTimer);
            ResumeTimerButton.onClick.AddListener(ResumeTimer);
            ReturnButton.onClick.AddListener(ReturnTimer);

            StartTimerButton.gameObject.SetActive(true);
            PauseTimerButton.gameObject.SetActive(false);
            ResumeTimerButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(true);

            // Debug
            // if (DebugSecond > 0)
            // {
            //     SetTimer(new DurationData
            //     {
            //         Hours = 0,
            //         Minute = 0,
            //         Second = DebugSecond
            //     });
            // }

            SetTimer();

            if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
            {
                if (_dayData.ResultData is GoalTimerResultData _resultData)
                {
                    UpdateUI(_resultData.GetRemainingDuration());
                }
            }
            else
            {
                if (PlayerData.Data.CurrentHabit.ModeData is GoalTimerData _timerData)
                {
                    UpdateUI(_timerData.IdealTime.GetSeconds());
                }
            }
        }

        private void OnDisable()
        {
            StartTimerButton.onClick.RemoveListener(StartTimer);
            PauseTimerButton.onClick.RemoveListener(PauseTimer);
            ResumeTimerButton.onClick.RemoveListener(ResumeTimer);
            ReturnButton.onClick.RemoveListener(ReturnTimer);

            //CurrentHabit = null;
        }

        public void SetTimer(DurationData durationData)
        {
            overtime = false;
            duration = remainingDuration - durationData.GetSeconds();
            IdealTime = durationData.GetSeconds();
            Bonus = IdealTime > 1800;
            uiStatusText.text = "Let's beat the clock!";

            // Score
            if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
            {
                ThisScore = _dayData.Score;
                ScoreText.text = $"+{Mathf.Round(ThisScore)}";
            }
            else
            {
                ThisScore = 0;
                ScoreText.text = $"+{Mathf.Round(ThisScore)}";
            }
        }

        public void SetTimer()
        {
            if (PlayerData.Data.CurrentHabit?.ModeData is GoalTimerData _timerData)
            {
                if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Now.ToShortDateString(), out var _dayData))
                {
                    if (_dayData.ResultData is GoalTimerResultData _timerResult)
                    {
                        remainingDuration = _timerResult.GetRemainingDuration();
                    }
                }
                else
                {
                    remainingDuration = _timerData.IdealTime.GetSeconds();
                }

                var _duration = _timerData.IdealTime;
                SetTimer(_duration);
            }
        }

        public override void SaveData(bool _complete = false)
        {
            if (PlayerData.Data.CurrentHabit == null) return;

            var _remain = remainingDuration;
            var _result = new GoalTimerResultData(((GoalTimerData)PlayerData.Data.CurrentHabit.ModeData).IdealTime)
            {
                Completed = _complete,
                RemainingDuration = new DurationData { Second = _remain }
            };

            PlayerData.Data.CurrentHabit.DaysData[DateTime.Today.ToShortDateString()] = new DayData(_result) { Score = ThisScore };

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

            // Debug.Log(PlayerData.Data.CurrentHabit.SerializeData());
            // Return progress duration to Habit screen: (+)Remaining duration / (-)Overtime duration
            // Save current stats: Remaining duration / Overtime duration, Used Duration, Recorded Time. Stackable with old stats of same progression (Can view history)
            // Class History { DurationData RemainingDuration; DateTime RecordedTime; }
        }


        private void StartTimer()
        {
            timerState = TimerState.play;

            uiShowText.gameObject.SetActive(true);
            // setClock.SetActive(false);
            StartTimerButton.gameObject.SetActive(false);
            PauseTimerButton.gameObject.SetActive(true);
            ResumeTimerButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            UpdateTimerCoroutine = StartCoroutine(UpdateTimer());
        }

        private void PauseTimer()
        {
            timerState = TimerState.pause;

            uiShowText.gameObject.SetActive(true);
            // setClock.SetActive(false);
            StartTimerButton.gameObject.SetActive(false);
            PauseTimerButton.gameObject.SetActive(false);
            ResumeTimerButton.gameObject.SetActive(true);
            ReturnButton.gameObject.SetActive(true);
            MarkAsCompleted();
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            StopCoroutine(UpdateTimerCoroutine);
        }

        private void ResumeTimer()
        {
            timerState = TimerState.play;

            uiShowText.gameObject.SetActive(true);
            // setClock.SetActive(false);
            StartTimerButton.gameObject.SetActive(false);
            PauseTimerButton.gameObject.SetActive(true);
            ResumeTimerButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            UpdateTimerCoroutine = StartCoroutine(UpdateTimer());
        }

        private void ReturnTimer()
        {
            timerState = TimerState.idle;
            StopCoroutine(UpdateTimerCoroutine);
            UpdateUI(remainingDuration);

            uiShowText.gameObject.SetActive(true);
            StartTimerButton.gameObject.SetActive(false);
            PauseTimerButton.gameObject.SetActive(false);
            ResumeTimerButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            MarkAsCompleted();
            Back();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                if (timerState == TimerState.play)
                {
                    PauseTimer();
                    uiStatusText.text = "If you unfocus on the tenant, the timer will pause!";
                }
            }
        }

        public void Back()
        {
            habitStatsUI.gameObject.SetActive(true);
            Parent.SetActive(false);
            habitStatsUI.ReturnedToHabitStatsUI();
        }

        private IEnumerator UpdateTimer()
        {
            while (timerState == TimerState.play)
            {
                yield return new WaitForSeconds(1f);
                remainingDuration--;
                soundFX_clock.PlayFeedbacks();
                if (remainingDuration < 0 && overtime == false)
                {
                    overtime = true;
                    ConfettiFX_PS.Play();
                    soundFX_confetti.PlayFeedbacks();
                }
                if (!overtime)
                {
                    recordScores.ScoreAdd((50 * (Bonus ? 2 : 1) / (float)IdealTime) * 50);
                    ThisScore += (50 * (Bonus ? 2 : 1) / (float)IdealTime) * 50;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                    //Debug.Log($"{IdealTime} : {100 / (float)IdealTime}");
                }
                else if (overtime && -remainingDuration <= IdealTime)
                {
                    recordScores.ScoreAdd((10 * (Bonus ? 2 : 1) / (float)IdealTime) * 50);
                    ThisScore += (10 * (Bonus ? 2 : 1) / (float)IdealTime) * 50;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                    //Debug.Log($"{IdealTime} : {20 / (float)IdealTime}");
                }
                UpdateUI(remainingDuration);
            }
        }

        private void UpdateUI(int seconds)
        {
            int _second = seconds;
            if (overtime)
            {
                _second = -seconds;
                if (_second < IdealTime)
                {
                    uiFillImage.fillAmount = Mathf.InverseLerp(0, IdealTime, _second);
                }
                else
                {
                    uiFillImage.fillAmount = 1f;
                }
            }
            else
            {
                uiFillImage.fillAmount = Mathf.InverseLerp(0, IdealTime, _second);
            }
            uiShowText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", _second / 3600, _second / 60 % 60, _second % 60);

            if (remainingDuration == IdealTime)
            {
                uiStatusText.text = "Let's beat the clock!";
            }
            else if (remainingDuration > IdealTime * 3 / 4)
            {
                uiStatusText.text = "Keep Focus!";
            }
            else if (remainingDuration <= IdealTime * 3 / 4 && remainingDuration > IdealTime * 2 / 4)
            {
                uiStatusText.text = "Halfway to go!";
            }
            else if (remainingDuration <= IdealTime * 2 / 4 && remainingDuration > IdealTime * 1 / 4)
            {
                uiStatusText.text = "Almost there!";
            }
            else if (remainingDuration <= IdealTime * 1 / 4 && remainingDuration > 0)
            {
                uiStatusText.text = "Just a bit more!";
            }
            else if (remainingDuration == 0)
            {
                uiStatusText.text = "Congratulation!";
            }
            else if (overtime && -remainingDuration < IdealTime)
            {
                uiStatusText.text = "Congratulation!";
            }
            else if (overtime && -remainingDuration == IdealTime)
            {
                uiStatusText.text = "You acquired maximum points possible!";
                ConfettiFX_PS.Play();
                soundFX_confetti.PlayFeedbacks();
            }
            else
            {
                uiStatusText.text = "You acquired maximum points possible!";
            }
        }

        public override bool MarkAsCompleted()
        {
            bool isSuccessful = remainingDuration <= 0;
            SaveData(isSuccessful);
            return isSuccessful;
        }
    }
}
