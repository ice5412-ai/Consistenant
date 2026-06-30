using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Habillage
{
    public class ScheduleMonthlySelectionUI : ScheduleSelectionUI
    {
        public List<int> DaysOfMonth;
        public bool LastDay;
        public List<ToggleButton> DaysButton = new();
        public ToggleButton LastDayButton;
        public Color uiColor = Color.clear;

        private void Start()
        {
            for (var _index = 0; _index < DaysButton.Count; _index++)
            {
                var _button = DaysButton[_index];
                var _day = _index + 1;
                _button.OnToggleChanged.AddListener(() => ToggleDay(_day));
            }

            LastDayButton.OnToggleChanged.AddListener((() =>
            {
                LastDay = !LastDay;
            }));
        }

        private void OnEnable()
        {
            //SyncSelectedDays(new List<int>(), false);
        }

        public void ResetSelectedDays()
        {
            for (var _index = 0; _index < DaysButton.Count; _index++)
            {
                var _button = DaysButton[_index];
                _button.isToggled = false;
            }
        }

        public void SyncSelectedDays(List<int> _days, bool _lastDay)
        {
            DaysOfMonth = new List<int>(_days);
            LastDay = _lastDay;
            LastDayButton.isToggled = _lastDay;

            for (var _index = 0; _index < DaysButton.Count; _index++)
            {
                var _button = DaysButton[_index];
                _button.isToggled = _days.Contains(_index);
            }
        }

        public void ToggleDay(int _day)
        {
            if (DaysOfMonth.Contains(_day))
            {
                RemoveDay(_day);
            }
            else
            {
                AddDay(_day);
            }
        }

        public void AddDay(int _day)
        {
            DaysOfMonth.Add(_day);
        }

        public void RemoveDay(int _day)
        {
            DaysOfMonth.Remove(_day);
        }

        public override ScheduleType Type => ScheduleType.Monthly;
        public override ScheduleData GetScheduleData()
        {
            return new Monthly(DaysOfMonth, LastDay);
        }

        public override bool IsValid()
        {
            return DaysOfMonth.Any() || LastDay;
        }
    }
}