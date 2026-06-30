using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using TMPro;
using MPUIKIT;
using UnityEngine.UI;
using System.Diagnostics;
using System.Globalization;

namespace Habillage
{
    public class ChartObjectProperties : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] Slider slider;
        [SerializeField] MPImage image;
        [SerializeField] TextMeshProUGUI dayText;
        public DateTime setDate;

        public void SetDate(int year, int month, int day, float highestValue, float value, Color color, HabitCompletion completion)
        {
            //Debug.Log(day);
            setDate = new DateTime(year, month, day);

            valueText.text = value.ToString(CultureInfo.InvariantCulture);
            slider.value = Mathf.InverseLerp(0, highestValue, value);
            switch (completion)
            {
                case HabitCompletion.Unfilled:
                    image.color = color;
                    valueText.color = color;
                    image.StrokeWidth = 0f;
                    break;
                case HabitCompletion.Succeed:
                    image.color = color;
                    valueText.color = color;
                    image.StrokeWidth = 0f;
                    break;
                case HabitCompletion.Failed:
                    image.color = Color.black;
                    valueText.color = Color.white;
                    image.StrokeWidth = 10f;
                    break;
                /*case HabitCompletion.Repaired:
                    image.color = Color.black;
                    valueText.color = Color.white;
                    image.StrokeWidth = 0f;
                    break;*/
            }
            dayText.text = day.ToString();
            //Debug.Log(newDate);
        }
    }
}
