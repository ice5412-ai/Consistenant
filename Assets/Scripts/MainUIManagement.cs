using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MPUIKIT;
using TMPro;
//using UnityEditor.Localization.Platform.Android;
using UnityEngine;
using Unity.Mathematics;

namespace Habillage
{
    public class MainUIManagement : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI moneyDisplay;
        [SerializeField] TextMeshProUGUI habitDisplay;
        [SerializeField] TextMeshProUGUI scoreDisplay;
        [SerializeField] GameObject RedAlert;
        [SerializeField] FloatingNumber floatingNumber;
        [SerializeField] float keptMoney;
        [SerializeField] float keptScore;
        [SerializeField] private List<GameObject> OtherWindows;
        
        private void Start()
        {
            keptMoney = PlayerData.Data.Inventory.Money;
            keptScore = PlayerData.Data.Inventory.TodayScore;
        }

        private void Update()
        {
            UpdateMoneyDisplayValue();
            UpdateHabitDisplayTotal();
            UpdateHabitTodayScore();
        }

        public void UpdateMoneyDisplayValue()
        {
            var _data = PlayerData.Data.Inventory;

            if (keptMoney != _data.Money)
            {
                if (OtherWindows.All(OtherWindow => !OtherWindow.activeSelf))
                {
                    float changedValue = _data.Money - keptMoney;
                    keptMoney = _data.Money;
                    SpawnFloatingNumber(changedValue);
                }
            }

            moneyDisplay.text = _data.Money.ToString();
        }

        public void UpdateHabitDisplayTotal()
        {
            // Done: show "[Total number of habit being done today] / [total number of Habit created by player]"

            var totalHabit = PlayerData.Data.CreatedHabits.Where(_t => _t.Value.ScheduleData.ValidToday()).Count();
            var _doneToday = PlayerData.Data.CreatedHabits.Where(_h =>
                            _h.Value.DaysData.ContainsKey(DateTime.Today.ToShortDateString())).Count(_h =>
                            _h.Value.DaysData[DateTime.Today.ToShortDateString()].ResultData.CompleteStat() ==
                            HabitCompletion.Succeed);

            habitDisplay.text = string.Format("{0}/{1}", _doneToday.ToString(), totalHabit.ToString());

            RedAlert.SetActive(_doneToday < totalHabit);
        }

        public void UpdateHabitTodayScore()
        {
            var _data = PlayerData.Data.Inventory;

            if (keptScore != _data.TodayScore)
            {
                if (OtherWindows.All(OtherWindow => !OtherWindow.activeSelf))
                {
                    float changedValue = _data.TodayScore - keptScore;
                    keptScore = _data.TodayScore;
                    SpawnFloatingNumber(changedValue);
                }
            }

            scoreDisplay.text = Mathf.Round(_data.TodayScore).ToString();
        }

        public void SpawnFloatingNumber(float _value)
        {
            if (_value == 0)
            {
                return;
            }
            Vector3 Destination = new Vector3(0, moneyDisplay.transform.lossyScale.y * 50, 0);
            //Debug.Log(CURRENT_TMP.transform.lossyScale.y);
            FloatingNumber newfloatingNumber = Instantiate(floatingNumber, moneyDisplay.rectTransform.position + (_value > 0 ? Destination : -Destination), quaternion.identity, moneyDisplay.rectTransform);
            newfloatingNumber.color = _value > 0 ? ColorPreset.Green : ColorPreset.Red;
            newfloatingNumber.tmp.text = _value > 0 ? "+" + MathF.Round(_value * 100) / 100 : (MathF.Round(_value * 100) / 100).ToString();
            newfloatingNumber.dir = _value > 0 ? Vector3.up * 75 : Vector3.down * 75;
            newfloatingNumber.AnimationTime = 1.5f;
            newfloatingNumber.DestroyTime = 3f;
        }
    }
}
