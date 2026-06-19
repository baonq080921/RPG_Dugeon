

using Base;
using player;
using Save;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    private bool _isSave = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out var player))
        {
            if (!_isSave)
            {
                //call save:
            _isSave = true;
            ServiceLocator.Get<SaveManager>().Save();
            ServiceLocator.Get<PoolManager>().levelupPool.Spawn(player.transform);
            }
            
        }
    }
}