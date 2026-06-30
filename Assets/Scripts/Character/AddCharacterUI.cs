using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public class AddCharacterUI : MonoBehaviour
    {
        public CharacterButton buttonPrefab;
        public RectTransform content;
        public DormManager DormManager;
        private List<CharacterButton> activeButtons = new();
        
        private void OnEnable()
        {
            foreach (var kvp in RuntimeData.CharacterDatabase.Data)
            {
                var characterData = kvp.Value;
                //if (!PlayerData.Data.Inventory.ActiveCharacter.Contains(kvp.Key))
                {
                    //Debug.Log(kvp.Key);
                    var newButton = Instantiate(buttonPrefab, content, false);
                    newButton.Show(characterData);

                    newButton.Button.onClick.AddListener((() =>
                    {
                        DormManager.AddCharacter(characterData);
                        newButton.UpdateNumber();
                    }));
                    activeButtons.Add(newButton);
                }
            }
            UpdateAllButtons();
            
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

            PlayerData.Data.Inventory.OnCharacterChanged -= UpdateAllButtons;
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
