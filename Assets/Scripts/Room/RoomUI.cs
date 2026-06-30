using System;
using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class RoomUI : MonoBehaviour
    {
        public TMP_Text RoomName;
        public RectTransform RoomPanel;
        public Button BackButton;
        public Button EditButton;
        public Button VariantButton;
        public Button ConfirmEditButton;
        public ObjectSelectionUI FurniturePanel;

        public List<GameObject> SelectRoomUIs;
        public List<GameObject> EditRoomUIs;
        public List<GameObject> OtherUIs;

        public RectTransform WhatToShift;
        public float DefaultLocation = 20;
        public float ShiftedLocation = 420;

        public GameObject RedAlert;
        public List<MPImage> ColoredUIs;

        private void Start()
        {
            RoomPanel.gameObject.SetActive(false);
            DeselectRoom();
        }

        private void OnEnable()
        {
            SwitchShift(false);
        }

        public void SwitchShift(bool shiftOn)
        {
            if (shiftOn)
            {
                WhatToShift.anchoredPosition = new Vector2(WhatToShift.anchoredPosition.x, ShiftedLocation);
            }
            else
            {
                WhatToShift.anchoredPosition = new Vector2(WhatToShift.anchoredPosition.x, DefaultLocation);
            }
        }

        public void ToggleActive(GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void SelectRoom(GameObject room)
        {
            RoomPanel.gameObject.SetActive(true);
            RoomName.SetText(room.GetComponent<Room>().HabitTitle);
            // BackButton.gameObject.SetActive(true);
            // EditButton.gameObject.SetActive(true);
            // VariantButton.gameObject.SetActive(false);
            // ConfirmEditButton.gameObject.SetActive(false);
            // FurniturePanel.gameObject.SetActive(false);

            SelectRoom();
        }

        private void Update()
        {
            var _data = PlayerData.Data.CurrentHabit;
            if (_data != null && _data.ScheduleData.ValidToday())
            {
                if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                {
                    RedAlert.SetActive(!_dayData.ResultData.Completed);
                }
                else
                {
                    RedAlert.SetActive(true);
                }
            }
        }

        public void SelectRoom()
        {
            foreach (var ui in OtherUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in EditRoomUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in SelectRoomUIs)
            {
                ui.gameObject.SetActive(true);

                var _data = PlayerData.Data.CurrentHabit;
                foreach (var uis in ColoredUIs)
                {
                    uis.color = _data.Color.FromEnum();
                }
                RoomName.color = _data.Color.FromEnum();
            }
        }

        public void DeselectRoom()
        {
            // RoomPanel.gameObject.SetActive(false);
            // BackButton.gameObject.SetActive(false);
            // EditButton.gameObject.SetActive(false);

            foreach (var ui in SelectRoomUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in EditRoomUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in OtherUIs)
            {
                ui.gameObject.SetActive(true);
            }
        }

        public void EditRoom()
        {
            // RoomPanel.gameObject.SetActive(true);
            // BackButton.gameObject.SetActive(false);
            // EditButton.gameObject.SetActive(false);
            // VariantButton.gameObject.SetActive(true);
            // ConfirmEditButton.gameObject.SetActive(true);
            // FurniturePanel.gameObject.SetActive(true);

            foreach (var ui in OtherUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in SelectRoomUIs)
            {
                ui.gameObject.SetActive(false);
            }

            foreach (var ui in EditRoomUIs)
            {
                ui.gameObject.SetActive(true);
            }
            SwitchShift(true);
        }

        public void ExitEditRoom()
        {
            // BackButton.gameObject.SetActive(true);
            // EditButton.gameObject.SetActive(true);
            // VariantButton.gameObject.SetActive(false);
            // ConfirmEditButton.gameObject.SetActive(false);
            // FurniturePanel.gameObject.SetActive(false);

            SelectRoom();
            SwitchShift(false);
        }
    }
}
