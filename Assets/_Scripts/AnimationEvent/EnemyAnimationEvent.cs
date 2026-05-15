using player;
using UnityEngine;
namespace enemy
{
    public class EnemyAnimationEvent: EntityAnimationEvent
    {
        
        private Enemy _enemy;
        private EnemyVfx _enemyVFX;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponentInParent<Enemy>();
            _enemyVFX = GetComponentInParent<EnemyVfx>(false);
        }
        public void EnableAlertCounterSingal()
        {
            _enemy.CanCounter = true;
            _enemyVFX.EnableCounterAlert();
        }

        public void DisableAlertCounterSingal()
        {
            _enemy.CanCounter = false;
            _enemyVFX.DisableCounterAlert();
        }
    }
}