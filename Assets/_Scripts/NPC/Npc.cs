using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
namespace NPC
{
    public  class Npc : MonoBehaviour, IInteractable
    {

        // [SerializeField] protected UIFloating_Panel _uIFloating_Panel;
        public virtual void OnInteract()
        {
            // _uIFloating_Panel.ShowPanel(true);

        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<player.PlayerInteract>(out var playerInteract))
            {
                playerInteract?.InteractDectect();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // _uIFloating_Panel.ShowPanel(false);
        }

    }

}


