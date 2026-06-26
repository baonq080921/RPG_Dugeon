using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Rolls a random loot drop from a drop table and requests the world spawn for each item.
    /// Lives in the Inventory package (loot is an item concern) so the base <c>entity</c> package
    /// never has to reference Inventory; the actual spawn is delegated to the Pool layer via
    /// <see cref="DropItemRequestedEvent"/>.
    /// </summary>
    public class EntityDrop : MonoBehaviour
    {
        [SerializeField] private ItemListData _dropDataList;

        [Header("Drop Limit")]
        [SerializeField] private float maxRarity = 1200;

        [SerializeField] private int maxDropOut = 3;

        // void Update()
        // {
        //     if(Input.GetKeyDown(KeyCode.P))
        //         DropItems();
        // }

        public void DropItems()
        {
            if (_dropDataList == null) return;
            List<ItemData> itemsToDrop = RollRandomDrop();
            Debug.Log(itemsToDrop.Count);
            int dropCount = Mathf.Min(maxDropOut, itemsToDrop.Count);
            for (int i = 0; i < dropCount; i++)
            {
                CreateDropItem(itemsToDrop[i]);
            }
        }

        // Raise a drop request so the Pool layer performs the spawn — keeps this loot component free of
        // any Pool dependency (and, living in Inventory now, free of an entity↔Inventory cycle).
        private void CreateDropItem(ItemData itemData) =>
            EventBus<DropItemRequestedEvent>.Raise(new DropItemRequestedEvent(itemData, transform.position));


        public List<ItemData> RollRandomDrop()
        {
            List<ItemData> possibleDrops = new List<ItemData>();
            List<ItemData> finalDrops = new List<ItemData>();
            //Step 1 take random based on the rare and max drop chance5
            foreach (var item in _dropDataList.itemDataList)
            {
                if (Random.Range(0f, 100f) <= item.GetDropChance())
                    possibleDrops.Add(item);
            }

            possibleDrops = possibleDrops.OrderByDescending(item => item.itemRare).ToList();

            //Adding this to the final list until rarity limit on entity is reached
            float remainingRarity = maxRarity;
            foreach (var item in possibleDrops)
            {
                if (remainingRarity > item.itemRare)
                {
                    finalDrops.Add(item);
                    remainingRarity -= item.itemRare;
                }
            }

            return finalDrops;


        }
    }
}
