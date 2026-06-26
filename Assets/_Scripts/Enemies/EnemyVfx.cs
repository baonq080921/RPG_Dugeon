using entity;
using Interfaces;
using UnityEngine;
namespace enemy
{
    public class EnemyVfx : EntityVfx
    {

        private Enemy _enemy;
        [SerializeField] private Alert _alertCounterSignal;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();
            _alertCounterSignal = GetComponentInChildren<Alert>();
            knockBackMat = _enemy.enemyData.Material;
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