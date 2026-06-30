using System.Collections;
using System.Collections.Generic;
using Habillage;
using MPUIKIT;
using TMPro;
using UnityEngine;

namespace Consistenant
{
    public class DailyRewards : MonoBehaviour
    {
        public TextMeshProUGUI ButtonText;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DayText;
        public List<GameObject> Rewards;
        public List<MPImage> CheckMarks;
        public ParticleSystem Particle;


        void OnEnable()
        {
            var _data = PlayerData.Data;
            TitleText.text = _data.DRtaken ? "You already take today's reward" : "Daily Reward!";
            ButtonText.text = _data.DRtaken ? "Done" : "Get Reward!";
            DayText.text = $"Day {_data.Daily_Rewards + 1}";
            for (int i = 0; i < Rewards.Count; i++)
            {
                Rewards[i].SetActive(i == _data.Daily_Rewards);
                CheckMarks[i].gameObject.SetActive(i < _data.Daily_Rewards);
            }
        }

        public void OnGetReward()
        {
            var _data = PlayerData.Data;
            if (_data.DRtaken)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                _data.DRtaken = true;
                switch (_data.Daily_Rewards)
                {
                    case 0:
                        PlayerData.Data.Inventory.AddCharacter("Awake");
                        break;
                    case 1:
                        PlayerData.Data.Inventory.AddFurniture("Guitar");
                        break;
                    case 2:
                        PlayerData.Data.Inventory.AddMoney(2000);
                        break;
                    case 3:
                        PlayerData.Data.Inventory.AddMoney(4000);
                        break;
                    case 4:
                        PlayerData.Data.Inventory.AddMoney(8000);
                        break;
                    case 5:
                        PlayerData.Data.Inventory.AddMoney(16000);
                        break;
                    case 6:
                        PlayerData.Data.Inventory.AddCharacter("Sleep");
                        break;
                }
                Particle.Play();
                CheckMarks[_data.Daily_Rewards].gameObject.SetActive(true);
                Vector3 checkmarkScale = CheckMarks[_data.Daily_Rewards].transform.localScale;
                CheckMarks[_data.Daily_Rewards].transform.localScale = Vector3.zero;

                LeanTween.scale(CheckMarks[_data.Daily_Rewards].gameObject, checkmarkScale, 0.5f).setEaseOutBack().setLoopOnce();
                _data.Daily_Rewards += 1;
                ButtonText.text = _data.DRtaken ? "Done" : "Get Reward!";
                PlayerData.WriteSave();
            }
        }
    }
}
