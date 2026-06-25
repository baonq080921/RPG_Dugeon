using player;
using UnityEngine;

namespace Trap
{
    public class BearTrap : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Player>(out Player player))
            {
                player?.Die();
            }
        }
    }
}
