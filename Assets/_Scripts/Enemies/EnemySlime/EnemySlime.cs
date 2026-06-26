using UnityEngine;

namespace enemy
{
    public class EnemySlime : Enemy
    {
        public EnemySlimeDeathState enemySlimeDeathState { get; private set; }
        [SerializeField] private int _childAmount;
        [SerializeField] private GameObject _slimeChildPefab;
        private Coroutine _spawnCoroutine;
        [SerializeField] private float _shootOutPowerX = 4f;
        [SerializeField] private float _shootOutPowerYMin = 4f;
        [SerializeField] private float _shootOutPowerYMax = 8f;
        [SerializeField] private bool _enableReformAnimation = true;

        protected override void InitializeStates()
        {
            enemyIdleState = new EnemySlimeIdleState(this, stateMachine, "Idle");
            enemyMoveState = new EnemySlimeMoveState(this, stateMachine, "Move");
            enemyChaseState = new EnemySlimeChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemySlimeAttackState(this, stateMachine, "Attack");
            enemyStunState = new EnemySlimeStunState(this, stateMachine, "Hit");
            enemySlimeDeathState = new EnemySlimeDeathState(this, stateMachine, "Died");
        }

        protected override void Start()
        {
            base.Start();
            animator.SetBool("HasRecovery", _enableReformAnimation);
        }

        public override void ChangeToDiedState()
        {
            base.ChangeToDiedState();
            stateMachine.ChangeState(enemySlimeDeathState);
        }

        public void CreateChild()
        {
            if (_slimeChildPefab == null || _childAmount <= 0) return;
            SpawnSlimeChild();
        }

        private void SpawnSlimeChild()
        {
            for (int i = 0; i < _childAmount; i++)
            {
                GameObject go = Instantiate(_slimeChildPefab);
                var enemySlimeChild = go.GetComponent<EnemySlime>();
                ShootUpSlimeChild(enemySlimeChild);
            }
        }

        private void ShootUpSlimeChild(EnemySlime enemySlime)
        {
            Debug.Log("SPawn Enemy child");
            Vector2 velocity = new Vector2(
                Random.Range(-_shootOutPowerX, _shootOutPowerX),
                Random.Range(_shootOutPowerYMin, _shootOutPowerYMax));
            enemySlime.SetVelocity(new Vector2(velocity.x, velocity.y ));
        }
    }
}
