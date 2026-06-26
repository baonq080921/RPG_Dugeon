using UnityEngine;

namespace enemy
{
    public class BossAnimationEvent : EnemyAnimationEvent
    {
        [SerializeField] private EnemyDeathRippler _enemyDeathRippler;
        public void SetTeleportTrigger()
        {
            _enemyDeathRippler.SetTeleportTriggered(true);
        }
    }
}
