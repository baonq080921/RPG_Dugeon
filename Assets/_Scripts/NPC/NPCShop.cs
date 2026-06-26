using Base;
using UnityEngine;

namespace NPC
{
    public class NPCShop : Npc
    {

        public override void OnInteract()
        {
            base.OnInteract();
            EventBus<StoreCallEvent>.Raise(new StoreCallEvent());
        }
    }
}
