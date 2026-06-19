using Base;
using NPC;
using UnityEngine;
public class NPCShop : Npc
{
    
    public override void OnInteract()
    {
        base.OnInteract();
        EventBus<StoreCallEvent>.Raise(new StoreCallEvent());
    }
}
