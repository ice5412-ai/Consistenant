using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Habillage
{
    public class ChartUI : MonoBehaviour
    {
        [SerializeField] private CalenderController calenderController;
        [SerializeField] private ChartObjectProperties chartObject;
        [SerializeField] private RectTransform content;
        //[SerializeField] private List<float> SampleValue;
        [SerializeField] public Color uiColor = Color.clear;

        private DateTime watchDate;
        void Start()
        {
            //UpdateChart(new HabitData());
        }
        // Done: Pull History to display on chart
        // Chart properties: 
        // watchdate
        // Chart Value - Alarm(When alarm is stopped compared to time's up), Timer(Total Time being recorded, Checklist(Total Task done, Quota(Value)))
        // For each chart will have states just like DayObjectProperties, set the information in it after generate each one

        public void UpdateChart()
        {
            UpdateChart(PlayerData.Data.CurrentHabit);
        }
        
        public void UpdateChart(HabitData _data)
        {
            foreach (Transform child in content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            
            //Debug.Log("updatechart");

            watchDate = calenderController.watchDate;
            

            var totalDayNumber = GetTotalNumberOfDays(watchDate.Year, watchDate.Month);
            
            var highestValue = _data.GetMaxValue();

            // foreach (var _dayData in _data.DaysData)
            // {
            //     Debug.Log(_dayData.Key);
            // }
            
            for (int i = 0; i < totalDayNumber; i++)
            {
                var temp = Instantiate(chartObject.gameObject, content.transform);
                ChartObjectProperties newChart = temp.GetComponent<ChartObjectProperties>();

                var _value = 0f;
                var _day = i + 1;
                var _dateTime = new DateTime(watchDate.Year, watchDate.Month, _day);
                if (_data.DaysData.TryGetValue(_dateTime.ToShortDateString(), out var _dayData))
                {
                    _value = _dayData.ResultData.GetValue();
                }
                // Done: Get "HabitCompletion" of each day to shown on this calender

                newChart.SetDate(watchDate.Year, watchDate.Month, _day, highestValue, _value, uiColor, _data.GetCompletion(_dateTime));
            }
        }

        int GetTotalNumberOfDays(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }
    }
}
