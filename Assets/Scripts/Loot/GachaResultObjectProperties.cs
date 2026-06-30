using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using MPUIKIT;
using UnityEngine;

namespace Consistenant
{
    public class GachaResultObjectProperties : MonoBehaviour
    {
        public MPImage image;
        public int rarity;
        public List<Transform> star;
        public RectTransform boxsize;
        public GameObject ThreeStar, FourStar, FiveStar;
        public void SetUpProperties(Sprite sprite, int rarity, int type)
        {
            image.sprite = sprite;
            for (int i = 1; i < star.Count; i++)
            {
                if (i < rarity)
                {
                    star[i].gameObject.SetActive(true);
                }
                else
                {
                    star[i].gameObject.SetActive(false);
                }
            }
            switch (rarity)
            {
                case 3:
                    ThreeStar.SetActive(true);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(false);
                    break;
                case 4:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(true);
                    FiveStar.SetActive(false);
                    break;
                case 5:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(true);
                    break;
                default:
                    ThreeStar.SetActive(false);
                    FourStar.SetActive(false);
                    FiveStar.SetActive(false);
                    break;
            }
            if (type == 0)
            {
                image.rectTransform.sizeDelta = new Vector2(1, 1) * boxsize.sizeDelta;
            }
            else if (type == 1)
            {
                image.rectTransform.sizeDelta = new Vector2(.694f, 1) * boxsize.sizeDelta;
            }
        }
    }
}
