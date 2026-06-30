using System;
using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using Unity.VisualScripting.ReorderableList;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    [ExecuteInEditMode]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private MPImage mPImage;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] Button button;
        [SerializeField] public Color mainColor;
        [SerializeField] public Color subColor;
        [SerializeField] public bool isToggled;
        [SerializeField] public ToggleEvent OnToggleChanged = new ToggleEvent();
        [Serializable] public class ToggleEvent : UnityEngine.Events.UnityEvent { }

        // Start is called before the first frame update
        void Start()
        {
            button.onClick.AddListener(ToggleButtonPressed);
            UpdateColor();
        }

        void Update()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                UpdateColor();
            }
        }

        public void UpdateColor()
        {
            if (mPImage != null && textMeshProUGUI != null && button != null)
            {
                if (!isToggled)
                {
                    mPImage.color = mainColor;
                    mPImage.StrokeWidth = 10;
                    textMeshProUGUI.color = mainColor;
                }
                else
                {
                    mPImage.color = mainColor;
                    mPImage.StrokeWidth = 0;
                    textMeshProUGUI.color = subColor;
                }
            }
        }

        private void ToggleButtonPressed()
        {
            isToggled = !isToggled;
            UpdateColor();
            if (OnToggleChanged != null)
            {
                OnToggleChanged.Invoke();
            }
        }
    }
}
