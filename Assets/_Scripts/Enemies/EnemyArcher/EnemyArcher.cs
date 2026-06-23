using enemy;
using UnityEngine;

public class EnemyArcher : Enemy 
{

    public EnemyArcherCombatState enemyArcherCombatState;
    public EnemyArcherMoveBackState enemyArcherMoveBackState;
    protected override void InitializeStates()
    {
        enemyIdleState = new EnemyArcherIdleState(this,stateMachine, "Idle");
        enemyMoveState = new EnemyArcherChaseState(this,stateMachine,"Move");
        enemyAttackState = new EnemyArcherAttackState(this, stateMachine,"Attack");
        enemyArcherCombatState = new EnemyArcherCombatState(this, stateMachine,"Battle");
        enemyArcherMoveBackState = new EnemyArcherMoveBackState(this,stateMachine,"Move");
    }
    
}