using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using System.Linq;
#if UNITY_ANDROID
using Unity.Notifications;
#endif

namespace Habillage
{
    public class Checklist : BaseRecord
    {
        [SerializeField] public TextMeshProUGUI headerText;
        [SerializeField] public CheckListObjectProperties TaskPrefab;
        [SerializeField] public List<CheckListObjectProperties> TaskObjects = new List<CheckListObjectProperties>();
        [SerializeField] public TextMeshProUGUI numberOfListDone;
        [SerializeField] public ParticleSystem ConfettiFX_PS;
        [SerializeField] public MMF_Player soundFX_confetti;
        [SerializeField] public Transform content;
        [SerializeField] private ContentSizeFitter root;
        [SerializeField] private Color uiColor = Color.clear;
        int totaltaskDone = 0;
        [SerializeField] RecordScores recordScores;
        float scorePerTask = 0;
        bool wasDone = false;
        [SerializeField] HabitStatsUI habitStatsUI;
        bool setUpDone = false;

        [SerializeField] TextMeshProUGUI ScoreText;
        public float ThisScore;

        [SerializeField] AndroidNotificationControl androidNotificationControl;
        [SerializeField] NotificationController notificationController;

        private void OnEnable()
        {
            setUpDone = false;
            //CheckListData TestData = new CheckListData();
            if (PlayerData.Data.CurrentHabit?.ModeData is CheckListData)
            {
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
                SetUpLists();
            }
            uiColor = PlayerData.Data.CurrentHabit.Color.FromEnum();
        }

        private void OnDisable()
        {
            foreach (var _taskObject in TaskObjects)
            {
                Destroy(_taskObject.gameObject);
            }

            TaskObjects.Clear();
        }

        public void SetUpLists()
        {
            var _data = PlayerData.Data.CurrentHabit;

            headerText.text = _data.Title;

            if (_data.ModeData is not CheckListData _checkListData) return;

            int numberInList = 0;
            foreach (string _task in _checkListData.Tasks)
            {
                var _newTask = Instantiate(TaskPrefab, content, false);
                _newTask.gameObject.SetActive(true);

                var _isDone = false;

                if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                {
                    if (_dayData.ResultData is CheckListResultData _checkListResult)
                    {
                        if (_checkListResult.FinishedTasks.Contains(_task))
                        {
                            _isDone = true;
                        }
                    }
                }

                _newTask.SetList(_isDone, _task, _data.Color.FromEnum(), this, numberInList); // Fix this one
                TaskObjects.Add(_newTask);
                RefreshContentSize();
                Canvas.ForceUpdateCanvases();
                numberInList++;
            }
            setUpDone = true;

            int temp = TaskObjects.Where(TaskObjects => TaskObjects.IsDone).Count();
            totaltaskDone = temp;

            wasDone = totaltaskDone == TaskObjects.Count ? true : false;
            scorePerTask = 80 / (float)TaskObjects.Count;
            numberOfListDone.text = $"Task Done {totaltaskDone}/{TaskObjects.Count}";
        }

        private void RefreshContentSize()
        {
            if (this.gameObject.activeSelf)
            {
                System.Collections.IEnumerator Routine()
                {
                    root.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    yield return null;
                    root.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
                this.StartCoroutine(Routine());
            }
            else
            {
                root.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        public void UpdateCondition(int _numberInList, bool _isDone)
        {
            if (setUpDone)
            {
                Debug.Log(setUpDone);
                recordScores.ScoreAdd((_isDone ? scorePerTask : -scorePerTask) * 50);
                ThisScore += (_isDone ? scorePerTask : -scorePerTask) * 50;
                ScoreText.text = $"+{Mathf.Round(ThisScore)}";

                if (wasDone && !_isDone)
                {
                    wasDone = false;
                    recordScores.ScoreAdd(-20 * 50);
                    ThisScore -= 20 * 50;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                }

                int temp = TaskObjects.Where(TaskObjects => TaskObjects.IsDone).Count();
                totaltaskDone = temp;

                Debug.Log(TaskObjects.Where(TaskObjects => TaskObjects.IsDone).Count());

                if (totaltaskDone >= TaskObjects.Count)
                {
                    ConfettiFX_PS.Play();
                    soundFX_confetti.PlayFeedbacks();
                    wasDone = true;
                    recordScores.ScoreAdd(20 * 50);
                    ThisScore += 20 * 50;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                    numberOfListDone.color = uiColor;
                }
                else if (totaltaskDone < TaskObjects.Count)
                {
                    numberOfListDone.color = Color.white;
                }

                numberOfListDone.text = $"Task Done {totaltaskDone}/{TaskObjects.Count}";
                MarkAsCompleted();
            }
        }

        public override bool MarkAsCompleted()
        {
            bool isSuccessful;

            isSuccessful = totaltaskDone >= Mathf.RoundToInt(TaskObjects.Count) ? true : false;

            SaveData(isSuccessful);
            return isSuccessful;
        }

        public void Back()
        {
            habitStatsUI.gameObject.SetActive(true);
            Parent.SetActive(false);
            habitStatsUI.ReturnedToHabitStatsUI();
        }

        public override void SaveData(bool _completed = false)
        {
            if (PlayerData.Data.CurrentHabit == null ||
                PlayerData.Data.CurrentHabit.ModeData is not CheckListData _checkListData) return;

            var _completedList = new List<string>();
            foreach (var _taskObject in TaskObjects)
            {
                if (_taskObject.IsDone)
                {
                    _completedList.Add(_taskObject.task);
                }
            }

            var _checklist = new CheckListResultData(_checkListData.Tasks)
            {
                FinishedTasks = _completedList,
                Completed = _completed
            };

            PlayerData.Data.CurrentHabit.DaysData[DateTime.Now.ToShortDateString()] = new DayData(_checklist) { Score = ThisScore };

            DormManager.Current?.Save();
            PlayerData.WriteSave();

#if UNITY_ANDROID
            if (PlayerData.Data.CurrentHabit.ScheduleData.ValidToday())
            {
                if (!_completed)
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
    }
}
