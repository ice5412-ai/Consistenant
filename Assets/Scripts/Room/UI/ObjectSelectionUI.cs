using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class ObjectSelectionUI : MonoBehaviour
    {
        public FurnitureDatabase FurnitureDatabase;

        [SerializeField] private ObjectButton buttonPrefab;
        [SerializeField] private RectTransform content;

        public UnityEvent<FurnitureData> OnClickedButton;
        private List<ObjectButton> activeButtons = new();

        private void OnEnable()
        {
            foreach (var data in FurnitureDatabase.GetData())
            {
                var newButton = Instantiate(buttonPrefab, content, false);
                newButton.Show(data);
                newButton.Button.onClick.AddListener(() =>
                {
                    OnClickedButton?.Invoke(newButton.FurnitureData);
                    newButton.UpdateNumber();
                });
                activeButtons.Add(newButton);
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
