using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Habillage;
using MPUIKIT;
using UnityEngine;

namespace Consistenant
{
    public class ColorGridChart : MonoBehaviour
    {
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] public MPImage border;
        [SerializeField] private List<GridChartObjectProperties> days;
        [SerializeField] public Room room;

        void OnEnable()
        {
            UpdateChart();
        }
        public void UpdateChart()
        {
            if (PlayerData.Data.CreatedHabits.TryGetValue(room.HabitTitle, out var _habit))
            {
                uiColor = _habit.Color.FromEnum();
                border.color = uiColor;
                for (int i = 0; i < days.Count; i++)
                {
                    int counterdate = days.Count - 1 - i;
                    days[i].SetDate(DateTime.Today.AddDays(-counterdate), uiColor, _habit.GetCompletion(DateTime.Today.AddDays(-counterdate)));
                }
            }
            else
            {
                for (int i = 0; i < days.Count; i++)
                {
                    int counterdate = days.Count - 1 - i;
                    days[i].SetDate(DateTime.Today.AddDays(-counterdate), Color.white, HabitCompletion.Unfilled);
                }
            }
        }
    }
}
