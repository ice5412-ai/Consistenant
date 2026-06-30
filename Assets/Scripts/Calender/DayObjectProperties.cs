using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using TMPro;
using MPUIKIT;
using UnityEngine.Playables;

namespace Habillage
{
    public class DayObjectProperties : MonoBehaviour
    {
        [SerializeField] bool _playable;
        [SerializeField] TMP_Text text;
        [SerializeField] MPImage image;
        public DateTime setDate;
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] public Transform particleHolds;

        public void SetDate(DateTime date, DateTime watchDate, Color color, HabitCompletion completion)
        {
            uiColor = color;
            setDate = new DateTime(date.Year, date.Month, date.Day);
            DateTime today = DateTime.Today; // If need to debug day, test it here
            text.text = date.Day.ToString();

            float H, S, V;
            Color.RGBToHSV(uiColor, out H, out S, out V);

            Color tintColor = Color.HSVToRGB(H, S / 5 * 2f, V / 5 * 2f); // repaired this watch month
            Color fadedColor = new Color(color.r, color.g, color.b, 0.2f); // repaired last watch month
            Color fadedBlack = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Color fadedWhite = new Color(1, 1, 1, 0.2f);
            Color fadedTint = new Color(tintColor.r, tintColor.g, tintColor.b, 0.2f);

            //Debug.Log(today);

            switch (DateTime.Compare(new DateTime(date.Year, date.Month, 1), new DateTime(watchDate.Year, watchDate.Month, 1)))
            {
                case < 0: // Before Watching Month
                    {
                        switch (DateTime.Compare(setDate, today))
                        {
                            case < 0: // Before Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedBlack, Color.clear, 0f, Color.clear, 0f, "1-U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedWhite, fadedColor, 0f, Color.clear, 0f, "1-S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedBlack, Color.clear, 0f, Color.clear, 0f, "1-F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedWhite, fadedTint, 0f, Color.clear, 0f, "1-R");
                                            break;*/
                                }
                                break;

                            case 0: // Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedWhite, fadedColor, 10f, Color.clear, 0f, "10U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedWhite, fadedColor, 0f, fadedWhite, 10f, "10S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedBlack, fadedColor, 10f, Color.clear, 0f, "10F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedWhite, fadedTint, 0f, fadedWhite, 10f, "10R");
                                            break;*/
                                }
                                break;

                            case > 0: // After Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "1+U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "1+S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "1+F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "1+R");
                                            break;*/
                                }
                                break;
                        }
                        break;
                    }
                case 0: // Watching Month
                    {
                        switch (DateTime.Compare(setDate, today))
                        {
                            case < 0: // Before Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(Color.gray, Color.clear, 0f, Color.clear, 0f, "2-U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(Color.white, color, 0f, Color.clear, 0f, "2-S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(Color.gray, Color.clear, 0f, Color.clear, 0f, "2-F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(Color.white, tintColor, 0f, Color.clear, 0f, "2-R");
                                            break;*/
                                }
                                break;

                            case 0: // Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(Color.white, color, 10f, Color.clear, 0f, "20U", true);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(Color.white, color, 0f, Color.white, 10f, "20S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(Color.gray, color, 10f, Color.clear, 0f, "20F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(Color.white, tintColor, 0f, Color.white, 10f, "20R");
                                            break;*/
                                }
                                break;

                            case > 0: // After Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(color, Color.clear, 0f, Color.clear, 0f, "2+U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(color, Color.clear, 0f, Color.clear, 0f, "2+S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(color, Color.clear, 0f, Color.clear, 0f, "2+F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(color, Color.clear, 0f, Color.clear, 0f, "2+R");
                                            break;*/
                                }
                                break;
                        }
                        break;
                    }
                case > 0: // After Watching Month
                    {
                        switch (DateTime.Compare(setDate, today))
                        {
                            case < 0: // Before Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedBlack, Color.clear, 0f, Color.clear, 0f, "3-U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedWhite, fadedColor, 0f, Color.clear, 0f, "3-S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedBlack, Color.clear, 0f, Color.clear, 0f, "3-F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedWhite, fadedTint, 0f, Color.clear, 0f, "3-R");
                                            break;*/
                                }
                                break;

                            case 0: // Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedWhite, fadedColor, 10f, Color.clear, 0f, "30U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedWhite, fadedColor, 0f, fadedWhite, 10f, "30S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedBlack, fadedColor, 10f, Color.clear, 0f, "30F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedWhite, fadedTint, 0f, fadedWhite, 10f, "30R");
                                            break;*/
                                }
                                break;

                            case > 0: // After Today
                                switch (completion)
                                {
                                    case HabitCompletion.Unfilled:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "3+U", false);
                                        break;

                                    case HabitCompletion.Succeed:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "3+S", false);
                                        break;

                                    case HabitCompletion.Failed:
                                        SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "3+F", false);
                                        break;

                                        /*case HabitCompletion.Repaired:
                                            SetColor(fadedColor, Color.clear, 0f, Color.clear, 0f, "3+R");
                                            break;*/
                                }
                                break;
                        }
                        break;
                    }
            }
        }

        public void SetColor(Color textColor, Color imageColor, float strokeWidth, Color outLineColor, float outlineWidth, string caseName, bool holdingParticle)
        {
            text.color = textColor;

            image.color = imageColor;
            image.StrokeWidth = strokeWidth;

            image.OutlineColor = outLineColor;
            image.OutlineWidth = outlineWidth;

            gameObject.name = caseName;

            var _data = PlayerData.Data.CurrentHabit;
            if (holdingParticle)
            {
                if (_data.ModeData is AlarmData _alarmData)
                {
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                    TimeSpan alarmTime = _alarmData.AlarmTime.ToTimeSpan();

                    // Calculate the difference between alarmTime and currentTime
                    double timeDifference = (currentTime - alarmTime).TotalMinutes;  // Use currentTime - alarmTime here
                    double nextDayDifference = (alarmTime + new TimeSpan(1, 0, 0, 0) - currentTime).TotalMinutes;

                    particleHolds.gameObject.SetActive(true);

                    if ((timeDifference < -30 && timeDifference > 10) || (nextDayDifference < -30 && nextDayDifference > 10))
                    {
                        particleHolds.gameObject.SetActive(false);
                    }
                    if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                    {
                        particleHolds.gameObject.SetActive(false);
                    }

                    //Debug.Log(DateTime.Now.TimeOfDay.TotalMinutes - _alarmData.AlarmTime.ToTimeSpan().TotalMinutes);
                }
                else
                {
                    particleHolds.gameObject.SetActive(true);
                }
            }
            else { particleHolds.gameObject.SetActive(false); _playable = false; }


        }

        private void OnMouseUpAsButton()
        {
            Debug.Log($"This Button is actually works and its name is '{gameObject.name}' !");
            Debug.Log("Today is " + setDate);
        }
    }
}
