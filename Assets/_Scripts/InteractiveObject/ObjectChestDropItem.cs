using UnityEngine;

public class ObjectChestDropItem : ObjectChestBase
{
    
    [SerializeField]private EntityDrop _entityDrop;


    protected override void DropChestItem()
    {
        base.DropChestItem();
        _entityDrop.DropItems();
    }
    
}