using Interfaces;
using UnityEngine;
namespace enemy
{
    public class EnemyVfx : EntityVfx, IHitVFX
    {

        private Enemy _enemy;
        [SerializeField] private Alert _alertCounterSignal;

        protected override Material KnockBackMat =>_enemy.enemyData.Material;
        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();
            _alertCounterSignal = GetComponentInChildren<Alert>();
        }



        public void EnableCounterAlert()
        {
            _alertCounterSignal.gameObject.SetActive(true);
        }

        public void DisableCounterAlert()
        {
            _alertCounterSignal.gameObject.SetActive(false);
        }
    }
}