using UnityEngine;
using enemy;
using Unity.VisualScripting;

public class EnemyArcherCombatState : EnemyState
{

    private EnemyArcher _enemyArcher;
    public EnemyArcherCombatState(Enemy enemy, stateMachine.StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyArcher = enemy as EnemyArcher;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (!_enemyArcher.IsInAttackRange())
        {
            stateMachine.ChangeState(_enemyArcher.enemyMoveState);
            return;
        }
        if (_enemyArcher.DistanceToPlayer() <= _enemyArcher.enemyData.AttackRange * 0.25f)
            stateMachine.ChangeState(_enemyArcher.enemyArcherMoveBackState);
        else
            stateMachine.ChangeState(_enemyArcher.enemyAttackState);
    }
    public override void Exit()
    {
        base.Exit();
    }
}