using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using AYellowpaper.SerializedCollections.Editor.Data;
#endif
using MPUIKIT;
using TMPro;
using Unity.Mathematics;
#if UNITY_EDITOR
using Unity.PlasticSCM.Editor.WebApi;
#endif
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

namespace Habillage
{
    public enum QuotaType
    {
        MinOnly = 0,
        MaxOnly = 1,
        MinMax = 2,
    }
    public class Quota : BaseRecord
    {
        [SerializeField] public TextMeshProUGUI headerText, MIN_TMP, CURRENT_TMP, MAX_TMP, DISPLAY_TMP;
        [SerializeField] public FloatingNumber floatingNumber;
        private float _MIN, _MAX, _CURRENT;
        [SerializeField] private List<float> _REVERSE = new List<float>();
        private string _INPUT = "";
        private double _RESULT = 0.0;
        private float _MIN_SLIDERFLAG = 0, _MAX_SLIDERFLAG = 1;
        float Full;
        private Vector2 TopPos, BottomPos;
        [SerializeField] public Slider slider;
        [SerializeField] public RectTransform sliderHandleArea;
        [SerializeField] public MPImage MIN_arrow, MAX_arrow, ReferenceAnchor;
        [SerializeField] public MPImage GradientMinOnly, GradientMaxOnly, GradientMinMax;

        [SerializeField] private ParticleSystem victoryParticle;
        [SerializeField] public MMF_Player soundFX_confetti;

        [SerializeField] RecordScores recordScores;

        private QuotaType quotaType;

        bool wasDone;
        [SerializeField] HabitStatsUI habitStatsUI;

        [SerializeField] TextMeshProUGUI ScoreText;
        public float ThisScore;

        [SerializeField] AndroidNotificationControl androidNotificationControl;
        [SerializeField] NotificationController notificationController;

        private void OnEnable()
        {
            if (PlayerData.Data.CurrentHabit?.ModeData is not QuotaData _quotaData) return;

            var _current = 0f;

            if (PlayerData.Data.CurrentHabit.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
            {
                if (_dayData.ResultData is QuotaResultData _resultData)
                {
                    _current = _resultData.CurrentValue;
                    ThisScore = _dayData.Score;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                }
                else
                {
                    ThisScore = 0;
                    ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                }
            }

            slider.value = 0;
            MIN_arrow.rectTransform.localPosition = ReferenceAnchor.rectTransform.localPosition;
            MAX_arrow.rectTransform.localPosition = ReferenceAnchor.rectTransform.localPosition;

            SetUpCalculator(_quotaData.IdealValue, _current);
        }

        public void SetUpCalculator(Vector2 _value, float current)
        {
            var _data = PlayerData.Data.CurrentHabit;
            headerText.text = _data.Title;

            _MIN = _value.x;
            _MAX = _value.y;

            _CURRENT = current;

            TopPos = new Vector2(ReferenceAnchor.rectTransform.anchoredPosition.x, sliderHandleArea.anchoredPosition.y + sliderHandleArea.rect.height / 2 + ReferenceAnchor.rectTransform.rect.height / 2);
            BottomPos = new Vector2(ReferenceAnchor.rectTransform.anchoredPosition.x, sliderHandleArea.anchoredPosition.y - sliderHandleArea.rect.height / 2 + ReferenceAnchor.rectTransform.rect.height / 2);

            if (_value.x == 0 && _value.y == 0)
            {
                Debug.LogWarning("Min and Max should not be 0 at the same time");
            }
            else if (_value.x <= 0 && _value.y > 0)
            {
                quotaType = QuotaType.MaxOnly;

                MIN_arrow.gameObject.SetActive(false);
                MAX_arrow.gameObject.SetActive(true);

                Full = _MAX * (3f / 2f);

                _MIN_SLIDERFLAG = 0f;
                _MAX_SLIDERFLAG = _MAX / Full;
                Debug.Log(_MAX_SLIDERFLAG);

                MAX_arrow.rectTransform.LeanMoveLocal(Vector2.Lerp(BottomPos, TopPos, _MAX_SLIDERFLAG), 1).setEaseOutQuad();

                GradientMinOnly.gameObject.SetActive(false);
                GradientMaxOnly.gameObject.SetActive(true);
                GradientMinMax.gameObject.SetActive(false);

                wasDone = _CURRENT <= _MAX;
                ThisScore = _CURRENT <= _MAX ? 120 * 50 : 0;
                ScoreText.text = $"{Mathf.Round(ThisScore)}";
            }
            else if (_value.x > 0 && _value.y <= 0)
            {
                quotaType = QuotaType.MinOnly;

                MIN_arrow.gameObject.SetActive(true);
                MAX_arrow.gameObject.SetActive(false);

                Full = _MIN * (3f / 1f);

                _MIN_SLIDERFLAG = _MIN / Full;
                _MAX_SLIDERFLAG = 1f;
                Debug.Log(_MIN_SLIDERFLAG);

                MIN_arrow.rectTransform.LeanMoveLocal(Vector2.Lerp(BottomPos, TopPos, _MIN_SLIDERFLAG), 1).setEaseOutQuad();

                GradientMinOnly.gameObject.SetActive(true);
                GradientMaxOnly.gameObject.SetActive(false);
                GradientMinMax.gameObject.SetActive(false);

                wasDone = _CURRENT >= _MIN;
                ThisScore = _CURRENT >= _MIN ? 120 * 50 : 0;
                ScoreText.text = $"{Mathf.Round(ThisScore)}";
            }
            else
            {
                quotaType = QuotaType.MinMax;

                MIN_arrow.gameObject.SetActive(true);
                MAX_arrow.gameObject.SetActive(true);

                Full = _MIN + _MAX;

                _MIN_SLIDERFLAG = _MIN / Full;
                _MAX_SLIDERFLAG = _MAX / Full;
                Debug.Log(_MIN_SLIDERFLAG);
                Debug.Log(_MAX_SLIDERFLAG);

                MAX_arrow.rectTransform.LeanMoveLocal(Vector2.Lerp(BottomPos, TopPos, _MAX_SLIDERFLAG), 1).setEaseOutQuad();
                MIN_arrow.rectTransform.LeanMoveLocal(Vector2.Lerp(BottomPos, TopPos, _MIN_SLIDERFLAG), 1).setEaseOutQuad();

                GradientMinOnly.gameObject.SetActive(false);
                GradientMaxOnly.gameObject.SetActive(false);
                GradientMinMax.gameObject.SetActive(true);

                wasDone = _CURRENT >= _MIN && _CURRENT <= _MAX;
                ThisScore = (_CURRENT >= _MIN && _CURRENT <= _MAX) ? 120 * 50 : 0;
                ScoreText.text = $"{Mathf.Round(ThisScore)}";
            }
            ClearInput();
            UpdateSlider();
        }

        public void OnButtonClick(string buttonValue)
        {
            if (_INPUT == "ERROR")
            {
                ClearInput();
            }
            switch (buttonValue)
            {
                case "=":

                    if (_INPUT == "")
                    {
                        _INPUT = "0";
                    }

                    if (_INPUT[^1..] == "%" ||
                        _INPUT[^1..] == "/" ||
                        _INPUT[^1..] == "*" ||
                        _INPUT[^1..] == "-" ||
                        _INPUT[^1..] == "+")
                    {
                        _INPUT = _INPUT.Remove(_INPUT.Length - 1, 1);
                    }
                    CalculateResult();
                    break;

                case "backspace":
                    if (_INPUT.Length > 0)
                    {
                        _INPUT = _INPUT.Remove(_INPUT.Length - 1);
                        UpdateDisplay();
                    }
                    break;

                case "allclear":
                    ClearInput();
                    break;

                case "ADD":
                    CalculateResult();
                    _CURRENT += (float)_RESULT;
                    _REVERSE.Add((float)_RESULT);
                    SpawnFloatingNumber((float)_RESULT);
                    UpdateDisplay();
                    MarkAsCompleted();
                    UpdateSlider();
                    break;

                case "SUBTRACT":
                    CalculateResult();
                    _CURRENT -= (float)_RESULT;
                    _REVERSE.Add(-(float)_RESULT);
                    SpawnFloatingNumber(-(float)_RESULT);
                    UpdateDisplay();
                    MarkAsCompleted();
                    UpdateSlider();
                    break;

                case "reverse":
                    if (_REVERSE.Count != 0)
                    {
                        _CURRENT -= _REVERSE[_REVERSE.Count - 1];
                        _REVERSE.RemoveAt(_REVERSE.Count - 1);
                    }
                    UpdateDisplay();
                    MarkAsCompleted();
                    UpdateSlider();
                    break;

                case "%":
                case "/":
                case "*":
                case "-":
                case "+":

                    if (_INPUT.Length <= 0)
                    {
                        break;
                    }

                    string[] splited = _INPUT.Split('%', '/', '*', '-', '+');
                    if (splited.Length > 1)
                    {
                        CalculateResult();
                        _INPUT += buttonValue;
                        UpdateDisplay();
                        break;
                    }

                    if (_INPUT[^1..] == "%" ||
                    _INPUT[^1..] == "/" ||
                    _INPUT[^1..] == "*" ||
                    _INPUT[^1..] == "-" ||
                    _INPUT[^1..] == "+")
                    {
                        _INPUT = _INPUT.Remove(_INPUT.Length - 1, 1) + buttonValue;
                        UpdateDisplay();
                        break;
                    }

                    if (_INPUT == "")
                    {
                        if (buttonValue == "-")
                        {
                            _INPUT += buttonValue;
                            UpdateDisplay();
                            break;
                        }
                        break;
                    }

                    _INPUT += buttonValue;
                    UpdateDisplay();
                    break;

                case ".":
                    if (_INPUT.Split('%', '/', '*', '-', '+').Last().Contains("."))
                    {
                        break;
                    }
                    _INPUT += buttonValue;
                    UpdateDisplay();
                    break;

                default:
                    if (_INPUT.Length == 1)
                    {
                        if (_INPUT[^1..] == "0")
                        {
                            _INPUT = _INPUT.Remove(_INPUT.Length - 1, 1) + buttonValue;
                            UpdateDisplay();
                            break;
                        }
                    }

                    if (_INPUT.Length >= 2)
                    {
                        if (_INPUT[^1..] == "0")
                        {
                            if (_INPUT[^2..] == "%0" ||
                            _INPUT[^2..] == "/0" ||
                            _INPUT[^2..] == "*0" ||
                            _INPUT[^2..] == "-0" ||
                            _INPUT[^2..] == "+0")
                            {
                                _INPUT = _INPUT.Remove(_INPUT.Length - 1, 1);
                            }
                        }
                    }
                    _INPUT += buttonValue;
                    UpdateDisplay();
                    break;
            }
        }

        public void CalculateResult()
        {
            try
            {
                _RESULT = Convert.ToDouble(new System.Data.DataTable().Compute(_INPUT, ""));

                _INPUT = _RESULT.ToString(CultureInfo.InvariantCulture);
                UpdateDisplay();
            }
            catch (System.Exception)
            {
                _INPUT = "ERROR";
                UpdateDisplay();
            }
        }

        public void ClearInput()
        {
            _INPUT = "";
            _RESULT = 0.0;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            MIN_TMP.text = _MIN.ToString(CultureInfo.InvariantCulture);
            CURRENT_TMP.text = _CURRENT.ToString(CultureInfo.InvariantCulture);
            MAX_TMP.text = _MAX.ToString(CultureInfo.InvariantCulture);
            DISPLAY_TMP.text = _INPUT;
        }

        public void UpdateSlider()
        {
            switch (quotaType)
            {
                case QuotaType.MinOnly:
                    if (_CURRENT >= _MIN)
                    {
                        victoryParticle.Play();
                        soundFX_confetti.PlayFeedbacks();
                    }
                    break;
                case QuotaType.MaxOnly:
                    if (_CURRENT <= _MAX)
                    {
                        victoryParticle.Play();
                        soundFX_confetti.PlayFeedbacks();
                    }
                    break;
                case QuotaType.MinMax:
                    if (_CURRENT >= _MIN && _CURRENT <= _MAX)
                    {
                        victoryParticle.Play();
                        soundFX_confetti.PlayFeedbacks();
                    }
                    break;
            }

            LeanTween.cancel(slider.gameObject);
            //float temp = (float)Mathf.Round(_CURRENT / (_MIN + _MAX) * 1000) / 1000;

            LeanTween.value(slider.gameObject, SliderIsMoving, slider.value, Mathf.Round(_CURRENT / Full * 1000) / 1000, .5f).setEaseOutQuad();
        }

        public void SliderIsMoving(float _value)
        {
            slider.value = _value;
        }

        public void SpawnFloatingNumber(float _value)
        {
            if (_value == 0)
            {
                return;
            }
            Vector3 Destination = new Vector3(0, CURRENT_TMP.transform.lossyScale.y * 75, 0);
            //Debug.Log(CURRENT_TMP.transform.lossyScale.y);
            FloatingNumber newfloatingNumber = Instantiate(floatingNumber, CURRENT_TMP.rectTransform.position + (_value > 0 ? Destination : -Destination), quaternion.identity, CURRENT_TMP.rectTransform);
            newfloatingNumber.color = _value > 0 ? ColorPreset.Green : ColorPreset.Red;
            newfloatingNumber.tmp.text = _value > 0 ? "+" + MathF.Round(_value * 100) / 100 : (MathF.Round(_value * 100) / 100).ToString();
            newfloatingNumber.dir = _value > 0 ? Vector3.up * 75 : Vector3.down * 75;
            newfloatingNumber.AnimationTime = 1.5f;
            newfloatingNumber.DestroyTime = 2.5f;
        }

        public override void SaveData(bool _complete = false)
        {
            if (PlayerData.Data.CurrentHabit == null ||
                PlayerData.Data.CurrentHabit.ModeData.Type != GameModeType.Quota) return;

            var _quota = new QuotaResultData
            {
                Ideal = new Vector2(_MIN, _MAX),
                CurrentValue = _CURRENT,
                Completed = _complete
            };

            PlayerData.Data.CurrentHabit.DaysData[DateTime.Now.ToShortDateString()] = new DayData(_quota) { Score = ThisScore };

            DormManager.Current?.Save();
            PlayerData.WriteSave();

#if UNITY_ANDROID
            if (PlayerData.Data.CurrentHabit.ScheduleData.ValidToday())
            {
                if (!_complete)
                {
                    var _notiTime = PlayerData.Data.CurrentHabit.ModeData.Notify.ToTimeSpan();
                    androidNotificationControl.SendNotification(PlayerData.Data.CurrentHabit.Title, PlayerData.Data.CurrentHabit.Description, DateTime.Today.Add(_notiTime), IconListEnum.small, IconListEnum.large, PlayerData.Data.CurrentHabit.notificationId);
                }
                else
                {
                    androidNotificationControl.CancelNotification(PlayerData.Data.CurrentHabit.notificationId);
                }
            }

            notificationController.DailyResetNotification();
#endif

            // Debug.Log(PlayerData.Data.CurrentHabit.SerializeData().ToString());
        }

        /*public void ClickMarkAsCompleted()
        {
            MarkAsCompleted();
        }*/

        public override bool MarkAsCompleted()
        {
            bool isSuccessful = false;
            switch (quotaType)
            {
                case QuotaType.MaxOnly:
                    if (_CURRENT <= _MAX)
                    {
                        if (!wasDone)
                        {
                            recordScores.ScoreAdd(100 * 50);
                            ThisScore += 100 * 50;
                            ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                            wasDone = true;
                        }
                        isSuccessful = true;
                    }
                    else
                    {
                        if (wasDone)
                        {
                            recordScores.ScoreAdd(-100 * 50);
                            ThisScore -= 100 * 50;
                            ScoreText.text = $"{Mathf.Round(ThisScore)}";
                            wasDone = false;
                        }
                        isSuccessful = false;
                    }
                    break;

                case QuotaType.MinMax:
                    if (_CURRENT <= _MAX && _CURRENT >= _MIN)
                    {
                        if (!wasDone)
                        {
                            recordScores.ScoreAdd(100 * 50);
                            ThisScore += 100 * 50;
                            ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                            wasDone = true;
                        }
                        isSuccessful = true;
                    }
                    else if (_CURRENT > _MAX)
                    {
                        if (wasDone)
                        {
                            recordScores.ScoreAdd(-100 * 50);
                            ThisScore -= 100 * 50;
                            ScoreText.text = $"{Mathf.Round(ThisScore)}";
                            wasDone = false;
                        }
                        isSuccessful = false;
                    }
                    else if (_CURRENT < _MIN)
                    {
                        if (wasDone)
                        {
                            recordScores.ScoreAdd(-100 * 50);
                            ThisScore -= 100 * 50;
                            ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                            wasDone = false;
                        }
                        isSuccessful = false;
                    }
                    break;

                case QuotaType.MinOnly:
                    if (_CURRENT >= _MIN)
                    {
                        if (!wasDone)
                        {
                            recordScores.ScoreAdd(100 * 50);
                            ThisScore += 100 * 50;
                            ScoreText.text = $"+{Mathf.Round(ThisScore)}";
                            wasDone = true;
                        }
                        isSuccessful = true;
                    }
                    else
                    {
                        if (wasDone)
                        {
                            recordScores.ScoreAdd(-100 * 50);
                            ThisScore -= 100 * 50;
                            ScoreText.text = $"{Mathf.Round(ThisScore)}";
                            wasDone = false;
                        }
                        isSuccessful = false;
                    }
                    break;
            }
            SaveData(isSuccessful);
            return isSuccessful;
        }

        public void Back()
        {
            habitStatsUI.gameObject.SetActive(true);
            Parent.SetActive(false);
            habitStatsUI.ReturnedToHabitStatsUI();
        }
    }
}