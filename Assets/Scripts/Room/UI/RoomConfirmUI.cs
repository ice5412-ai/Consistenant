using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class RoomConfirmUI : MonoBehaviour
    {
        public TMP_Text ConfirmText;
        public Button confirmButton;
        public int Price;

        private void OnEnable()
        {
            confirmButton.interactable = PlayerData.Data.Inventory.Money >= Price;
        }

        private void OnDisable()
        {
            
        }
    }
}
