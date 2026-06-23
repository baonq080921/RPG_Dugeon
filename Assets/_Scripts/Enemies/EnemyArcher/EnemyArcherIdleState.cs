using enemy;
using stateMachine;
using UnityEngine;

public class EnemyArcherIdleState : EnemyState
{
    private EnemyArcher _enemyArcher;
    public EnemyArcherIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyArcher = enemy as EnemyArcher;
    }


    public override void Enter()
    {
        base.Enter();
        _enemyArcher.SetVelocity(Vector2.zero);
        stateTimer = _enemyArcher.enemyData.IdleTime;
    }


    public override void Update()
    {
        base.Update();
        if(stateTimer <= 0)
            stateMachine.ChangeState(_enemyArcher.enemyMoveState);
    }

    public override void Exit()
    {
        base.Exit();
    }


}