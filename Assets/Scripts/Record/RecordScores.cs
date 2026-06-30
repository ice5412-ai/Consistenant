using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Habillage
{
    public class RecordScores : MonoBehaviour
    {
        private float totalScore;
        int money = 0;

        public TextMeshProUGUI displayHabitsIncome;

        public void Awake()
        {
            totalScore = PlayerData.Data.Inventory.TodayScore;
            money = Mathf.RoundToInt(totalScore);
        }

        public void Update()
        {
            displayHabitsIncome.text = $"Habit's score will reward {money} coins on dailies reset.";
        }

        public void DailyReset()
        {
            ScoreToCoin();
        }

        public void ScoreToCoin()
        {
            money = Mathf.RoundToInt(totalScore);
            PlayerData.Data.Inventory.AddMoney(money);
            totalScore = 0;
            PlayerData.Data.Inventory.ChangeTodayScore(totalScore);
            money = Mathf.RoundToInt(totalScore);
            PlayerData.WriteSave();
        }

        public void ScoreAdd(float _score)
        {
            totalScore += _score;
            money = Mathf.RoundToInt(totalScore);
            PlayerData.Data.Inventory.ChangeTodayScore(totalScore);
            PlayerData.WriteSave();
            // Debug.Log("Score is now " + PlayerData.Data.Inventory.TodayScore);
        }
    }
}
