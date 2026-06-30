using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Habillage
{
    public class ScheduleWeeklySelectionUI : ScheduleSelectionUI
    {
        public List<DayOfWeek> DaysOfWeek;
        public SerializedDictionary<DayOfWeek, ToggleButton> DaysButton = new();
        public Color uiColor = Color.clear;

        private void Start()
        {
            foreach (var _kvp in DaysButton)
            {
                _kvp.Value.OnToggleChanged.AddListener(() => ToggleDay(_kvp.Key));
            }
        }

        private void OnEnable()
        {
            //SyncSelectedDays(new List<DayOfWeek>());
        }

        public void ResetSelectedDays()
        {
            foreach (var _button in DaysButton.Values)
            {
                _button.isToggled = false;
            }
        }

        public void SyncSelectedDays(List<DayOfWeek> _days)
        {
            Debug.Log("sync");
            DaysOfWeek = new List<DayOfWeek>(_days);

            foreach (var _button in DaysButton.Values)
            {
                _button.isToggled = false;
            }

            foreach (var _day in _days)
            {
                Debug.Log(_day.ToString());
                if (DaysButton.TryGetValue(_day, out var _button))
                {
                    _button.isToggled = true;
                }
            }
        }

        public void ToggleDay(DayOfWeek _day)
        {
            if (DaysOfWeek.Contains(_day))
            {
                RemoveDay(_day);
            }
            else
            {
                AddDay(_day);
            }
        }

        public void AddDay(DayOfWeek _day)
        {
            DaysOfWeek.Add(_day);
        }

        public void RemoveDay(DayOfWeek _day)
        {
            DaysOfWeek.Remove(_day);
        }

        public override ScheduleType Type => ScheduleType.Weekly;
        public override ScheduleData GetScheduleData()
        {
            return new Weekly(DaysOfWeek);
        }

        public override bool IsValid()
        {
            return DaysOfWeek.Any();
        }
    }
}