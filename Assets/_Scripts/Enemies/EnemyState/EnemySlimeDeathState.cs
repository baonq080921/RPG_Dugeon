using UnityEngine;
using stateMachine;

namespace enemy
{
    public class EnemySlimeDeathState : EnemyState
    {
        private EnemySlime _enemySlime;
        
        public EnemySlimeDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            _enemySlime = enemy as EnemySlime;
        }
        public override void Enter()
        {
            base.Enter();
            _enemySlime.CreateChild();
            stateTimer = 3f;
            rb.simulated = false;
            

            
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
            {                
                enemy.ReturnToPool();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}