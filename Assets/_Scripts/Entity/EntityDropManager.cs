using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

public class EntityDrop : MonoBehaviour
{
    [SerializeField] private ItemListData _dropDataList;

    [Header("Drop Limit")]
    [SerializeField] private float maxRarity = 1200;

    [SerializeField] private int maxDropOut = 3;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            DropItems();
    }

    public void DropItems()
    {
        if(_dropDataList == null) return;
        List<ItemData> itemsToDrop = RollRandomDrop();
        Debug.Log(itemsToDrop.Count);
        int dropCount = Mathf.Min(maxDropOut, itemsToDrop.Count);
        for(int i = 0; i < dropCount; i++)
        {
            CreateDropItem(itemsToDrop[i]);
        }
    }

    private void CreateDropItem(ItemData itemData) =>
        ServiceLocator.Get<PoolManager>()?.itemObjectPool?.Spawn(itemData, transform.position);


    public List<ItemData> RollRandomDrop()
    {
        List<ItemData> possibleDrops = new List<ItemData>();
        List<ItemData> finalDrops = new List<ItemData>();
        //Step 1 take random based on the rare and max drop chance
        foreach(var item in _dropDataList.itemDataList)
        {
            if(Random.Range(0f,100f) <= item.GetDropChance())
                possibleDrops.Add(item);
        }

        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRare).ToList();

        //Adding this to the final list until rarity limit on entity is reached
        float remainingRarity = maxRarity;
        foreach(var item in possibleDrops)
        {
            if(remainingRarity > item.itemRare)
            {
                finalDrops.Add(item);
                remainingRarity -= item.itemRare;
            }
        }

        return finalDrops;


    }
}