using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class ConfirmBuyPanel : MonoBehaviour
    {
        public TMP_Text NameText;
        public TMP_Text PriceText;
        public Image Icon;
        public FurnitureData Data;

        public UnityEvent<FurnitureData> OnClickBuy;

        private void Start()
        {
            Hide();
        }

        public void Show(FurnitureData _data)
        {
            gameObject.SetActive(true);
            NameText.SetText(_data.Key);
            PriceText.SetText(_data.Price.ToString());
            Icon.sprite = _data.Icon;
            Data = _data;
        }

        public void ClickBuy()
        {
            OnClickBuy?.Invoke(Data);
            Hide();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Data = null;
        }
    }
}