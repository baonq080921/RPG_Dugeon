using enemy;
using stateMachine;
using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    private EnemyDeathRippler _enemyDeathRippler;
    public EnemyGroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyDeathRippler = enemy as EnemyDeathRippler;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
         if(_enemyDeathRippler.entityHealth.CurrentHealth/_enemyDeathRippler.entityStat.GetHealthValue() < 0.3f 
                && !_enemyDeathRippler.isLastAttackAttempt)
        {
            stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerSpecialLastState);
            return;
        }
        if (enemy.IsPlayerDetected())
            _enemyDeathRippler.TriggerAggro();

        if (_enemyDeathRippler.HasAggro)
            stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
    }
}