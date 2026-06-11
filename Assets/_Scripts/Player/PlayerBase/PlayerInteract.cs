using Interfaces;
using UnityEngine;
namespace player
{
    
    public class PlayerInteract : MonoBehaviour
    {

        private Collider2D[] results = new Collider2D[20];
        [SerializeField] private LayerMask _interactLayer;
        [SerializeField] private float _dectectRadius;
        // when player interact with npc,with some case, moneny,...
        public void InteractDectect()
        {
            int dectectCount = Physics2D.OverlapCircleNonAlloc(transform.position,_dectectRadius,results,_interactLayer);
            for(int i = 0 ; i < dectectCount; i++)
            {
                if(results[i].TryGetComponent<IInteractable>(out var interact))
                {
                    interact?.OnInteract();
                }
            }

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,_dectectRadius);
        }
    }
}
