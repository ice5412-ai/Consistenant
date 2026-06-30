using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Consistenant
{
    public class RainbowText : MonoBehaviour
    {
        public Gradient rainbowGradient;
        public TextMeshProUGUI text;
        public float duration;

        void OnEnable()
        {
            LeanTween.value(gameObject, 0f, 1f, duration).setEaseLinear().setRepeat(-1).setOnUpdate((value) =>
            {
                Color currentColor = rainbowGradient.Evaluate(value);
                text.color = currentColor;
            });
        }

        void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }
    }
}
