using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Habillage
{
    public class PlayerDataDebug : MonoBehaviour
    {
        public DormManager DormManager;
        public void Toggle(GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void AddItem(FurnitureData data)
        {
            PlayerData.Data.Inventory.AddFurniture(data.Key);
        }

        public void AddAllItems()
        {
            foreach (var id in RuntimeData.FurnitureDatabase.Data.Keys)
            {
                PlayerData.Data.Inventory.AddFurniture(id);
            }
        }

        public void AddAllCharacter()
        {
            foreach (var _id in RuntimeData.CharacterDatabase.Data.Keys)
            {
                PlayerData.Data.Inventory.AddCharacter(_id);
            }
        }

        public void AddMoney(int value)
        {
            PlayerData.Data.Inventory.AddMoney(value);
        }

        public void Save()
        {
            PlayerData.WriteSave();
        }

        public void Load()
        {
            PlayerData.ReadSave();
        }

        public void ClearSave()
        {
            if (DormManager)
            {
                DormManager.PlacementSystem.ClearAllObjects();
            }
            PlayerData.ClearSave();
            PlayerData.WriteSave();

            Application.Quit();
            //SceneManager.LoadScene("AccountScreen");
        }
    }
}
