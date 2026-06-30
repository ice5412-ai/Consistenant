using System.Collections;
using System.Collections.Generic;
using Consistenant;
using UnityEngine;
using UnityEngine.Events;

namespace Habillage
{
    public class ShopManager : MonoBehaviour
    {
        public UnityEvent<FurnitureData> OnTryBuyItem;
        public UnityEvent<FurnitureData> OnBoughtItem;
        public UnityEvent OnCancelBuy;

        public ShopUI ShopUI;
        public ConfirmBuyPanel BuyPanel;
        
        public void OpenShop()
        {
            ShopUI.gameObject.SetActive(true);
        }

        public void CloseShop()
        {
            ShopUI.gameObject.SetActive(false);
        }
        
        public void OpenBuyPanel(FurnitureData data)
        {
            BuyPanel.Show(data);
        }
        
        public void TryBuyItem(FurnitureData data)
        {
            if (PlayerData.Data.Inventory.TrySpendMoney(data.Price))
            {
                PlayerData.Data.Inventory.AddFurniture(data.Key);
                OnBoughtItem?.Invoke(data);
            }
            else
            {
                
            }
        }
    }
}
