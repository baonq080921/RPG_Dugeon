using UnityEngine;
using enemy;
using stateMachine;
public class EnemyArcherChaseState : EnemyState
{
    private EnemyArcher _enemyArcher;
    public EnemyArcherChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyArcher = enemy as EnemyArcher;
    }


    public override void Enter()
    {
        base.Enter();
        _enemyArcher.SetVelocity(new Vector2(_enemyArcher.direction * _enemyArcher.enemyData.MoveSpeed,rb.velocity.y));
    }
    public override void Update()
    {
        base.Update();
        if (_enemyArcher.isTouchingWall || !_enemyArcher.isGrounded)
        {
            _enemyArcher.Flip(-_enemyArcher.direction);
        }
        if (_enemyArcher.IsPlayerDetected())
        {
            _enemyArcher.SetVelocity(new Vector2(_enemyArcher.direction * _enemyArcher.enemyData.MoveSpeed * _enemyArcher.enemyData.MoveMultiplier, rb.velocity.y));
        }
        if (_enemyArcher.IsInAttackRange())
        {
            stateMachine.ChangeState(_enemyArcher.enemyArcherCombatState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}