using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Save
{
    /// <summary>
    /// Maps every <see cref="ItemData.ItemId"/> to its ScriptableObject at runtime.
    /// Place the generated asset at Assets/Resources/ItemDataRegistry.asset so it can
    /// be loaded without Addressables.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDataRegistry", menuName = "RPG/Save/ItemDataRegistry")]
    public class ItemDataRegistry : ScriptableObject
    {
        [SerializeField] private List<ItemData> _items = new List<ItemData>();

        private Dictionary<string, ItemData> _map;

#if UNITY_EDITOR
        /// <summary>
        /// Scans the entire project for every <see cref="ItemData"/> asset and rebuilds the list.
        /// Run this from the context-menu whenever items are added or removed.
        /// </summary>
        [ContextMenu("Refresh All Items")]
        private void RefreshAllItems()
        {
            _items.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item != null)
                    _items.Add(item);
            }
            EditorUtility.SetDirty(this);
            Debug.Log($"[ItemDataRegistry] Refreshed — {_items.Count} items found.");
        }
#endif

        /// <summary>Builds the lookup dictionary. Called once by <see cref="SaveManager"/> on startup.</summary>
        public void Initialize()
        {
            _map = new Dictionary<string, ItemData>();
            foreach (var item in _items)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.ItemId))
                {
                    Debug.LogWarning($"[ItemDataRegistry] '{item.name}' has no ItemId — it will not be restorable from a save file.");
                    continue;
                }
                if (_map.ContainsKey(item.ItemId))
                {
                    Debug.LogWarning($"[ItemDataRegistry] Duplicate ItemId '{item.ItemId}' on '{item.name}'. First entry wins.");
                    continue;
                }
                _map[item.ItemId] = item;
            }
        }

        /// <summary>Returns the <see cref="ItemData"/> for <paramref name="itemId"/>, or null if not found.</summary>
        public ItemData Get(string itemId)
        {
            if (_map == null) Initialize();
            return _map.TryGetValue(itemId, out var data) ? data : null;
        }
    }
}
