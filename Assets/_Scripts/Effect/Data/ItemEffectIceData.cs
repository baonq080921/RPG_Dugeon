using Base;
using Interfaces;
using player;
using UnityEngine;

/// <summary>Ice item effect that triggers when the player takes damage while below 40% health.</summary>
[CreateAssetMenu(fileName = "ItemEffect-IceData", menuName = "RPG/ItemEffectIceData")]
public class ItemEffectIceData : ItemEffectData
{
    private const float HealthThreshold = 0.4f;
    [field:SerializeField] public float effectCoolDown{get; private set;} = 6f;
    private float _lastTriggerTime = float.NegativeInfinity;
    [SerializeField] public GameObject _iceEffectPrefab;
    [SerializeField] public float _slowDownScaleFactor{get; private set;} = 0.6f;
    [field:SerializeField] public LayerMask hitLayer{get; private set;}
    [field:SerializeField] public float iceElementalDamage{get;private set;}
    [field:SerializeField] public float damageRadius{get; private set;} = 3f;

    protected override void ExecuteEffect()
    {

        if (player == null) return;
        float healthRatio = player.playerHealth.CurrentHealth / player.playerHealth.MaxHealth;
        if (healthRatio >= HealthThreshold) return;
        if (Time.time - _lastTriggerTime < effectCoolDown) return;
        DebugCustom.Log("Ice effect triggered at low health!");
        _lastTriggerTime = Time.time;
        //dectect target
        player.entityVfx.CreateEffectVFX(_iceEffectPrefab,player.transform);
        DamageTargetInExpolsiveRange(player.transform,iceElementalDamage,damageRadius);
    }

    private void DamageTargetInExpolsiveRange(Transform tf, float elemenatlDamage, float radius)
    {
        var colliders = Physics2D.OverlapCircleAll(tf.position, radius, hitLayer);
        Debug.Log(colliders);
        foreach(var col in colliders)
            Debug.Log(col.gameObject.name);
        foreach(var collider in colliders)
        {
            var target = collider.GetComponent<IHit>();
            if(target != null)
            {
                Transform targetTf = collider.GetComponent<Transform>();
                target?.TakeDamage(0,elemenatlDamage,ElementType.Ice,targetTf);
                player.entityCombat.ApplyStatusEffect(ElementType.Ice,targetTf,_slowDownScaleFactor); // apply ice status for all enemy
            }
        }
    }


    /// <inheritdoc/>
    public override void Subscribe(Player character)
    {
         player= character;
        player.playerHealth.OnPlayerTalkingDamage += ExecuteEffect;
        _lastTriggerTime = -float.MaxValue;
        
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Player character)
    {
        if (player != null)
            player.playerHealth.OnPlayerTalkingDamage -= ExecuteEffect;
        player = null;
    }
}
