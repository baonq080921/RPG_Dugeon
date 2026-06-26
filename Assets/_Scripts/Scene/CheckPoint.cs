using System.Collections;
using Base;
using player;
using Pool;
using Save;
using UnityEngine;

namespace scene
{
    public class CheckPoint : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent<Player>(out var player)) return;
            StartCoroutine(SaveNextFrame(player));
        }

        private IEnumerator SaveNextFrame(Player player)
        {
            yield return null;
            ServiceLocator.Get<SaveManager>().Save();
            ServiceLocator.Get<PoolManager>().levelupPool.Spawn(player.transform);
        }
    }
}
