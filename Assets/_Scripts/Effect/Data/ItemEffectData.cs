using player;
using UnityEngine;

/// <summary>Base ScriptableObject for item effects that trigger on game events.</summary>
public class ItemEffectData : ScriptableObject
{
    protected Player player;
    [field:TextArea]
    [field:SerializeField] public string effectInfo{get ;private set;}
    protected virtual void ExecuteEffect() {}

    /// <summary>Subscribes this effect to the relevant events for <paramref name="player"/>.</summary>
    public virtual void Subscribe(Player character) {}

    /// <summary>Unsubscribes this effect from all events. Call when the item is unequipped.</summary>
    public virtual void Unsubscribe(Player character) {}
}
