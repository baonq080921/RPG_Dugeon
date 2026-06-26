using Base;
using Inventory;
using player;
using UnityEngine;

namespace store
{
    public class Store : MonoBehaviour
    {
        private PlayerInventory playerInventory;
        private void CanBuyItem()
        {
            playerInventory = ServiceLocator.Get<Player>().GetComponent<PlayerInventory>();
            float asset = playerInventory.Money;

        }
    }
}
