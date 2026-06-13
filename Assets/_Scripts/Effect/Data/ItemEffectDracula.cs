using player;
using UnityEngine;

/// <summary>Dracula item effect: heals the player on attack and applies a HP drain DoT to each hit enemy.</summary>
[CreateAssetMenu(fileName = "ItemEffectDracula", menuName = "RPG/ItemEffectDraculaData", order = 1)]
public class ItemEffectDracula : ItemEffectData
{
    [field: SerializeField] public float HealAmountPercent { get; private set; } = 0.15f;
    [field: SerializeField] public float DotDuration { get; private set; } = 2f;
    [field: SerializeField] public float DotDamagePerTick { get; private set; } = 2f;
    [field: SerializeField] public float DotTickInterval { get; private set; } = 0.5f;

    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        float damage = player.entityStat.GetPhysicalDamageValue(out bool _);
        player.playerHealth.HealHP(damage * HealAmountPercent);
    }

    private void ApplyDotToEnemy(Transform enemyTransform)
    {
        EntityStatusHandler statusHandler = enemyTransform.GetComponent<EntityStatusHandler>();
        if (statusHandler == null) return;
        statusHandler.ApplyDraculaDoT(DotDamagePerTick, DotDuration, DotTickInterval,ElementType.Fire);
    }

    /// <inheritdoc/>
    public override void Subscribe(Player character)
    {
        player = character;
        player.playerCombat.OnPlayerAttacking += ExecuteEffect;
        player.playerCombat.OnPlayerHitEnemy += ApplyDotToEnemy;
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Player character)
    {
        player.playerCombat.OnPlayerAttacking -= ExecuteEffect;
        player.playerCombat.OnPlayerHitEnemy -= ApplyDotToEnemy;
        player = null;
    }
}