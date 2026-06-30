using System;
using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using TMPro;
using UnityEngine;

namespace Consistenant
{
    public class GridChartObjectProperties : MonoBehaviour
    {
        [SerializeField] MPImage image;
        public DateTime setDate;
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] TMP_Text text;
        public void SetDate(DateTime _date, Color color, HabitCompletion completion)
        {
            uiColor = color;
            setDate = new DateTime(_date.Year, _date.Month, _date.Day);
            DateTime today = DateTime.Today; // If need to debug day, test it here
            text.text = _date.Day.ToString();

            float H, S, V;
            Color.RGBToHSV(uiColor, out H, out S, out V);

            switch (completion)
            {
                case HabitCompletion.Unfilled:
                    if (setDate == today)
                    {
                        image.color = new Color(uiColor.r, uiColor.g, uiColor.b, 0.9f);
                        image.StrokeWidth = 0.1f;
                        text.text = _date.Day.ToString();
                        text.color = Color.white;
                    }
                    else
                    {
                        image.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                        image.StrokeWidth = 0f;
                        text.text = _date.Day.ToString();
                        text.color = Color.black;
                    }
                    break;

                case HabitCompletion.Succeed:
                    image.color = new Color(uiColor.r, uiColor.g, uiColor.b, 0.9f);
                    image.StrokeWidth = 0f;
                    text.text = _date.Day.ToString();
                    text.color = Color.white;
                    break;

                case HabitCompletion.Failed:
                    image.color = new Color(.2f, .2f, .2f, 0.9f);
                    image.StrokeWidth = 0f;
                    text.text = _date.Day.ToString();
                    text.color = Color.black;
                    break;
            }
        }
    }
}
