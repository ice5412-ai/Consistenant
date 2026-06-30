using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class HabitDataDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> gamemodeIcons;
        [SerializeField] private TextMeshProUGUI scheduleText;
        [SerializeField] private HabitStatsUI habitStatsUI;
        [SerializeField] private TextMeshProUGUI settingText;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private TextMeshProUGUI colorText;
        [SerializeField] private MPImage colorImage;
        [SerializeField] private Button editButton;
        [SerializeField] private Button scheduleButton;
        public Color uiColor = Color.clear;
        public bool _scheduleDisplay = false;
        public string scheduleTextRecurring;
        public string scheduleTextDays;

        // Done: Display Saved Data here

        public void OnEnable()
        {
            scheduleButton.onClick.AddListener(OnClickscheduleText);
        }
        public void OnDisable()
        {
            scheduleButton.onClick.RemoveListener(OnClickscheduleText);
        }


        public void OnClickscheduleText()
        {
            _scheduleDisplay = !_scheduleDisplay;
            scheduleText.text = !_scheduleDisplay ? scheduleTextRecurring : scheduleTextDays;
        }

        public void UpdateDisplay(GameModeType gameMode, string scheduleRecurring, string scheduleDays, string setting, string notification, string colorName, Color color)
        {
            switch (gameMode)
            {
                case GameModeType.Alarm:
                    foreach (GameObject gamemodeIcon in gamemodeIcons)
                    {
                        gamemodeIcon.SetActive(gamemodeIcon.name == "Alarm");
                    }
                    break;
                case GameModeType.GoalTimer:
                    foreach (GameObject gamemodeIcon in gamemodeIcons)
                    {
                        gamemodeIcon.SetActive(gamemodeIcon.name == "Timer");
                    }
                    break;
                case GameModeType.CheckList:
                    foreach (GameObject gamemodeIcon in gamemodeIcons)
                    {
                        gamemodeIcon.SetActive(gamemodeIcon.name == "Checklist");
                    }
                    break;
                case GameModeType.Quota:
                    foreach (GameObject gamemodeIcon in gamemodeIcons)
                    {
                        gamemodeIcon.SetActive(gamemodeIcon.name == "Quota");
                    }
                    break;
            }
            scheduleTextRecurring = scheduleRecurring;
            scheduleTextDays = scheduleDays;
            scheduleText.text = !_scheduleDisplay ? scheduleTextRecurring : scheduleTextDays;
            settingText.text = setting;
            notificationText.text = notification;
            colorText.text = colorName;
            uiColor = colorImage.color = colorText.color = color;
        }
    }
}
