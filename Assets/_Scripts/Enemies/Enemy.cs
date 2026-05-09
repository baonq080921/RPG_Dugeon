using UnityEngine;
namespace enemy
{
    public class Enemy: Entity
    {
        public EnemyIdleState enemyIdleState;


        protected override void Awake()
        {
            base.Awake();
            enemyIdleState = new EnemyIdleState(this, stateMachine,"Idle");
        }

        protected override void Start()
        {
            base.Start();
            stateMachine.Initialize(enemyIdleState);
        }

        protected override void Update()
        {
            base.Update();
        }

    }
}