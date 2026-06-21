using enemy;
using stateMachine;
using UnityEngine;

public class EnemyDeathRipplerUltimateState : EnemyState
{
    private EnemyDeathRippler _enemyDeathRippler;
    public EnemyDeathRipplerUltimateState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        _enemyDeathRippler = enemy as EnemyDeathRippler;
    }

    public override void Enter()
    {
        base.Enter();
        _enemyDeathRippler.UnTargetableEnemy(true);
        stateTimer = _enemyDeathRippler.enemyData.SkillDuration;
        _enemyDeathRippler.SpecialAttack();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer <= 0)
            stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
    }

    public override void Exit()
    {
        base.Exit();
        _enemyDeathRippler.StopSpecialAttack();
        _enemyDeathRippler.UnTargetableEnemy(false);
        _enemyDeathRippler.ResetUltCoolDownTick();
    }
}