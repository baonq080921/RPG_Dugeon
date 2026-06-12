
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemListData-", menuName = "RPG/ItemListData", order = 0)]
public class ItemListData : ScriptableObject
{
    public string itemListName;
    public List<ItemData> itemDataList;
}