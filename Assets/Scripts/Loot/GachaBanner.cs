using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Consistenant;

using Habillage;
using TMPro;

namespace Consistenant
{
    public class GachaBanner : MonoBehaviour
    {
        [SerializeField] public GachaResult gachaResult;
        [SerializeField] public List<Gacha> GachaList = new List<Gacha>();
        [SerializeField] public List<Gacha> GuaranteeList = new List<Gacha>();
        [SerializeField] public TextMeshProUGUI GuaranteeText;

        // Start is called before the first frame update
        Gacha GachaRoll()
        {
            int randomNumber = Random.Range(1, 101); // 1-100
            List<Gacha> possibleItems = new List<Gacha>();

            foreach (Gacha item in GachaList)
            {
                if (randomNumber <= item.dropChance)
                {
                    possibleItems.Add(item);
                }
            }
            if (possibleItems.Count > 0)
            {
                Gacha rolledItem = possibleItems[Random.Range(0, possibleItems.Count)];
                return rolledItem;
            }
            Debug.Log("No Gacha Rolled");
            return null;
        }

        Gacha GuaranteeRoll()
        {
            int randomNumber = Random.Range(1, 101); // 1-100
            List<Gacha> possibleItems = new List<Gacha>();

            foreach (Gacha item in GuaranteeList)
            {
                if (randomNumber <= item.dropChance)
                {
                    possibleItems.Add(item);
                }
            }
            if (possibleItems.Count > 0)
            {
                Gacha rolledItem = possibleItems[Random.Range(0, possibleItems.Count)];
                return rolledItem;
            }
            Debug.Log("No Gacha Rolled");
            return null;
        }

        public void InstantiateLoot()
        {
            if (!PlayerData.Data.Inventory.TrySpendMoney(1000)) return;
            
            PlayerData.Data.UpdateGuarantee(1);
            if (PlayerData.Data.Guarantee >= 100)
            {
                PlayerData.Data.UpdateGuarantee(-100);

                Gacha temp = GuaranteeRoll();
                if (temp != null)
                {
                    if (temp.furnitureData != null)
                    {
                        PlayerData.Data.Inventory.AddFurniture(temp.furnitureData.Key);
                        gachaResult.AddDisplayOrder(temp, temp.furnitureData.Icon, temp.rarity, 0);
                    }
                    else if (temp.characterData != null)
                    {
                        PlayerData.Data.Inventory.AddCharacter(temp.characterData.Name);
                        gachaResult.AddDisplayOrder(temp, temp.characterData.Sprites[0], temp.rarity, 1);
                    }
                }
            }
            else
            {
                Gacha temp = GachaRoll();
                if (temp != null)
                {
                    if (temp.furnitureData != null)
                    {
                        PlayerData.Data.Inventory.AddFurniture(temp.furnitureData.Key);
                        gachaResult.AddDisplayOrder(temp, temp.furnitureData.Icon, temp.rarity, 0);
                    }
                    else if (temp.characterData != null)
                    {
                        PlayerData.Data.Inventory.AddCharacter(temp.characterData.Name);
                        gachaResult.AddDisplayOrder(temp, temp.characterData.Sprites[0], temp.rarity, 1);
                    }
                }
            }
            gachaResult.DisplayFirst();
            GuaranteeText.text = "Total Roll " + PlayerData.Data.Guarantee+"/100\nguarantee get 5 star";
            PlayerData.WriteSave();
        }

        public void InstantiateTenLoots()
        {
            if (!PlayerData.Data.Inventory.TrySpendMoney(9000)) return;
            PlayerData.Data.UpdateGuarantee(10);
            if (PlayerData.Data.Guarantee >= 100)
            {
                PlayerData.Data.UpdateGuarantee(-100);

                Gacha temp = GuaranteeRoll();
                if (temp != null)
                {
                    if (temp.furnitureData != null)
                    {
                        PlayerData.Data.Inventory.AddFurniture(temp.furnitureData.Key);
                        gachaResult.AddDisplayOrder(temp, temp.furnitureData.Icon, temp.rarity, 0);
                    }
                    else if (temp.characterData != null)
                    {
                        PlayerData.Data.Inventory.AddCharacter(temp.characterData.Name);
                        gachaResult.AddDisplayOrder(temp, temp.characterData.Sprites[0], temp.rarity, 1);
                    }
                }

                int numberOfRoll = 9;
                for (int i = 0; i < numberOfRoll; i++)
                {
                    Gacha temp2 = GachaRoll();
                    if (temp2 != null)
                    {
                        if (temp2.furnitureData != null)
                        {
                            PlayerData.Data.Inventory.AddFurniture(temp2.furnitureData.Key);
                            gachaResult.AddDisplayOrder(temp2, temp2.furnitureData.Icon, temp2.rarity, 0);
                        }
                        else if (temp2.characterData != null)
                        {
                            PlayerData.Data.Inventory.AddCharacter(temp2.characterData.Name);
                            gachaResult.AddDisplayOrder(temp2, temp2.characterData.Sprites[0], temp2.rarity, 1);
                        }
                    }
                }
            }
            else
            {
                int numberOfRoll = 10;
                for (int i = 0; i < numberOfRoll; i++)
                {
                    Gacha temp = GachaRoll();
                    if (temp != null)
                    {
                        if (temp.furnitureData != null)
                        {
                            PlayerData.Data.Inventory.AddFurniture(temp.furnitureData.Key);
                            gachaResult.AddDisplayOrder(temp, temp.furnitureData.Icon, temp.rarity, 0);
                        }
                        else if (temp.characterData != null)
                        {
                            PlayerData.Data.Inventory.AddCharacter(temp.characterData.Name);
                            gachaResult.AddDisplayOrder(temp, temp.characterData.Sprites[0], temp.rarity, 1);
                        }
                    }
                }
            }
            gachaResult.DisplayFirst();
            GuaranteeText.text = "Total Roll " + PlayerData.Data.Guarantee+"/100\nguarantee get 5 star";
            PlayerData.WriteSave();
        }
    }
}
