using player;
using UnityEngine;

public class SkillTimeEcho : SkillBase
{
    [SerializeField] private SkillObjectTimeEchoPool _pool;
    [SerializeField] private Player _player;

    protected override void Awake()
    {
        base.Awake();
        if (_pool == null) Debug.LogError("Time Echo skill missing reference to its object pool.");
        if (_player == null) Debug.LogError("Time Echo skill missing reference to player.");
        _player = GetComponentInParent<Player>();
    }

    public override void ExecuteSkillEffect()
    {
        if (!_player.isGrounded) return;
        base.ExecuteSkillEffect();
        SkillObjectTimeEcho instance = _pool.GetInstance();
        switch (skillUpgrade)
        {
            case SkillUpgrade.TimeEcho:
                instance.TimeEchoBase(_player.transform);
                break;
            case SkillUpgrade.TimeEcho_ExtraEchoAttack:
                instance.TimeEchoSideKickAttack(_player.transform);
                break;
            case SkillUpgrade.TimeEcho_HealOnEcho:
                instance.TimeEchoHealing(_player.transform);
                break;
            case SkillUpgrade.TimeEcho_HealOnEchoAndDuration:
                instance.TimeEChoHealingCoolDown(_player.transform);
                break;
        }
    }

    public override void SetUpgradeForSkill(UpgradeData upgrade)
    {
        base.SetUpgradeForSkill(upgrade);
        SkillBaseDefinition.SetToUpgradeDamage(upgrade.UpgradeDamage);
    }
}
