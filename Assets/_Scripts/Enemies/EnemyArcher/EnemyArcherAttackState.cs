using UnityEngine;
using enemy;
using stateMachine;

/// <summary>
/// Locks the archer in place while the attack animation plays, then returns to <see cref="EnemyArcherCombatState"/>.
/// </summary>
public class EnemyArcherAttackState : EnemyState
{
    private EnemyArcher _enemyArcher;

    public EnemyArcherAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        _enemyArcher = enemy as EnemyArcher;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(Vector2.zero);
        animator.SetFloat("AttackMultplier", enemy.attackSpeed);
    }

    public override void Update()
    {
        base.Update();
        if (!isTriggered) return;
        if (!_enemyArcher.IsPlayerDetected())
            stateMachine.ChangeState(_enemyArcher.enemyIdleState);
        else
            stateMachine.ChangeState(_enemyArcher.enemyArcherCombatState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}