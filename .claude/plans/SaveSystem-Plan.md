# Save System Plan

## Overview

The save system persists player state to `Application.persistentDataPath/save.json` using `JsonUtility`.
It is bootstrapped at startup by `GameBootstrapper` and registered with `ServiceLocator` so any script can call it via `ServiceLocator.Get<SaveManager>()`.

---

## Step 1 — Core Player State ✅ Done

### What is saved
| Data | Source |
|---|---|
| Scene name | `SceneManager.GetActiveScene().name` |
| Player position (X, Y) | `player.transform.position` |
| Current health | `player.playerHealth.CurrentHealth` |
| Bag inventory (itemId + stackSize per slot) | `PlayerInventory.itemInventoriesList` |
| Equipment slots (slotType + itemId per slot) | `PlayerInventory.equipList` |

### What is NOT saved (intentional)
- **Player stats** — driven entirely by `CharacterData` ScriptableObject + equipped item modifiers, so they are always recalculated on load.
- **Gold / currency** — no currency system exists yet.

### Files created
| File | Purpose |
|---|---|
| `Assets/_Scripts/Save/SaveManager.cs` | Core save/load logic. Registered with ServiceLocator. |
| `Assets/_Scripts/Save/PlayerSaveData.cs` | Top-level serializable save data bag. |
| `Assets/_Scripts/Save/ItemSaveEntry.cs` | One bag-inventory slot (itemId + stackSize). |
| `Assets/_Scripts/Save/EquipSaveEntry.cs` | One equipment slot (slotType enum name + itemId). |
| `Assets/_Scripts/Save/ItemDataRegistry.cs` | ScriptableObject that maps ItemId → ItemData at runtime. |
| `Assets/_Scripts/Save/SaveSystemTester.cs` | Temporary debug helper. Delete after testing. |

### Files modified
| File | Change |
|---|---|
| `GameBootstrapper.cs` | Creates `SaveManager` GameObject as DontDestroyOnLoad at startup. |
| `EntityHealth.cs` | Added `RestoreHealth(float)` — sets HP directly without damage events. |
| `ItemData.cs` | Added `ItemId` (GUID, auto-generated in `OnValidate` when empty). |
| `Portal.cs` | Calls `SaveManager.Save()` before every scene transition (auto-save). |

### How items are identified across saves
Every `ItemData` asset has an `ItemId` string (GUID). It is auto-generated in `OnValidate` the first time Unity loads the asset — you never set it manually. `ItemDataRegistry` maps every `ItemId` to its `ItemData` at runtime so the load path can look up the ScriptableObject by ID.

### ItemDataRegistry setup (editor task, one-time)
1. **Create the asset**: `Assets → Create → RPG → Save → ItemDataRegistry`. Save it inside `Assets/Resources/` (exact folder matters — loaded via `Resources.Load`).
2. **Populate it**: select the asset, right-click the component header in the Inspector → **Refresh All Items**. This scans the whole project for every `ItemData` and fills the list automatically.
3. Re-run **Refresh All Items** any time you add new `ItemData` assets.

### Save trigger
- **Auto-save**: fires when the player walks through any `Portal`.
- **Manual save**: call `ServiceLocator.Get<SaveManager>().Save()` from anywhere (e.g., a pause menu save button).

### Load trigger (⚠ not wired yet — needs a main menu)
- Call `ServiceLocator.Get<SaveManager>().HasSave()` to check if a file exists.
- Call `ServiceLocator.Get<SaveManager>().Load()` to load — this triggers a full scene reload to the saved scene, then restores state one frame after the scene finishes loading.

---

## Step 2 — Skill Tree State 🔲 To Do

### What is saved
| Data | Source |
|---|---|
| Remaining skill points | `UISkillTree._skillPoint` |
| Per-node unlock state | `UITreeNode.isUnlocked` for every node that is unlocked |

`isLocked` does not need to be saved explicitly — it is re-derived during load by letting each restored unlocked node cascade locks to its conflict nodes, exactly as it does during normal gameplay.

### Key for identifying nodes
`skillTreeData.name` (the Unity asset name of the `SkillTreeData` ScriptableObject). This is stable as long as the asset is not renamed.

### Files to create
| File | Purpose |
|---|---|
| `Assets/_Scripts/Save/SkillTreeSaveData.cs` | `float skillPoints` + `List<NodeSaveEntry>`. |
| `Assets/_Scripts/Save/NodeSaveEntry.cs` | `string nodeKey` + `bool isUnlocked`. |

### Files to modify
| File | Change |
|---|---|
| `PlayerSaveData.cs` | Add `public SkillTreeSaveData skillTree` field. |
| `UISkillTree.cs` | Add `public float SkillPoints` getter. Add `public void RestoreSkillPoints(float)` setter (used only by save system on load). |
| `UITreeNode.cs` | Add `public void RestoreUnlocked()` — identical to the private `Unlock()` but skips the `ReduceSkillPoint` call. Called by the save system instead of `Unlock()` so that restoring a node does not double-charge skill points. |
| `SaveManager.cs` | Save: walk `UISkillTree.GetComponentsInChildren<UITreeNode>()`, record unlocked nodes. Load: reset all nodes → restore skill points → call `RestoreUnlocked()` on each saved entry → refresh all `UIConnectedHandler` line colors. |

### Load order for skill tree (inside `ApplyLoadedData`)
1. Get `UISkillTree` via `ServiceLocator.Get<UIManager>().uISkillTree`.
2. Call `node.ResetNode()` on every `UITreeNode` — resets `isUnlocked` and `isLocked` to false without refunding points (points are restored separately).
3. Call `uISkillTree.RestoreSkillPoints(data.skillTree.skillPoints)`.
4. For each `NodeSaveEntry` in save data: find the matching `UITreeNode` by `skillTreeData.name`, call `node.RestoreUnlocked()`.
5. Call `handler.RefreshLineColors()` on every `UIConnectedHandler` to repaint connection lines.

### Why `RestoreUnlocked` instead of calling `Unlock` directly
`UITreeNode.Unlock()` is private and calls `ReduceSkillPoint`. On load we set skill points from the save file first, so calling the normal `Unlock` path would incorrectly subtract points a second time. `RestoreUnlocked` applies the same visual and skill-manager effects without touching the point balance.

---

## Step 3 — Main Menu Continue Button 🔲 To Do

- Add a **Continue** button to the main menu that is only interactable when `SaveManager.HasSave()` returns true.
- Button `onClick` → `ServiceLocator.Get<SaveManager>().Load()`.
- Add a **New Game** button that optionally deletes the save file before loading the first scene.
