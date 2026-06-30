

using System.Collections;
using Base;
using Interfaces;
using player;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out var player)) return;
        StartCoroutine(SaveNextFrame(player));
    }

    // Defer one frame so any OnComplete() callbacks (MarkQuestComplete, etc.)
    // that fire on the same frame as the trigger finish writing to memory first.
    private IEnumerator SaveNextFrame(Player player)
    {
        yield return null;
        ServiceLocator.Get<ISaveService>()?.Save();
        ServiceLocator.Get<PoolManager>().levelupPool.Spawn(player.transform);
    }
}