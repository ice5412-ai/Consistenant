using System;
using System.Collections;
using System.Collections.Generic;
//using AYellowpaper.SerializedCollections.Editor.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class CalenderController : MonoBehaviour
    {
        [SerializeField] private Transform[] weeks;

        // --- month ---
        [SerializeField] private Button previousMonthButton;
        [SerializeField] private Button nextMonthButton;
        [SerializeField] private TextMeshProUGUI selectionMonthText;
        [SerializeField][Range(0, 12)] public int BeginningMonth = 0;
        [SerializeField] private List<string> MonthList = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        [SerializeField] public CalenderEvent OnMonthChangePreviousEvent = new CalenderEvent();
        [SerializeField] public CalenderEvent OnMonthChangeNextEvent = new CalenderEvent();
        [SerializeField] public CalenderEvent OnMonthChangeEvent = new CalenderEvent();

        // --- year ---
        [SerializeField] private Button TextMode;
        [SerializeField] private Button SelectionMode;
        [SerializeField] private GameObject SelectionContainer;
        [SerializeField] private bool IsToggled;
        [SerializeField] private Button previousYearButton;
        [SerializeField] private Button nextYearButton;
        [SerializeField] private TextMeshProUGUI textmodeText;
        [SerializeField] private TextMeshProUGUI selectionYearText;
        [SerializeField][Range(0, 9999)] public int BeginningYear = 0;
        [SerializeField] public CalenderEvent OnYearChangePreviousEvent = new CalenderEvent();
        [SerializeField] public CalenderEvent OnYearChangeNextEvent = new CalenderEvent();

        [SerializeField] public CalenderEvent OnYearChangeEvent = new CalenderEvent();

        [SerializeField] public Color uiColor = Color.clear;

        // ---
        [Serializable] public class CalenderEvent : UnityEngine.Events.UnityEvent { }

        public DateTime watchDate { get; private set; } = DateTime.Now;
        //private int sendYear, sendMonth, sendDate;

        [SerializeField] public ChartUI chartUI;

        [SerializeField] private List<TextMeshProUGUI> textElements;

        // done: Make each "DayObjectProperties" which is childObject recall success state (Succeed, Failed, Unknown, Repaired, OtherMonth)


        void Start()
        {
            //Debug.Log("start");
            //watchDate = DateTime.Now;
            /*if (BeginningMonth > 0 && BeginningMonth <= 12)
            {
                watchDate = new DateTime(watchDate.Year, BeginningMonth, watchDate.Day);
            }
            if (BeginningYear > 0 && BeginningYear <= 9999)
            {
                watchDate = new DateTime(BeginningYear, watchDate.Month, watchDate.Day);
            }*/
            watchDate = DateTime.Now;
            if (BeginningMonth > 0 && BeginningMonth <= 12)
            {
                watchDate = new DateTime(watchDate.Year, BeginningMonth, watchDate.Day);
            }
            if (BeginningYear > 0 && BeginningYear <= 9999)
            {
                watchDate = new DateTime(BeginningYear, watchDate.Month, watchDate.Day);
            }
            
            // if (BeginningMonth > 0 && BeginningMonth <= 12)
            // {
            //     watchDate = new DateTime(watchDate.Year, BeginningMonth, watchDate.Day);
            // }
            // if (BeginningYear > 0 && BeginningYear <= 9999)
            // {
            //     watchDate = new DateTime(BeginningYear, watchDate.Month, watchDate.Day);
            // }
            UpdateMonth(watchDate.Month);
            UpdateYear(watchDate.Year);
            UpdateCalendar(watchDate);
        }

        private void OnEnable()
        {
            previousMonthButton.onClick.AddListener(ChangeMonthPrevious);
            nextMonthButton.onClick.AddListener(ChangeMonthNext);
            TextMode.onClick.AddListener(ToggleMode);
            SelectionMode.onClick.AddListener(ToggleMode);
            previousYearButton.onClick.AddListener(ChangeYearPrevious);
            nextYearButton.onClick.AddListener(ChangeYearNext);
        }
        private void OnDisable()
        {
            previousMonthButton.onClick.RemoveListener(ChangeMonthPrevious);
            nextMonthButton.onClick.RemoveListener(ChangeMonthNext);
            TextMode.onClick.RemoveListener(ToggleMode);
            SelectionMode.onClick.RemoveListener(ToggleMode);
            previousYearButton.onClick.RemoveListener(ChangeYearPrevious);
            nextYearButton.onClick.RemoveListener(ChangeYearNext);
        }

        private void ToggleMode()
        {
            IsToggled = !IsToggled;
            TextMode.gameObject.SetActive(!IsToggled);
            SelectionContainer.SetActive(IsToggled);
        }

        private void ChangeMonthPrevious()
        {
            watchDate = watchDate.AddMonths(-1);

            if (OnMonthChangePreviousEvent != null)
            {
                OnMonthChangePreviousEvent.Invoke();
            }

            if (OnMonthChangeEvent != null)
            {
                OnMonthChangeEvent.Invoke();
            }

            // Done: Script used to be activate via button but now since it required "HabitData" now it not called.
            if (chartUI != null)
            {
                chartUI.UpdateChart();
            }

            UpdateMonth(watchDate.Month);
            UpdateYear(watchDate.Year);
            UpdateCalendar(watchDate);
        }

        private void ChangeMonthNext()
        {
            watchDate = watchDate.AddMonths(+1);

            if (OnMonthChangeNextEvent != null)
            {
                OnMonthChangeNextEvent.Invoke();
            }

            if (OnMonthChangeEvent != null)
            {
                OnMonthChangeEvent.Invoke();
            }

            // Done: Script used to be activate via button but now since it required "HabitData" now it not called.
            if (chartUI != null)
            {
                chartUI.UpdateChart();
            }

            UpdateMonth(watchDate.Month);
            UpdateYear(watchDate.Year);
            UpdateCalendar(watchDate);
        }

        private void ChangeYearPrevious()
        {
            if (watchDate.Year <= 1)
            {
                Debug.Log("Attemping to go lower year 1, return nothing");
                return;
            }

            watchDate = watchDate.AddYears(-1);

            if (OnYearChangePreviousEvent != null)
            {
                OnYearChangePreviousEvent.Invoke();
            }

            if (OnYearChangeEvent != null)
            {
                OnYearChangeEvent.Invoke();
            }

            // Done: Script used to be activate via button but now since it required "HabitData" now it not called.
            if (chartUI != null)
            {
                chartUI.UpdateChart();
            }

            UpdateYear(watchDate.Year);
            UpdateCalendar(watchDate);
        }

        private void ChangeYearNext()
        {
            if (watchDate.Year >= 9999)
            {
                Debug.Log("Attemping to go surpass year 9999, return nothing");
                return;
            }

            watchDate = watchDate.AddYears(+1);

            if (OnYearChangeNextEvent != null)
            {
                OnYearChangeNextEvent.Invoke();
            }

            if (OnYearChangeEvent != null)
            {
                OnYearChangeEvent.Invoke();
            }

            // Done: Script used to be activate via button but now since it required "HabitData" now it not called.
            if (chartUI != null)
            {
                chartUI.UpdateChart();
            }

            UpdateYear(watchDate.Year);
            UpdateCalendar(watchDate);
        }

        private void UpdateMonth(int month)
        {
            selectionMonthText.text = MonthList[month - 1];
        }

        private void UpdateYear(int year)
        {
            textmodeText.text = string.Format("{0:0000}", year);
            selectionYearText.text = string.Format("{0:0000}", year);
        }

        public void UpdateCalendar(DateTime dateTime)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            int endDayOfLastmonth = GetTotalNumberOfDaysOfLastMonth(year, month);
            int startDay = GetMonthStartDay(year, month);
            int endDay = GetTotalNumberOfDays(year, month);

            UpdateElementColor();

            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    DateTime sendDateTime = dateTime;
                    int currentDay = (w * 7) + i;
                    if (currentDay < startDay)
                    {
                        sendDateTime = sendDateTime.AddMonths(-1);
                        sendDateTime = new DateTime(sendDateTime.Year, sendDateTime.Month, endDayOfLastmonth + currentDay - startDay + 1);
                        //Debug.Log(string.Format("yy{0},mm{1},dd{2},now{0} Case 1", sendYear, sendMonth, sendDate, thisMonth));
                    }
                    else if (currentDay >= startDay && currentDay - startDay < endDay)
                    {
                        sendDateTime = new DateTime(sendDateTime.Year, sendDateTime.Month, currentDay - startDay + 1);
                        //Debug.Log(string.Format("yy{0},mm{1},dd{2},now{0} Case 2", sendYear, sendMonth, sendDate, thisMonth));
                    }
                    else if (currentDay - startDay >= endDay)
                    {
                        sendDateTime = sendDateTime.AddMonths(1);
                        sendDateTime = new DateTime(sendDateTime.Year, sendDateTime.Month, currentDay - startDay + 1 - endDay);
                        //Debug.Log(string.Format("yy{0},mm{1},dd{2},now{0} Case 3", sendYear, sendMonth, sendDate, thisMonth));
                    }
                    else
                    {
                        sendDateTime = DateTime.Now;
                        //Debug.Log(string.Format("yy{0},mm{1},dd{2},now{0} Wrong Case", sendYear, sendMonth, sendDate, thisMonth));
                    }

                    // #region 
                    // // Done: Get "HabitCompletion" of each day to shown on this calender
                    // // Debug
                    // HabitCompletion debugCompletion = HabitCompletion.Unfilled;
                    //
                    // if (sendDateTime > DateTime.Today) // later than today
                    // {
                    //     debugCompletion = 0;
                    //     //Debug.Log(sendDateTime + " | dateTime > DateTime.Today | " + debugCompletion);
                    // }
                    // else if (sendDateTime == DateTime.Today)
                    // {
                    //     debugCompletion = 0;
                    //     //debugCompletion = (HabitCompletion)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(HabitCompletion)).Length);
                    //     //Debug.Log(sendDateTime + " | dateTime == DateTime.Today | " + debugCompletion);
                    // }
                    // else if (sendDateTime < DateTime.Today) // before than today
                    // {
                    //     debugCompletion = (HabitCompletion)UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(HabitCompletion)).Length);
                    //     //Debug.Log(sendDateTime + " | ateTime <= DateTime.Today | " + debugCompletion);
                    // }
                    // // End of Debug
                    // #endregion


                    weeks[w].GetChild(i).GetComponent<DayObjectProperties>().SetDate(sendDateTime, watchDate, uiColor, PlayerData.Data.CurrentHabit.GetCompletion(sendDateTime));
                }
            }
        }


        int GetTotalNumberOfDaysOfLastMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month - 1 <= 0 ? 12 : month - 1);
        }

        int GetMonthStartDay(int year, int month)
        {
            DateTime temp = new DateTime(year, month, 1);
            return (int)temp.DayOfWeek;
        }

        int GetTotalNumberOfDays(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public void UpdateElementColor()
        {
            if (textElements != null)
            {
                foreach (TextMeshProUGUI element in textElements)
                {
                    element.color = uiColor;
                }
            }
        }

        /*public void ChangeMonth()
        {
            int tempMonth = selectionBox.CurrentTextNumber() + 1;
            if (tempMonth == 1)
            {
                toggleNumericSelectionBox.ForceChangeNumber(-1);
            }
            if (tempMonth == 12)
            {
                toggleNumericSelectionBox.ForceChangeNumber(+1);
            }
            int tempYear = toggleNumericSelectionBox.CurrentTextNumber();

            Debug.Log(String.Format("Change Month : {0}, {1}", tempYear, tempMonth));
            watchDate = new DateTime(tempYear, tempMonth, 1);
            UpdateCalendar(watchDate.Year, watchDate.Month);
        }

        public void ChangeYear()
        {
            int temp = toggleNumericSelectionBox.CurrentTextNumber();
            Debug.Log(String.Format("Change Month : {0}, {1}", temp, watchDate.Month));
            watchDate = new DateTime(temp, watchDate.Month, watchDate.Day);
            UpdateCalendar(watchDate.Year, watchDate.Month);
        }*/
    }
}
