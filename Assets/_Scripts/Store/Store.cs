using Base;
using player;
using UnityEngine;

public class Store : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private void CanBuyItem()
    {
        playerInventory = ServiceLocator.Get<Player>().GetComponent<PlayerInventory>();
        float asset = playerInventory.Money;
        
    }
}