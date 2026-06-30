using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Habillage
{
    public class CharacterUIController : MonoBehaviour
    {
        public CharacterButton buttonPrefab;
        public RectTransform content;
        public UnityEvent<Character> OnClickedRemove;
        public UnityEvent OnClickedAdd;

        public DormManager DormManager;

        private List<CharacterButton> activeButtons = new();
        public RoomUI roomUI;

        public void Add()
        {
            OnClickedAdd?.Invoke();
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);


        }

        private void Start()
        {
            UpdateMoney(PlayerData.Data.Inventory.Money);
            PlayerData.Data.Inventory.OnMoneyChanged += UpdateMoney;
        }

        public void UpdateMoney(int _money)
        {
            var _doneToday = PlayerData.Data.CreatedHabits.Where(_h =>
                _h.Value.DaysData.ContainsKey(DateTime.Today.ToShortDateString())).Count(_h =>
                _h.Value.DaysData[DateTime.Today.ToShortDateString()].ResultData.CompleteStat() ==
                HabitCompletion.Succeed);
            //Update ui here
        }

        private void OnEnable()
        {
            if (!DormManager.SelectedRoom) return;
            foreach (var character in DormManager.SelectedRoom.Characters)
            {
                AddButton(character);
            }
            roomUI.SwitchShift(true);
        }

        public void AddButton(Character _character)
        {
            var newButton = Instantiate(buttonPrefab, content, false);
            newButton.Show(_character);
            newButton.transform.SetAsFirstSibling();

            newButton.OnClickedRemoved.AddListener(_character =>
            {
                OnClickedRemove?.Invoke(_character);
                activeButtons.Remove(newButton);
                Destroy(newButton.gameObject);
            });

            activeButtons.Add(newButton);
        }

        public void SyncUI()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            while (activeButtons.Count > 0)
            {
                var button = activeButtons[0];
                Destroy(button.gameObject);
                activeButtons.Remove(button);
            }
            roomUI.SwitchShift(false);
        }
    }
}
