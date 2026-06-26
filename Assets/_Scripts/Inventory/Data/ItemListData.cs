using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ItemListData-", menuName = "RPG/ItemListData", order = 0)]
    public class ItemListData : ScriptableObject
    {
        public string itemListName;
        public List<ItemData> itemDataList;
    }
}
