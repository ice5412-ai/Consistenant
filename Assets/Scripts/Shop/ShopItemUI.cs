using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Image Icon;
        public Button Button;
        public FurnitureData Data;
        public TMP_Text priceText;
        public TMP_Text numberTxt;

        public UnityEvent<FurnitureData> OnClicked;

        private void Awake()
        {
            Button.onClick.AddListener(() => OnClicked?.Invoke(Data));
        }

        public void Show(FurnitureData data)
        {
            Data = data;
            Icon.sprite = data.Icon;
            priceText.SetText(data.Price.ToString());
            UpdateNumber();
        }
        
        public void UpdateNumber()
        {
            if (PlayerData.Data.Inventory.Furniture.TryGetValue(Data.Key, out var number))
            {
                
            }
            numberTxt.SetText($"x{number}");
        }
    }
}
