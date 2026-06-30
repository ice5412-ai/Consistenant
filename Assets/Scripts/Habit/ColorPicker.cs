using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class ColorPicker : MonoBehaviour
    {
        public SerializedDictionary<ColorPresetEnum, MPImage> ColorImage = new();
        [SerializeField] public ColorPresetEnum selectedColor;
        [SerializeField] public List<MPImage> elements;
        [SerializeField] public List<ToggleButton> toggleButtons;
        [SerializeField] public List<ToggleButton> toggleButtons_alt;
        [SerializeField] public List<TextMeshProUGUI> texts;

        public UnityEvent<ColorPresetEnum> OnPickedColor;
        // Start is called before the first frame update

        public void ColorPick(int index)
        {
            selectedColor = (ColorPresetEnum)index;
            //Debug.Log(selectedColor);
            foreach (var temp in ColorImage)
            {
                if (temp.Key == selectedColor)
                {
                    temp.Value.OutlineWidth = 10f;
                }
                else
                {
                    temp.Value.OutlineWidth = 00;
                }
            }

            OnPickedColor?.Invoke(selectedColor);

            UpdateElementColor();
        }

        public void UpdateElementColor()
        {
            if (elements != null)
            {
                foreach (MPImage element in elements)
                {
                    element.color = ColorPreset.FromEnum(selectedColor);
                }
            }
            if (toggleButtons != null)
            {
                foreach (ToggleButton toggleButton in toggleButtons)
                {
                    toggleButton.mainColor = Color.white;
                    toggleButton.subColor = Color.black;
                    toggleButton.UpdateColor();
                }
            }
            if (toggleButtons_alt != null)
            {
                foreach (ToggleButton toggleButton_alt in toggleButtons_alt)
                {
                    toggleButton_alt.mainColor = ColorPreset.FromEnum(selectedColor);
                    toggleButton_alt.subColor = Color.white;
                    toggleButton_alt.UpdateColor();
                }
            }
        }
    }
}
