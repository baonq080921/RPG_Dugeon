using player;
using UnityEngine;
namespace enemy
{
    /// <inheritdoc/>
    public class EnemySkeleton : Enemy
    {
        public EnemySkeletonDeathState enemyDeathState { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
             enemyDeathState = new EnemySkeletonDeathState(this, stateMachine,"Died");

        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void ChangeToDiedState()
        {
            base.ChangeToDiedState();
            stateMachine.ChangeState(enemyDeathState);
        }

    }
}
