# Plan: Skill Unlock Gate System

## Goal
Skills are **locked by default**. A player must unlock the base skill node in the skill tree
before the button becomes usable. Upgrading a skill then changes which variant runs.
The system must scale cleanly to any number of future skills.

---

## How the Current Flow Works

```
SkillButton (UI) → SkillButtonHandler.PressSkill(index) → sets _pending[index] = true
PlayerGroundedState.Update() → TryConsumeSkill(index) → changes state to PlayerDashState
```

`UITreeNode.Unlock()` already calls `PlayerSkillManager.GetSkillByType()` to set
`SkillBase.skillUpgrade`, but nothing currently **blocks** `PressSkill` when a skill is locked.

---

## Proposed Changes (4 files)

### 1. `SkillBase.cs` — add unlock state
- Add `bool IsUnlocked { get; private set; }` defaulting to `false`.
- Add `public void Unlock()` that sets `IsUnlocked = true`.
- `SetSkillUpgradeType` stays as-is (upgrade variant is a separate concern from unlock gate).

```
SkillBase
  + bool IsUnlocked      ← new
  + void Unlock()        ← new
    void SetSkillUpgradeType(SkillUpgrade)   ← existing
    void SetUpgradeForSkill(UpgradeData)     ← existing
```

### 2. `SkillButtonHandler.cs` — register + gate by unlock state
- Add `SkillBase[] _skills` parallel to existing `_states[]`.
- Add `public void RegisterSkill(int index, SkillBase skill)` (called from `Player.CreateStates`).
- In `PressSkill`: before setting `_pending`, check `_skills[index]?.IsUnlocked != true` → early return.
- `EnsureInitialized` initialises `_skills` array alongside existing arrays.

```
SkillButtonHandler
  + SkillBase[] _skills                       ← new array
  + void RegisterSkill(int index, SkillBase)  ← new
  PressSkill → guard: if skill != null && !skill.IsUnlocked → return
```

### 3. `Player.CreateStates()` — wire skill references
After registering states, also register each `SkillBase`:

```csharp
SkillButtonHandler.RegisterSkill((int)ButtonSkillName.Dash,
    ServiceLocator.Get<PlayerSkillManager>().skillDash);
```

> Note: `PlayerSkillManager` registers in `Awake`, `Player.CreateStates` is called in `Awake`
> too — order matters. We resolve this by calling `RegisterSkill` in `Player.Start` instead,
> or by moving `PlayerSkillManager` to register earlier (Script Execution Order).

### 4. `UITreeNode.Unlock()` — call `skill.Unlock()` on every tree unlock
When any node for a skill is unlocked (base *or* upgrade), call `skill.Unlock()` so the gate
opens. The upgrade variant is already handled by `SetSkillUpgradeType`.

```csharp
var skill = ServiceLocator.Get<PlayerSkillManager>().GetSkillByType(skillTreeData.SkillType);
skill.Unlock();                                              // ← new line
skill.SetSkillUpgradeType(skillTreeData.UpgradeData.SkillUpgrade);
skill.SetUpgradeForSkill(skillTreeData.UpgradeData);
```

---

## Skill Upgrade Variants (existing SkillUpgrade enum)

The upgrade enum already encodes what variant to run:

| `SkillUpgrade` value         | Meaning                          |
|------------------------------|----------------------------------|
| `Dash`                       | Base dash (no extra effect)      |
| `Dash_CloneOnStart`          | Spawn clone at dash start        |
| `Dash_CloneOnStartAndArrival`| Clone at start + arrival         |
| `Dash_ShardOnStart`          | Shard at dash start              |
| `Dash_ShardOnStartAndArrival`| Shard at start + arrival         |

`PlayerDashState.Enter()` (or `SkillDash`) reads `skillUpgrade` to branch into the right variant.
No change needed to this enum.

---

## Extensibility for Future Skills

To add a new skill (e.g. `SkillCounter`):
1. Add its `SkillType` and `SkillUpgrade` variants to the enums.
2. Add `ButtonSkillName` entry.
3. Add `SkillBase`-derived component to the player prefab.
4. In `PlayerSkillManager`, expose it as a property and return it from `GetSkillByType`.
5. In `Player.CreateStates`, call `RegisterSkill` for the new slot.
6. Set up `UITreeNode` assets in the skill tree — unlock gate and upgrade are automatic.

No changes to `SkillButtonHandler`, `UITreeNode`, or `SkillBase` are needed for each new skill.

---

## File Change Summary

| File | Change |
|------|--------|
| `SkillBase.cs` | + `IsUnlocked`, + `Unlock()` |
| `SkillButtonHandler.cs` | + `_skills[]`, + `RegisterSkill()`, gate in `PressSkill` |
| `Player.cs` | call `RegisterSkill` for each slot in `Start` |
| `UITreeNode.cs` | call `skill.Unlock()` inside `Unlock()` |

**Out of scope for this plan:** visual feedback on the button (greyed-out icon when locked).
Can be added later by having `SkillButton` observe `SkillBase.IsUnlocked`.
