using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Habillage
{
    public class ShopUI : MonoBehaviour
    {
        public ShopItemUI itemPrefab;
        public RectTransform content;
        
        public UnityEvent<FurnitureData> OnClickedButton;
        
        private List<ShopItemUI> activeButtons = new();

        public void OpenBuyPanel(FurnitureData _data)
        {
            
        }
        
        private void OnEnable()
        {
            foreach (var data in RuntimeData.FurnitureDatabase.Data.Values)
            {
                var newbutton = Instantiate(itemPrefab, content, false);
                newbutton.Show(data);
                
                newbutton.Button.onClick.AddListener((() =>
                {
                    OnClickedButton?.Invoke(newbutton.Data);
                }));
                
                activeButtons.Add(newbutton);
            }
            
            PlayerData.Data.Inventory.OnInventoryChanged += UpdateAllButtons;
        }

        private void OnDisable()
        {
            while (activeButtons.Count > 0)
            {
                var button = activeButtons[0];
                Destroy(button.gameObject);
                activeButtons.Remove(button);
            }
            
            PlayerData.Data.Inventory.OnInventoryChanged -= UpdateAllButtons;
        }
        
        public void UpdateAllButtons()
        {
            foreach (var button in activeButtons)
            {
                button.UpdateNumber();
            }
        }
    }
}