using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class HabitSettingQuota : HabitModeSettingUI
    {
        public TMP_InputField Min;
        public TMP_InputField Max;

        public void SetMinMax(Vector2 _value)
        {
            Min.SetTextWithoutNotify(_value.x.ToString(CultureInfo.InvariantCulture));
            Max.SetTextWithoutNotify(_value.y.ToString(CultureInfo.InvariantCulture));
        }

        public Vector2 GetMinMax()
        {
            if (string.IsNullOrWhiteSpace(Min.text))
            {
                Min.text = "0";
            }
            if (string.IsNullOrWhiteSpace(Max.text))
            {
                Max.text = "0";
            }

            return new Vector2(float.Parse(Min.text), float.Parse(Max.text));
        }


        public override GameModeType Type => GameModeType.Quota;
        public override ModeData GetModeData()
        {
            return new QuotaData(GetNotifyData(), GetMinMax());
        }

        [Button]
        public void TestDebug()
        {
            Debug.Log(GetMinMax());
            UnityEngine.Debug.Log(IsValid());
        }

        public override bool IsValid()
        {
            var _value = GetMinMax();


            if (_value.x <= 0)
            {
                if (_value.y > 0)
                {
                    return true;
                }
                return false;
            }

            if (_value.y <= 0)
            {
                if (_value.x > 0)
                {
                    return true;
                }
                return false;
            }

            if (_value.x >= _value.y)
                return false;

            return true;
        }
    }
}
