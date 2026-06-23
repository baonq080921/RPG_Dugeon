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
        Debug.Log("This is Combat State");
    }

    public override void Update()
    {
        base.Update();
        if (_enemyArcher.IsInAttackRange())
        {
            stateMachine.ChangeState(_enemyArcher.enemyAttackState);
            return;
        }
        
    
    }
    public override void Exit()
    {
        base.Exit();
    }
}