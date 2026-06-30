using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPUIKIT;
using TMPro;
using Habillage;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

namespace Consistenant
{
    public class GachaResult : MonoBehaviour
    {
        [SerializeField] public GameObject GachaResultPanel, GachaResultPanelMain, GachaResultPanelEnd;
        [SerializeField] public MPImage DisplaySpace;
        [SerializeField] public TextMeshProUGUI DisplayName;
        [SerializeField] public List<Transform> star;
        [SerializeField] public List<Gacha> displays;
        [SerializeField] public List<GameObject> displaysObject;
        [SerializeField] public Transform Content;
        [SerializeField] public RectTransform boxsize;
        [SerializeField] public int currentWatching = 0;
        [SerializeField] public Button SkipButton, NextButton;
        [SerializeField] public GameObject ThreeStar, FourStar, FiveStar;
        [SerializeField] public ParticleSystem Confetti;
        [SerializeField] public MMF_Player soundFX_confetti;
        [SerializeField] public GachaResultObjectProperties prefab;
        public void AddDisplayOrder(Gacha adding, Sprite sprite, int rarity, int type)
        {
            displays.Add(adding);
            GachaResultObjectProperties temp = Instantiate(prefab, Content, false);
            temp.SetUpProperties(sprite, rarity, type);
            displaysObject.Add(temp.gameObject);
            UpdateDisplay();
        }

        public void DisplayFirst()
        {
            DisplaySpace.rectTransform.localScale = Vector3.zero;
            LeanTween.scale(DisplaySpace.rectTransform, Vector2.one, 0.1f).setEaseInCubic();
            currentWatching = 0;
            GachaResultPanel.SetActive(true);
            GachaResultPanelMain.SetActive(true);
            GachaResultPanelEnd.SetActive(false);
            if (displays.Count < 2)
            {
                SkipButton.gameObject.SetActive(false);
            }
            else { SkipButton.gameObject.SetActive(true); }
            NextButton.gameObject.SetActive(true);
            UpdateDisplay();
        }

        public void SkipList()
        {
            GachaResultPanel.SetActive(true);
            GachaResultPanelMain.SetActive(false);
            GachaResultPanelEnd.SetActive(true);
        }

        public void DisplayNextInList()
        {
            if (currentWatching < displays.Count - 1)
            {
                currentWatching += 1;
                if (currentWatching >= displays.Count - 1)
                {
                    GachaResultPanel.SetActive(true);
                    SkipButton.gameObject.SetActive(false);
                    NextButton.gameObject.SetActive(true);
                    UpdateDisplay();
                }
                else
                {
                    GachaResultPanel.SetActive(true);
                    SkipButton.gameObject.SetActive(true);
                    NextButton.gameObject.SetActive(true);
                    DisplaySpace.rectTransform.localScale = Vector3.zero;
                    UpdateDisplay();
                }
            }
            else if (currentWatching >= displays.Count - 1)
            {
                GachaResultPanel.SetActive(true);
                GachaResultPanelMain.SetActive(false);
                GachaResultPanelEnd.SetActive(true);
            }
        }

        public void CloseList()
        {
            GachaResultPanel.SetActive(false);
            SkipButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);

            while (displays.Count > 0)
            {
                var display = displays[0];
                var displayObject = displaysObject[0];
                Destroy(displaysObject[0]);
                displays.Remove(display);
                displaysObject.Remove(displayObject);
            }

            displays = new List<Gacha>();
            displaysObject = new List<GameObject>();
            currentWatching = 0;
        }

        public void UpdateDisplay()
        {
            LeanTween.scale(DisplaySpace.rectTransform, Vector2.one, 0.1f).setEaseInCubic();
            if (displays[currentWatching].furnitureData != null)
            {
                DisplaySpace.sprite = displays[currentWatching].furnitureData.Icon;
            }
            else if (displays[currentWatching].characterData != null)
            {
                DisplaySpace.sprite = displays[currentWatching].characterData.Sprites[0];
            }
            DisplayName.text = displays[currentWatching].gachaName;

            for (int i = 1; i < star.Count; i++)
            {
                if (i < displays[currentWatching].rarity)
                {
                    star[i].gameObject.SetActive(true);
                }
                else
                {
                    star[i].gameObject.SetActive(false);
                }
            }

            switch (displays[currentWatching].rarity)
            {
                case 3:
                    ThreeStar.SetActive(true);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(false);
                    Confetti.Play();
                    soundFX_confetti.PlayFeedbacks();
                    break;
                case 4:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(true);
                    FiveStar.SetActive(false);
                    soundFX_confetti.PlayFeedbacks();
                    break;
                case 5:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(true);
                    soundFX_confetti.PlayFeedbacks();
                    break;
                default:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(false);
                    break;
            }

            if (displays[currentWatching].furnitureData != null)
            {
                DisplaySpace.rectTransform.sizeDelta = new Vector2(1, 1) * boxsize.sizeDelta;
            }
            else if (displays[currentWatching].characterData != null)
            {
                DisplaySpace.rectTransform.sizeDelta = new Vector2(.694f, 1) * boxsize.sizeDelta;
            }
        }
    }
}
