using System.Collections;
using System.Collections.Generic;
using Habillage;
using MPUIKIT;
using UnityEngine;

namespace Consistenant
{
    public class ToggleHabitViewMode : MonoBehaviour
    {
        public List<Room> rooms;
        bool _On = false;
        public MPImage img;
        public Sprite OnImg;
        public Sprite OffImg;

        public void Start()
        {
            _On = false;
            img.sprite = _On ? OnImg : OffImg;

            foreach (Room room in rooms)
            {
                room.ToggleGridChart(_On);
            }
        }

        public void Toggle()
        {
            _On = !_On;
            img.sprite = _On ? OnImg : OffImg;

            foreach (Room room in rooms)
            {
                room.ToggleGridChart(_On);
            }
        }

        public void OnDisable()
        {
            _On = false;
            img.sprite = _On ? OnImg : OffImg;

            foreach (Room room in rooms)
            {
                room.ToggleGridChart(_On);
            }
        }
    }
}
