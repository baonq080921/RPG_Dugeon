using UnityEngine;
using enemy;
using stateMachine;
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
        stateTimer = 0.5f;
    }

    public override void Update()
    {
        base.Update();
        _enemyArcher.SetVelocity(new Vector2(-_enemyArcher.direction * _enemyArcher.moveSpeed,rb.velocity.y));
        if(stateTimer <= 0)
        {
            stateMachine.ChangeState(_enemyArcher.enemyArcherCombatState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}