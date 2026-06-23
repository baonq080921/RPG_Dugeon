using UnityEngine;
using enemy;
using stateMachine;
public class EnemyArcherChaseState : EnemyState
{
    private EnemyArcher _enemyArcher;
    private float _velocity;
    public EnemyArcherChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
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
        _velocity = _enemyArcher.enemyData.MoveSpeed;
        if (_enemyArcher.isTouchingWall || !_enemyArcher.isGrounded)
        {
            _enemyArcher.Flip(-_enemyArcher.direction);
            _enemyArcher.SetDirection(-_enemyArcher.direction);
        }
        if (_enemyArcher.IsPlayerDetected())
        {
            _velocity = _velocity * _enemyArcher.enemyData.MoveMultiplier;
        }
        if (_enemyArcher.IsInAttackRange())
        {
            stateMachine.ChangeState(_enemyArcher.enemyArcherCombatState);
        }
        _enemyArcher.SetVelocity(new Vector2(_enemyArcher.direction * _velocity, rb.velocity.y));

    }

    public override void Exit()
    {
        base.Exit();
    }
}