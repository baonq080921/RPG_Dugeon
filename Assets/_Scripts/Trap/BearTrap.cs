using System.Collections;
using System.Collections.Generic;
using player;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player))
        {
            player?.Die();
        }
    }
}
