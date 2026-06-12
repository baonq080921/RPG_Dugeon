# Save System — Step 1: Player Info (Position + Inventory)

## Goal
Persist the player's world position, current health, bag inventory, and equipped items
across sessions using a JSON file on disk.

---

## Problem: ItemData Has No Stable ID

`ItemData` is a ScriptableObject. A save file cannot store the object itself — only a
string reference back to it. `ItemName` is not safe to use as a key (renaming it silently
breaks saves). A dedicated `ItemId` field must be added and treated as an immutable primary key.

---

## Files to Create / Modify

| File | Action | Purpose |
|---|---|---|
| `Assets/_Scripts/Inventory/Data/ItemData.cs` | **Modify** | Add `ItemId` string field |
| `Assets/_Scripts/Save/ItemDataRegistry.cs` | **Create** | ScriptableObject — maps `ItemId → ItemData` |
| `Assets/Resources/ItemDataRegistry.asset` | **Create** | Singleton asset loaded via `Resources.Load` |
| `Assets/_Scripts/Save/PlayerSaveData.cs` | **Create** | Plain serializable data structs |
| `Assets/_Scripts/Save/SaveManager.cs` | **Create** | Save / Load logic |
| `Assets/_Scripts/Base/GameBootstrapper.cs` | **Modify** | Register `SaveManager` via `ServiceLocator` |

---

## Data Structures

```csharp
// PlayerSaveData.cs
[Serializable]
public class PlayerSaveData
{
    public string sceneName;       // scene to reload on load
    public float  posX, posY;      // world position
    public float  currentHealth;
    public List<ItemSaveEntry>  inventory;   // bag slots
    public List<EquipSaveEntry> equipment;   // equip slots
}

[Serializable]
public class ItemSaveEntry
{
    public string itemId;
    public int    stackSize;
}

[Serializable]
public class EquipSaveEntry
{
    public string slotType;   // EquipSlotType enum name
    public string itemId;     // empty string = slot is empty
}
```

---

## ItemDataRegistry

A `ScriptableObject` placed in `Assets/Resources/` so it can be loaded at runtime
without an Addressables or AssetBundle setup.

```csharp
// ItemDataRegistry.cs
[CreateAssetMenu(fileName = "ItemDataRegistry", menuName = "RPG/ItemDataRegistry")]
public class ItemDataRegistry : ScriptableObject
{
    [SerializeField] private List<ItemData> _items;

    private Dictionary<string, ItemData> _map;

    public void Initialize()  // called once by SaveManager on startup
    {
        _map = new Dictionary<string, ItemData>();
        foreach (var item in _items)
            if (!string.IsNullOrEmpty(item.ItemId))
                _map[item.ItemId] = item;
    }

    public ItemData Get(string itemId)
        => _map.TryGetValue(itemId, out var data) ? data : null;
}
```

Every `ItemData` asset in the project must be added to this list in the Inspector.

---

## SaveManager

Registered with `ServiceLocator` so any system can trigger Save/Load.
Save path: `Application.persistentDataPath + "/save.json"`.

### Save flow

```
1. Collect player world position (Player.transform.position)
2. Collect Player.playerHealth.CurrentHealth
3. Collect SceneManager.GetActiveScene().name
4. Map PlayerInventory.itemInventoriesList  → List<ItemSaveEntry>
5. Map PlayerInventory.equipList            → List<EquipSaveEntry>
6. JsonUtility.ToJson(PlayerSaveData)       → write to save.json
```

### Load flow

```
1. Read save.json → JsonUtility.FromJson<PlayerSaveData>
2. SceneManager.LoadScene(data.sceneName)
3. Wait for scene load (subscribe to SceneManager.sceneLoaded once)
4. Set Player.transform.position = (data.posX, data.posY)
5. Set PlayerHealth.CurrentHealth = data.currentHealth (needs setter)
6. foreach ItemSaveEntry  → registry.Get(itemId) → PlayerInventory.AddToInventory
7. foreach EquipSaveEntry → registry.Get(itemId) → PlayerInventory.EquipItem
```

---

## GameBootstrapper Change

```csharp
// Inside Initialize():
var saveManager = new GameObject(nameof(SaveManager));
saveManager.AddComponent<SaveManager>();
Object.DontDestroyOnLoad(saveManager);
```

`SaveManager.Awake()` loads the registry and registers itself with `ServiceLocator`.

---

## Implementation Order

1. **Add `ItemId` to `ItemData`** — one-line change, do first so assets can be filled in.
2. **Create `ItemDataRegistry.cs`** + generate the `.asset` in `Resources/`.
3. **Create `PlayerSaveData.cs`** — pure data, no Unity dependencies.
4. **Create `SaveManager.cs`** — Save() then Load(), one method at a time.
5. **Wire into `GameBootstrapper`** — register via ServiceLocator.
6. **Test** — call `SaveManager.Save()` on a test button, quit, call `Load()` on start.

---

## Out of Scope for Step 1
- Multiple save slots
- Auto-save on room transition
- Saving enemy/chest state
- Saving quest/story flags
- Encryption
