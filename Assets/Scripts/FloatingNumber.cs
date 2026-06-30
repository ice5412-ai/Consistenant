using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Habillage
{
    public class FloatingNumber : MonoBehaviour
    {
        public TextMeshProUGUI tmp;
        public Color color;
        public Color targetcolor;
        public Vector3 dir;

        public float AnimationTime = 1.5f;
        public float DestroyTime = 3f;

        private void Start()
        {
            tmp.color = color;
            targetcolor = new Color(color.r, color.g, color.b, 0);
            LeanTween.value(gameObject, ChangeTMPcolor, color, targetcolor, AnimationTime).setEase(LeanTweenType.easeOutExpo);
            LeanTween.moveLocal(gameObject, gameObject.transform.localPosition + dir, AnimationTime).setEase(LeanTweenType.easeOutExpo);
            Destroy(this.gameObject, DestroyTime);
        }

        private void ChangeTMPcolor(Color _color)
        {
            tmp.color = _color;
        }
    }
}
