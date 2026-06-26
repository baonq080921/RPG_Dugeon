using UnityEngine;

namespace enemy
{
    public class EnemyArcher : Enemy
    {
        public EnemyArcherCombatState enemyArcherCombatState;
        public EnemyArcherMoveBackState enemyArcherMoveBackState;
        public EnemyArcherDeathState enemyArcherDeathState;

        [SerializeField] private float _projectileSpeed;
        [SerializeField] private Transform _arrowStartTf;
        [Range(0, 1)]
        [SerializeField] private float _chanceToSpawnIceArrow = 0.4f;

        private float _lastAttackTime = float.NegativeInfinity;
        private ArcherArrowPool _arrowPool;

        public void StartAttackCooldown() => _lastAttackTime = Time.time;
        public bool CanAttack() => Time.time >= _lastAttackTime + enemyData.AttackCooldown;

        protected override void InitializeStates()
        {
            _arrowPool = GetComponent<ArcherArrowPool>();
            enemyIdleState = new EnemyArcherIdleState(this, stateMachine, "Idle");
            enemyMoveState = new EnemyArcherChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemyArcherAttackState(this, stateMachine, "Attack");
            enemyArcherCombatState = new EnemyArcherCombatState(this, stateMachine, "Battle");
            enemyArcherMoveBackState = new EnemyArcherMoveBackState(this, stateMachine, "Move");
            enemyStunState = new EnemyArcherStunState(this, stateMachine, "Hit");
            enemyArcherDeathState = new EnemyArcherDeathState(this, stateMachine, "Died");
        }

        public override void SpecialAttack()
        {
            base.SpecialAttack();
            CreateAndSetUpArrow();
        }

        private void CreateAndSetUpArrow()
        {
            if (Random.Range(0f, 1f) <= _chanceToSpawnIceArrow)
            {
                var arrowIce = _arrowPool.GetIceArrow();
                arrowIce.transform.position = _arrowStartTf.position;
                arrowIce.SetUpArrow(_projectileSpeed * 1.1f, direction, entityCombat);
                return;
            }

            var arrow = _arrowPool.GetArrow();
            arrow.transform.position = _arrowStartTf.position;
            arrow.SetUpArrow(_projectileSpeed, direction, entityCombat);
        }

        public override void ChangeToDiedState()
        {
            base.ChangeToDiedState();
            stateMachine.ChangeState(enemyArcherDeathState);
        }
    }
}
