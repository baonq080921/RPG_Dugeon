using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a single equipment slot in the inventory UI, matched by item type.
/// </summary>
public class UIEquipmentSlot : UiItemSlotBase
{
    [field: SerializeField] public ItemTypes SlotType { get; private set; }
    public ItemInventoryEquipment ItemEquip { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Refreshes the slot display to reflect the current equipped item.
    /// </summary>
    /// <param name="equipment">The equipment slot data to display.</param>
    public void UpdateUISlot(ItemInventoryEquipment equipment)
    {
        ItemEquip = equipment;
        image.sprite = equipment != null && equipment.HasItem()
            ? equipment.equipItem.itemData.Sprite
            : deafultSpriteSlot;
    }
}
