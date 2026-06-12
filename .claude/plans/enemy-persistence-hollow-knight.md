# Enemy Persistence — Hollow Knight Style

**Goal:** Enemies killed in a dungeon scene stay dead when the player re-enters that scene.
Regular enemies reset only when the player rests at a Save Point. Boss/named enemies never reset.

---

## How It Works (Big Picture)

```
[Scene loads]
    └─► Each EnemySpawner checks WorldStateManager
            ├─ Enemy ID found in dead list → do not spawn
            └─ Enemy ID not found → spawn enemy from EnemyPool

[Enemy dies]
    └─► Enemy raises EnemyDiedEvent (carries SpawnerId + SceneName)
            └─► WorldStateManager listens → stores ID under scene name

[Player rests at SavePoint]
    └─► WorldStateManager.ResetNonPermanent()
            └─► Clears all non-boss dead lists → enemies respawn next entry

[Save to disk / Load from disk]
    └─► WorldStateManager serializes its state to JSON via SaveSystem
```

The key insight (same as Hollow Knight): **enemies don't exist in the scene by default**.
A lightweight `EnemySpawner` GameObject sits in the scene instead. It decides whether
to spawn the enemy at runtime by querying WorldStateManager.

---

## Architecture

### New Scripts

#### 1. `WorldStateManager` — `Assets/_Scripts/System/WorldStateManager.cs`

- Namespace: `System`
- `DontDestroyOnLoad` singleton, registered to `ServiceLocator`
- Initialized by `GameBootstrapper` (before any scene loads)

```
Dictionary<string (sceneName), HashSet<string> (dead spawnerIds)>
```

**Public API:**

| Method | Description |
|--------|-------------|
| `RecordEnemyDeath(string sceneName, string spawnerId)` | Marks enemy as dead for that scene |
| `IsEnemyDead(string sceneName, string spawnerId)` | Returns true if enemy is in dead list |
| `ResetScene(string sceneName)` | Clears dead list for one scene (non-permanent only) |
| `ResetAllNonPermanent()` | Called by SavePoint — clears all non-permanent dead lists |
| `GetState()` / `LoadState(WorldSaveData)` | Serialization for save/load |

**Integration with EventBus:**
`WorldStateManager` subscribes to `EnemyDiedEvent` on Awake and deregisters on OnDestroy.
It reads `event.SpawnerId` and `event.SceneName` and calls `RecordEnemyDeath`.

---

#### 2. `EnemySpawner` — `Assets/_Scripts/Enemies/EnemySpawner.cs`

- Namespace: `Enemies`
- A plain `MonoBehaviour` placed in each scene **instead of** a raw Enemy prefab.
- Has a stable `PersistentId` (GUID string) baked in the editor.

**Serialized Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `PersistentId` | `string` | Unique GUID, baked at edit time via custom editor button |
| `EnemyPrefabType` | `EnemyType` (enum) | Which enemy to spawn (maps to the correct EnemyPool) |
| `IsPermanentDeath` | `bool` | True = never resets even after rest point (use for bosses) |
| `SpawnOffset` | `Vector3` | Offset from spawner position |

**Lifecycle:**

```
Start()
  → worldState = ServiceLocator.Get<WorldStateManager>()
  → if worldState.IsEnemyDead(SceneManager.GetActiveScene().name, PersistentId) → return (stay inactive)
  → enemy = EnemyPool.Spawn(transform.position + SpawnOffset)
  → enemy.SpawnerId = PersistentId
  → enemy.OwningScene = SceneManager.GetActiveScene().name
```

No direct death callback needed — `EnemySpawner` just lets the enemy raise
`EnemyDiedEvent` and lets `WorldStateManager` handle it globally.

---

#### 3. `WorldSaveData` — `Assets/_Scripts/System/WorldSaveData.cs`

- A plain serializable C# class (no MonoBehaviour)
- Contains: `Dictionary<string, List<string>> deadEnemiesByScene`
- Used only for JSON serialization/deserialization

---

### Modified Scripts

#### 4. `GameEvents.cs` — Add fields to `EnemyDiedEvent`

Current:
```csharp
public struct EnemyDiedEvent : IEvent { }
```

After:
```csharp
public struct EnemyDiedEvent : IEvent
{
    public string SpawnerId;   // set by the Enemy when it dies
    public string SceneName;   // set by the Enemy when it dies
}
```

---

#### 5. `Enemy.cs` — Two new properties, modify `Die()`

Add to `Enemy`:
```csharp
public string SpawnerId { get; set; }    // set by EnemySpawner after spawn
public string OwningScene { get; set; } // set by EnemySpawner after spawn
```

Modify `Die()`:
```csharp
protected override void Die()
{
    base.Die();
    EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent
    {
        SpawnerId = SpawnerId,
        SceneName = OwningScene
    });
    ReturnToPool();
}
```

---

#### 6. `GameBootstrapper.cs` — Initialize `WorldStateManager`

In the `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` method, after creating
the existing `Helper` and `CameraResolutionFitter` objects, also create:

```csharp
var wsm = new GameObject("[WorldStateManager]");
wsm.AddComponent<WorldStateManager>();
Object.DontDestroyOnLoad(wsm);
```

This ensures it is available before any scene or enemy loads.

---

#### 7. `EnemyPool.cs` — No structural change needed

`EnemyPool.Spawn()` already returns the `Enemy`. `EnemySpawner` calls it and then
sets `SpawnerId` and `OwningScene` on the returned instance. No changes required.

---

### Optional: Save Point Reset

Any existing or new `SavePoint` MonoBehaviour calls:
```csharp
ServiceLocator.Get<WorldStateManager>().ResetAllNonPermanent();
```
This clears all room dead lists except permanent enemies (bosses).
Enemies will re-appear the next time their scene loads.

---

## File Creation Order

Follow this order to avoid compile errors between steps:

1. `WorldSaveData.cs` — pure data class, no dependencies
2. `GameEvents.cs` — add fields to `EnemyDiedEvent`
3. `WorldStateManager.cs` — depends on EventBus + WorldSaveData
4. `EnemySpawner.cs` — depends on WorldStateManager + EnemyPool
5. `Enemy.cs` — modify Die() and add two properties
6. `GameBootstrapper.cs` — add WorldStateManager initialization

---

## Scene Setup (After Implementation)

**Before:** Enemy prefab placed directly in scene hierarchy.

**After:**
1. Delete the raw Enemy prefabs from the scene.
2. Place an `EnemySpawner` prefab at each enemy position.
3. In the Inspector for each `EnemySpawner`:
   - Click **"Generate ID"** (custom editor button) to bake a GUID into `PersistentId`.
   - Set `EnemyPrefabType` to the correct enemy type.
   - Check `IsPermanentDeath` only for bosses or named enemies.

> **Important:** Never duplicate an EnemySpawner GO in the editor — it will copy the same GUID.
> Always use prefab variant or the Generate ID button after any copy.

---

## Persistence to Disk (Save / Load)

`WorldStateManager` exposes `GetState()` returning a `WorldSaveData`.
Your future `SaveSystem` (or GameManager) serializes this alongside other save data:

```
SaveSystem.Save()
  → collect WorldStateManager.GetState()
  → collect player stats, position, inventory
  → write to JSON file

SaveSystem.Load()
  → read JSON
  → WorldStateManager.LoadState(data.worldSaveData)
  → restore player stats etc.
```

`WorldStateManager` does **not** do its own file I/O — it only manages in-memory state
and hands data to/from the SaveSystem. This keeps concerns separated.

---

## Summary of New/Modified Files

| File | Status | Role |
|------|--------|------|
| `System/WorldStateManager.cs` | **New** | Central dead-enemy registry |
| `System/WorldSaveData.cs` | **New** | Serializable snapshot of WorldStateManager |
| `Enemies/EnemySpawner.cs` | **New** | Scene-placed spawner with stable GUID |
| `Base/GameEvents.cs` | **Modified** | Add `SpawnerId` + `SceneName` to `EnemyDiedEvent` |
| `Enemies/Enemy.cs` | **Modified** | Add `SpawnerId`/`OwningScene`, update `Die()` |
| `Base/GameBootstrapper.cs` | **Modified** | Create WorldStateManager before scene load |
