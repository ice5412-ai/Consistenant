using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Habillage
{
    public class HabitStreak : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI StreakValueText;
        [SerializeField] public TextMeshProUGUI BestValueText;
        [SerializeField] public TextMeshProUGUI TotalValueText;
        [SerializeField] public TextMeshProUGUI RecentValueText;
        public int Total;
        public int CurrentStreak;
        public DateTime RecentCompletion;
        public int BestStreak;
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] GameObject ParticleOnStreak;
        [SerializeField] GameObject ParticleBest;
        [SerializeField] TextMeshProUGUI StreakAnnoncementText;
        [SerializeField] ParticleSystem StreakAnnoncementPS_FX;

        void OnEnable()
        {
            UpdateStreak();
            var _habitData = PlayerData.Data.CurrentHabit;
            var _streak = _habitData.CalculateStreak();
            switch (_streak.CurrentStreak)
            {
                case 1:
                    StreakAnnoncementPS_FX.Play();
                    break;
                case 3:
                    StreakAnnoncementPS_FX.Play();
                    break;
                case 7:
                    StreakAnnoncementPS_FX.Play();
                    break;
                default: StreakAnnoncementText.gameObject.SetActive(false); break;
            }
        }

        private void Update()
        {
            UpdateStreak();
        }

        public void UpdateStreak()
        {
            // Done: Update Habit steak here
            var _habitData = PlayerData.Data.CurrentHabit;
            var _streak = _habitData.CalculateStreak();
            StreakValueText.text = _streak.CurrentStreak.ToString();
            BestValueText.text = _streak.BestStreak.ToString();
            TotalValueText.text = _streak.Total.ToString();
            RecentValueText.text = _streak.RecentCompletion.ToString(CultureInfo.InvariantCulture);

            StreakValueText.color = BestValueText.color = TotalValueText.color = RecentValueText.color = uiColor;

            ParticleOnStreak.SetActive(_streak.CurrentStreak >= _streak.BestStreak && _streak.CurrentStreak > 0);
            ParticleBest.SetActive(_streak.BestStreak > 0);

            switch (_streak.CurrentStreak)
            {
                case 1:
                    StreakAnnoncementText.text = "First Streak, Nice start!";
                    StreakAnnoncementText.gameObject.SetActive(true);
                    break;
                case 3:
                    StreakAnnoncementText.text = "3-Streaks, Keep Going!";
                    StreakAnnoncementText.gameObject.SetActive(true);
                    break;
                case 7:
                    StreakAnnoncementText.text = "7-Streaks, Excellent!";
                    StreakAnnoncementText.gameObject.SetActive(true);
                    break;
                default: StreakAnnoncementText.gameObject.SetActive(false); break;
            }
        }

        public void UpdateElementColor()
        {
            StreakValueText.color = uiColor;
            BestValueText.color = uiColor;
            TotalValueText.color = uiColor;
            RecentValueText.color = uiColor;
        }
    }
}
