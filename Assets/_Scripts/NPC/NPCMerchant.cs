using Base;
using UnityEngine;
namespace NPC
{
    public class NpcMerchant : Npc
    {
                
        public override void OnInteract()
        {
            Debug.Log("Hello this is NPC Merchant");
            // Now Open the Store:
            EventBus<CraftStoreCallEvent>.Raise(new CraftStoreCallEvent());

        }


        
    }
}