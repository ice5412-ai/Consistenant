using System;
using System.Collections;
using System.Collections.Generic;
using Habillage;
using TMPro;
using UnityEngine;

namespace Consistenant
{
    public class MoneyGenerate : MonoBehaviour
    {

        public static int nextSecondGetResource = 1800;

        public TextMeshProUGUI display;

        void Update()
        {
            var roomIncome = PlayerData.Data.UnlockedRoom * 100;
            var CharacterIncome = PlayerData.Data.Inventory.TotalSpawnedCharacter * 20;
            var _money = roomIncome+CharacterIncome;

            display.text = string.Format("Generating {0} coins in {1:00}:{2:00}\n{3} from Rooms\n{4} from Tenants", _money, Mathf.Abs((PlayerData.Data.NextGenerate - DateTime.Now).Minutes), Mathf.Abs((PlayerData.Data.NextGenerate - DateTime.Now).Seconds), roomIncome, CharacterIncome);

            if (DateTime.Now > PlayerData.Data.NextGenerate)
            {

                TimeSpan diff = DateTime.Now - PlayerData.Data.NextGenerate;

                Debug.Log($"Diff: {diff}");

                PlayerData.Data.Inventory.AddMoney(_money * Mathf.RoundToInt((float)((diff.TotalSeconds + nextSecondGetResource) / nextSecondGetResource)));
                Debug.Log($"AddMoney{_money * Mathf.RoundToInt((float)((diff.TotalSeconds + nextSecondGetResource) / nextSecondGetResource))}");

                PlayerData.Data.NextGenerate = PlayerData.Data.NextGenerate.AddSeconds(nextSecondGetResource).AddTicks(diff.Ticks);
                Debug.Log($"AddSeconds{nextSecondGetResource}AddTicks{diff.Ticks}/{diff.Seconds}");

                PlayerData.WriteSave();
            }
        }
    }
}
