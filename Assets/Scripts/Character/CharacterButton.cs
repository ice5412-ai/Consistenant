using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class CharacterButton : MonoBehaviour
    {
        public UnityEvent<Character> OnClickedRemoved;
        public Character Character;
        public CharacterData Data;
        public Button Button;
        public Image icon;
        public TMP_Text numberTxt;

        public void Show(Character character)
        {
            Character = character;
            Data = character.Data;
            icon.sprite = character.Data.Sprites[character.variantIndex];
        }

        public void Show(CharacterData data)
        {
            Data = data;
            icon.sprite = data.Sprites[0];
            UpdateNumber();
        }

        public void Remove()
        {
            OnClickedRemoved?.Invoke(Character);
        }
        
        public void UpdateNumber()
        {
            if (PlayerData.Data.Inventory.Character.TryGetValue(Data.Name, out var number))
            {
                
            }
            numberTxt.SetText($"x{number}");
        }
    }
}