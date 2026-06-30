using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class ObjectButton : MonoBehaviour
    {
        [SerializeField] private Image Icon;
        public Button Button;
        public FurnitureData FurnitureData;
        public TMP_Text numberTxt;

        public void Show(FurnitureData data)
        {
            FurnitureData = data;
            Icon.sprite = data.Icon;
            UpdateNumber();
        }

        public void UpdateNumber()
        {
            if (PlayerData.Data.Inventory.Furniture.TryGetValue(FurnitureData.Key, out var number))
            {
                
            }
            numberTxt.SetText($"x{number}");
        }

        private void OnDisable()
        {

        }
    }
}
