using enemy;
using stateMachine;
using UnityEngine;

public class EnemyDeathRipplerSpecialLastState : EnemyState
{
    private EnemyDeathRippler _enemyDeathRippler;
    public EnemyDeathRipplerSpecialLastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyDeathRippler = enemy as EnemyDeathRippler;
    }

    public override void Enter()
    {
        base.Enter();
        _enemyDeathRippler.UnTargetableEnemy(true);
        _enemyDeathRippler.SpecialLastAttack();
        _enemyDeathRippler.ToggleHealthCanvas(false);
    }

    public override void Update()
    {
        base.Update();
        if(_enemyDeathRippler.AreLastEnemiesDefeated)
        {
            _enemyDeathRippler.isLastAttackAttempt = true;
            stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerTeleportState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _enemyDeathRippler.UnTargetableEnemy(false);
        _enemyDeathRippler.ToggleHealthCanvas(true);
    }
}