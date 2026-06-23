using UnityEngine;
using enemy;
using stateMachine;
using UnityEditor.Experimental.GraphView;
public class EnemyArcherMoveBackState : EnemyState
{
    private EnemyArcher _enemyArcher;
    public EnemyArcherMoveBackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyArcher = enemy as EnemyArcher;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.05f;
        _enemyArcher.SetDirection(-_enemyArcher.direction);
        _enemyArcher.Flip(_enemyArcher.direction);
    }

    public override void Update()
    {
        base.Update();
        _enemyArcher.SetVelocity(new Vector2(_enemyArcher.direction * _enemyArcher.moveSpeed * _enemyArcher.enemyData.MoveMultiplier,rb.velocity.y));
        if(!_enemyArcher.isGrounded || _enemyArcher.isTouchingWall)
        {
            _enemyArcher.SetVelocity(new Vector2(0.001f,rb.velocity.y));
            stateMachine.ChangeState(_enemyArcher.enemyAttackState);
            return;
        }
        if(_enemyArcher.DistanceToPlayer() >= _enemyArcher.enemyData.AttackRange * 0.8f)
        {
            _enemyArcher.SetVelocity(new Vector2(0.001f,rb.velocity.y));
            stateMachine.ChangeState(_enemyArcher.enemyAttackState);
            return;
        }
        if(stateTimer <= 0)
        {
            stateMachine.ChangeState(_enemyArcher.enemyAttackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}