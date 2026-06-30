

using Base;
using player;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectDemon", menuName ="RPG/ItemEffectDemon", order = 2)]
public class ItemEffectDemon : ItemEffectData
{
    private EventBinding<PlayerLastAttackEvent> _eventBinding;
    [SerializeField] private DemonEffect _demonEffectPrefab;
    [SerializeField] private float _scaleFactor = 1.2f;
    [field:SerializeField] public ElementType elementType;
  
    protected override void ExecuteEffect()
    {
        base.ExecuteEffect();
        float playerElementalDamage = player.entityStat.GetElementalDamageValue(out _);
        var effect = Instantiate(_demonEffectPrefab,new Vector2(player.transform.position.x + 1.21f,player.transform.position.y),Quaternion.identity);
        effect.SetUpDemonEffect(playerElementalDamage,player.direction,_scaleFactor,elementType);
        Destroy(effect.gameObject, 1f);
    }


    public override void Subscribe(Player character)
    {
        if (_eventBinding != null) return; // Already subscribed; avoid leaking a second binding into the bus.
        base.Subscribe(character);
        player = character;
        _eventBinding = new EventBinding<PlayerLastAttackEvent>(ExecuteEffect);
        EventBus<PlayerLastAttackEvent>.Register(_eventBinding);
    }

    public override void Unsubscribe(Player character)
    {
        base.Unsubscribe(character);
        if (_eventBinding == null) return;
        EventBus<PlayerLastAttackEvent>.Deregister(_eventBinding); // Deregister BEFORE clearing the reference.
        _eventBinding = null; // Reset so a later Subscribe can register again.
    }
}