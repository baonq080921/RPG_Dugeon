# Class Design Details
# RPG Dungeon

**Ngày:** 20/06/2026  
**Phạm vi:** 4 lớp cốt lõi — được chọn vì mỗi lớp đại diện cho một **design domain hoàn toàn khác nhau**

---

## Tiêu chí chọn lớp

| Lớp | Domain | Pattern chính |
|---|---|---|
| `EntityStat` | Computation / Formula engine | Modifier pattern, Diminishing returns |
| `Entity` | Entity root / Component composition | Template Method, Composite, Observer |
| `EventBus<T>` | Decoupled messaging infrastructure | Observer (type-safe), Generic singleton channel |
| `SaveManager` | Persistence / State management | Snapshot, Repository, Coroutine-deferred restore |

> **Thay đổi so với phiên bản trước:**  
> `Player` → `Entity`: Entity là nền tảng thực sự của toàn bộ entity hierarchy; Player chỉ extends Entity với input binding và cooldown.  
> `PlayerInventory` → `SaveManager`: SaveManager đại diện cho domain persistence — domain hoàn toàn chưa được đại diện trong bộ 4 cũ.

---

## 1. EntityStat

> **File:** `Assets/_Scripts/Entity/EntityStat.cs`  
> **Namespace:** *(global)*  
> **Kế thừa:** `MonoBehaviour`  
> **Trách nhiệm:** Tính toán toàn bộ giá trị stat cuối cùng cho một entity — áp dụng công thức diminishing returns, crit, cộng dồn modifier từ trang bị và level-up.

---

### 1.1 Quan hệ phụ thuộc

```
EntityStat
 ├── MajorStats (ScriptableObject)       ← Strength, Agility, Intelligence, Vitality
 ├── OffensiveStats (ScriptableObject)   ← Damage, CritChance, CritPower, AttackMultiplier, ElementalDamage
 ├── DefensiveStats (ScriptableObject)   ← MaxHealth, Armor, Evasion, ElementalResistance, HealthRegen, KnockBackThreshold
 └── Stat (class)                        ← đơn vị stat nhỏ nhất: baseValue + runtime modifiers
```

---

### 1.2 Attributes

| Tên | Kiểu | Visibility | Mô tả |
|---|---|---|---|
| `_entityType` | `EntityType` | `private` | Phân biệt PlayerNormal / PlayerSpecial / Enemy — ảnh hưởng đến loại elemental damage trả về |
| `_majorStats` | `MajorStats` | `private` | SO chứa 4 major stats: STR / AGI / INT / VIT |
| `_offensiveStats` | `OffensiveStats` | `private` | SO chứa các stat tấn công |
| `_defensiveStats` | `DefensiveStats` | `private` | SO chứa các stat phòng thủ |
| `StunDuration` | `float` | `public get / private set` | Thời gian stun khi bị knockback, dùng chung bởi KnockBackState |
| `_strengthPoints` | `int` | `private` | Số điểm đã phân bổ vào STR — track riêng để UI hiển thị |
| `_agilityPoints` | `int` | `private` | *(tương tự)* |
| `_intelligencePoints` | `int` | `private` | *(tương tự)* |
| `_vitalityPoints` | `int` | `private` | *(tương tự)* |
| `K = 100f` | `const float` | `private` | Hệ số scaling cho công thức diminishing returns armor/resistance |

---

### 1.3 Methods

#### Combat Computation (có randomization)

| Method | Trả về | Mô tả |
|---|---|---|
| `GetPhysicalDamageValue(out bool isCrit)` | `float` | Tính sát thương vật lý cuối cùng. Công thức: `(Damage + STR) × critMultiplier?`. Crit chance = `CritChance + AGI×0.3`, capped 100%. `isCrit` = true nếu random hit crit |
| `GetElementalDamageValue(out ElementType elementType)` | `float` | Player trả về Electric damage = `ElementalDmg + INT`. Enemy trả về 0 / None |
| `GetEnvasionValue()` | `float` | Random chance né đòn = `Evasion + AGI×0.5`, capped 85% |

#### Mitigation Computation (không random, deterministic)

| Method | Trả về | Công thức | Mô tả |
|---|---|---|---|
| `GetMigiationValue()` | `float` | `K / (K + Armor + VIT)` | Hệ số nhân vào incoming damage (0→1). Gần 0 = giảm nhiều. Diminishing returns |
| `GetElementalResitanceValue()` | `float` | `K / (K + ElmRes + INT×0.5)` | Tương tự cho elemental damage |
| `GetHealthValue()` | `float` | `MaxHealth + VIT×10` | Max HP |
| `GetAttackMultiplier()` | `float` | `AttackMult + AGI×0.5` | Multiplier animation speed attack |
| `GetHealthRegen()` | `float` | `min(HealthRegen, MaxHP×0.05)` | Lượng HP regen mỗi tick, capped 5% maxHP |
| `GetKnockBackThreshHold()` | `float` | raw stat | Ngưỡng damage ratio để áp heavy vs light knockback |

#### Display Values (cho UI — luôn deterministic)

| Method | Mô tả |
|---|---|
| `GetDamageDisplayValue()` | Damage + STR (không random, để UI hiển thị) |
| `GetCritChanceDisplayValue()` | CritChance + AGI×0.3, cap 100% |
| `GetCritPowerDisplayValue()` | CritPower + STR×0.5 |
| `GetArmorDisplayValue()` | Armor + VIT |
| `GetElementalDamage()` | ElementalDmg + INT |
| `GetElementalResistanceDisplayValue()` | `(1 − mitigation) × 100` → % giảm thiệt hại |

#### Modifier Management

| Method | Mô tả |
|---|---|
| `AddMajorStatPoint(StatType)` | Thêm 1 điểm vào major stat + gọi LevelUp tương ứng để AddModifier vào các stat phụ |
| `GetStatByType(StatType)` | Trả về object `Stat` theo enum — dùng bởi ItemInventory khi equip/unequip trang bị |
| `GetAllocatedPoints(StatType)` | Trả về số điểm đã phân bổ — dùng bởi UI level-up |
| `ResetAllStats()` | Reset toàn bộ runtime modifier về 0, gọi khi Awake và khi SaveManager load |

#### Stat Level-Up Side Effects (private)

| Method | Tác động |
|---|---|
| `StrenghLevelUp()` | `Damage +1`, `CritPower +0.5` |
| `AgilityLevelUp()` | `Evasion +0.5`, `CritChance +0.3` |
| `IntelegenceLevelUp()` | `ElementalDmg +1`, `ElmResistance +0.5` |
| `VitalityLevelUp()` | `MaxHealth +5`, `Armor +1` |

---

### 1.4 Lớp phụ trợ: `Stat`

> **File:** `Assets/_Scripts/Stats/StatData.cs`

```
Stat
 ├── baseValue  : float  (SerializeField — set trong Inspector/SO)
 ├── value      : float  (runtime — tích lũy modifier, NonSerialized)
 └── _initialized : bool (lazy init flag)

GetValue()         → value nếu đã init, else baseValue
AddModifier(v, n)  → value += v
RemoveModifier(v,n)→ value -= v
Reset()            → value = baseValue, _initialized = true
```

> **Lưu ý thiết kế:** `value` không serialize để tránh dữ liệu modifier bị persist vào ScriptableObject asset. `ResetAllStats()` phải được gọi trước khi gameplay bắt đầu.

---

---

## 2. Entity

> **File:** `Assets/_Scripts/Entity/Entity.cs`  
> **Namespace:** *(global)*  
> **Kế thừa:** `MonoBehaviour`  
> **Trách nhiệm:** Lớp nền của mọi game entity (player và enemy). Cung cấp physics foundation, FSM, hit detection gizmo, knockback / shock coroutine, và composition root cho các component con. `Player` và `Enemy` đều kế thừa Entity và chỉ extend thêm behavior riêng của chúng.

---

### 2.1 Quan hệ phụ thuộc

```
Entity (MonoBehaviour)
 ├── StateMachine          ← được tạo trong Awake()
 ├── EntityStat            ← GetComponent trong Awake()
 ├── EntityHealth          ← GetComponent trong Awake()
 ├── EntityVfx             ← GetComponent trong Awake()
 ├── EntityCombat          ← GetComponent trong Awake()
 ├── Rigidbody2D / Collider2D
 └── Animator              ← GetComponentInChildren

Player : Entity            ← thêm input, cooldown, 14 states
Enemy  : Entity            ← thêm detection, patrol, FSM states riêng
```

---

### 2.2 Attributes

#### Physics / Detection

| Field / Property | Kiểu | Mô tả |
|---|---|---|
| `rb` | `Rigidbody2D` | Rigidbody vật lý, public get / private set |
| `col` | `Collider2D` | Collider chính của entity |
| `_groundCheckPoint` | `Transform` | Điểm kiểm tra chạm đất |
| `_groundCheckRadius` | `float` | Bán kính OverlapCircle kiểm tra đất (0–1) |
| `_wallCheckDistance` | `float` | Độ dài Raycast kiểm tra tường (0–10) |
| `_whatIsGround` / `_whatIsWall` | `LayerMask` | Layer masks cho ground/wall check |
| `isGrounded` | `bool` | Kết quả kiểm tra mỗi frame |
| `isTouchingWall` | `bool` | Kết quả raycast tường mỗi frame |
| `direction` | `float` | 1 = phải, -1 = trái. Dùng bởi Flip, knockback, wall check |

#### State Flags

| Field / Property | Kiểu | Mô tả |
|---|---|---|
| `isDead` | `bool` | Set true trong `Die()`, dùng bởi states để tránh transition |
| `IsKnocked` | `bool` | True trong thời gian knockback coroutine đang chạy — `SetVelocity` bị block |
| `IsShocked` | `bool` | True trong thời gian shock coroutine — `SetVelocity` bị block |


#### Component References (public get / private set)

| Property | Mô tả |
|---|---|
| `stateMachine` | FSM, khởi tạo trong Awake |
| `animator` | GetComponentInChildren — animator nằm trên child object |
| `entityStat` | Stat engine |
| `entityHealth` | HP / TakeDamage handler |
| `entityVfx` | VFX controller |
| `entityCombat` | Hitbox / attack executor |

#### Events

| Event | Kiểu | Mô tả |
|---|---|---|
| `OnFlip` | `event Action` | Raised khi `Flip()` được gọi — HealthBar lắng nghe để flip UI ngược lại |

---

### 2.3 Methods

#### Core (virtual — subclass override)

| Method | Mô tả |
|---|---|
| `SetVelocity(Vector2)` *(virtual)* | Set `rb.velocity`. Early-return nếu `IsKnocked` hoặc `IsShocked`. `Player` override thêm `HandleFlip` |
| `Die()` *(virtual)* | Set `isDead = true`. `Player` override raise `PlayerDiedEvent`; `Enemy` override gọi `DropItems` và raise `EnemyDiedEvent` |
| `ApplyKnockBack(float damage)` *(virtual)* | Chọn `_knockBackPowerLight` vs `_knockBackPowerHeavy` dựa trên tỉ lệ `damage/maxHP` so `GetKnockBackThreshHold()`. `Player` override bỏ qua nếu ratio < 0.5f |
| `ApplyEffect(float, ElementType)` *(virtual)* | Hook cho elemental status. `Enemy` override: Ice giảm `moveSpeed` |
| `ResetEffect()` *(virtual)* | Khôi phục moveSpeed hoặc các effect đã áp |

#### Coroutine-based State Control

| Method | Mô tả |
|---|---|
| `Shock(float duration)` | Khởi `ShockCo` — set `IsShocked = true`, zero velocity, sau `duration` giây restore |
| `ApplyKnockBack` → `ReciveKnockBack` → `KnockBackCo` | Chain riêng cho knockback: tung ra `knockBack` velocity, sau `stunDuration` giây clear `IsKnocked` |
| `ResetKnockbackState()` *(protected)* | Cancel coroutine ngay lập tức — gọi khi enemy return to pool |

#### Utility

| Method | Mô tả |
|---|---|
| `CheckGrounded()` | `OverlapCircle` tại `_groundCheckPoint` — gọi mỗi frame trong `Update` |
| `CheckWall()` | `Raycast` theo hướng `direction` — gọi mỗi frame trong `Update` |
| `Flip(float direction)` | Scale âm X để lật sprite, raise `OnFlip` |
| `TriggerAnimationEvent()` | Bridge từ `EntityAnimationEvent` → `stateMachine.currentState.TriggerAnimation()` |
| `SetDirection(float)` | Cập nhật `direction` — gọi bởi `Enemy.FacePlayer()` |
| `ResetAllAnimatorBools()` *(protected)* | Set toàn bộ Animator bool parameters về false — dùng khi reset enemy từ pool |

---

### 2.4 Design Patterns sử dụng

| Pattern | Nơi dùng |
|---|---|
| **Template Method** | `Awake / Start / Update / Die / SetVelocity / ApplyKnockBack` — base define flow, subclass override phần cụ thể |
| **Composite (Component)** | Entity là root; `EntityStat`, `EntityHealth`, `EntityVfx`, `EntityCombat` là component độc lập được lấy qua `GetComponent` |
| **Observer** | `OnFlip` event — HealthBar subscribe để flip UI mà không cần Entity biết HealthBar tồn tại |
| **State Machine** | `stateMachine.currentState.Update()` trong `Update()` — toàn bộ behavior điều khiển qua state |

---

---

## 3. EventBus\<T\>

> **File:** `Assets/_Scripts/Base/EventBus.cs`  
> **Namespace:** `Base`  
> **Kế thừa:** *(không kế thừa gì — pure C# static generic class)*  
> **Trách nhiệm:** Hệ thống messaging toàn cục, type-safe, không coupling. Mỗi event type T có một channel độc lập.

---

### 3.1 Thiết kế tổng quan

```
EventBus<PlayerDiedEvent>  ← channel riêng cho PlayerDiedEvent
EventBus<EnemyDiedEvent>   ← channel riêng cho EnemyDiedEvent
EventBus<AlertNotiEvent>   ← channel riêng cho AlertNotiEvent
... (một channel per T)
```

**Publisher không biết Subscriber tồn tại. Subscriber không cần reference đến Publisher.**

---

### 3.2 Attributes

| Field | Kiểu | Mô tả |
|---|---|---|
| `_bindings` | `static HashSet<IEventBinding<T>>` | Tập hợp tất cả listener đã đăng ký cho type T. Static per generic type → mỗi T có HashSet riêng |

---

### 3.3 Methods

| Method | Mô tả |
|---|---|
| `Register(EventBinding<T>)` | Thêm binding vào `_bindings`. Gọi trong `OnEnable()` |
| `Deregister(EventBinding<T>)` | Xóa binding. Gọi trong `OnDisable()` để tránh memory leak và stale reference |
| `Clear()` | Xóa toàn bộ binding — gọi khi unload scene tránh MonoBehaviour bị destroy vẫn còn trong set |
| `Raise(T @event)` | Snapshot `_bindings` rồi invoke. **Snapshot quan trọng** — tránh `InvalidOperationException` khi handler tự deregister trong lúc dispatch |

---

### 3.4 Lớp phụ trợ: `EventBinding<T>`

```csharp
EventBinding<T> : IEventBinding<T>
 ├── _onEvent      : Action<T>   ← callback nhận payload event
 └── _onEventNoArgs: Action      ← callback không cần payload (tiện dụng)

// IEventBinding<T> là internal interface — EventBus dùng để gọi mà không expose setter ra ngoài
```

**Cách sử dụng đúng:**

```csharp
// Subscribe (OnEnable)
_binding = new EventBinding<PlayerDiedEvent>(OnPlayerDied);
EventBus<PlayerDiedEvent>.Register(_binding);

// Unsubscribe (OnDisable)
EventBus<PlayerDiedEvent>.Deregister(_binding);

// Raise (anywhere)
EventBus<PlayerDiedEvent>.Raise(new PlayerDiedEvent());
```

---

### 3.5 Tất cả Events hiện có trong project

| Event Struct | Payload | Ai raise | Ai lắng nghe |
|---|---|---|---|
| `PlayerDiedEvent` | *(none)* | `Player.Die()` | `PlayerInventory` (drop items) |
| `EnemyDiedEvent` | `EnemyLevel`, `BaseExp` | `Enemy.Die()` | `PlayerLevel` (XP gain) |
| `PlayerLevelUpEvent` | `EntityStat` | `PlayerLevel` | Level-up UI |
| `PlayerXPChangedEvent` | `currentExp`, `expToNext`, `level` | `PlayerLevel` | XP bar UI |
| `EquipEvent` | `ItemInventory` | Inventory UI | `PlayerInventory.TryToEquipEvent()` |
| `OnInventoryChangedEvent` | *(none)* | `InventoryBase` (add/remove/equip) | Inventory UI, Shop UI |
| `StoreCallEvent` | *(none)* | `NPCShop` | `UI_Shop` (mở UI) |
| `CraftStoreCallEvent` | *(none)* | `NPCMerchant` | `UI_StoreMerchant` |
| `StoreItemGetInfoEvent` | `ItemData` | `UI_ShopSlot.OnPointerDown` | `UI_ShopItemDetails` |
| `CraftGetInfoEvent` | `ItemCraftData` | Craft slot click | `UI_ItemDetailsSlot` |
| `AlertNotiEvent` | `message`, `position`, `color` | Nhiều nơi | `UIAlert` (hiển thị popup) |
| `TargetGotHitEvent` | `Transform target`, `isCrit` | `EntityCombat` | Hit VFX system |
| `GamePauseChangedEvent` | `IsPause` | `GameManager` | `Player` (disable input) |
| `QuestStartedEvent` | `QuestData` | `SceneQuestController` | Quest UI |
| `QuestProgressEvent` | `QuestData` | `SceneQuestController` | Quest UI |
| `QuestCompletedEvent` | `QuestData` | `SceneQuestController` | Quest UI |
| `NpcRescuedEvent` | *(none)* | `RescuableNpc` | Quest tracker |
| `SkillPointRewardEvent` | `Amount` | Chests / rewards | `PlayerInventory.AddSkillPoints()` |
| `CameraZoneChangedEvent` | `Collider2D` | Zone trigger | Camera Confiner |
| `PlayerCheckPointEvent` | *(none)* | CheckPoint | SaveManager |
| `ResetStats` | *(none)* | Skill tree reset | `EntityStat.ResetAllStats()` |
| `PlayerAddHealthAmount` | `Amount` | Skill/item effect | `PlayerHealth` |

---

### 3.6 Design Patterns sử dụng

| Pattern | Mô tả |
|---|---|
| **Observer** | Publisher/Subscriber hoàn toàn decoupled |
| **Generic singleton channel** | `static HashSet` per type T — CLR tạo một instance riêng cho mỗi T |
| **Snapshot dispatch** | Tránh concurrent modification trong khi Raise đang chạy |

---

---

## 4. SaveManager

> **File:** `Assets/_Scripts/Save/SaveManager.cs`  
> **Namespace:** `Save`  
> **Kế thừa:** `MonoBehaviour`  
> **Trách nhiệm:** Toàn bộ persistence layer của game — ghi/đọc save file JSON, giữ snapshot in-memory khi player đi qua portal, quản lý trạng thái scene (enemy đã kill, chest đã mở), quest progress. Được tạo bởi `GameBootstrapper` với `DontDestroyOnLoad`.

---

### 4.1 Lifecycle và khởi tạo

```
[RuntimeInitializeOnLoadMethod] GameBootstrapper.Initialize()
    └── Instantiate SaveManager GameObject → DontDestroyOnLoad
            └── Awake() → load ItemDataRegistry, Register<SaveManager> vào ServiceLocator
```

`SaveManager` không gắn vào scene — nó sống suốt vòng đời game và là singleton truy cập qua `ServiceLocator.Get<SaveManager>()`.

---

### 4.2 Attributes

| Field | Kiểu | Mô tả |
|---|---|---|
| `SavePath` | `string` (static property) | `Application.persistentDataPath/save.json` |
| `_registry` | `ItemDataRegistry` | SO registry ánh xạ `itemId` → `ItemData` — dùng khi restore inventory |
| `_pendingLoad` | `PlayerSaveData` | Dữ liệu đọc từ disk, giữ tạm cho đến khi scene mới Awake/Start xong |
| `_transitionSnapshot` | `PlayerSaveData` | Snapshot in-memory khi portal load — không ghi disk |
| `_completedQuestScenes` | `HashSet<string>` | Tên scene có quest đã hoàn thành |
| `_sceneStates` | `Dictionary<string, SceneStateData>` | Per-scene: enemy đã kill, chest đã mở |
| `_questProgress` | `Dictionary<string, int>` | Tiến độ số của quest theo scene |
| `HasTransitionSnapshot` | `bool` (public) | True khi có snapshot portal đang chờ apply |

---

### 4.3 Methods

#### Save

| Method | Mô tả |
|---|---|
| `Save()` | Entry point — `FindObjectOfType<Player>()` → `BuildSaveData()` → `WriteToDisk()`. Gọi từ CheckPoint |
| `SnapshotForTransition(Player)` | Gọi bởi `SceneTransitionManager` trước khi unload scene cũ. Capture state vào `_transitionSnapshot`, **không ghi disk** |
| `BuildSaveData(Player)` *(private)* | Thu thập: position, HP, level/XP, stat points, inventory list, equipment slots, skill tree nodes, quest data, scene states |
| `WriteToDisk(PlayerSaveData)` *(private)* | `JsonUtility.ToJson` → `File.WriteAllText(SavePath)` |

#### Load — Full disk restore

| Method | Mô tả |
|---|---|
| `HasSave()` | `File.Exists(SavePath)` |
| `Load()` | Đọc JSON → `_pendingLoad`, đăng ký `SceneManager.sceneLoaded` → `LoadScene(sceneName)` |
| `OnSceneLoaded` *(private)* | Deregister event, `StartCoroutine(ApplyAfterFullLoad())` |
| `ApplyAfterFullLoad()` *(IEnumerator)* | `yield return null` (chờ Awake/Start), tìm Player + Inventory, gọi `ApplyData(..., restorePosition: true)` |

#### Load — Portal transition restore

| Method | Mô tả |
|---|---|
| `RestoreAfterTransition(Player)` *(IEnumerator)* | Gọi bởi `SceneTransitionManager` sau khi scene mới load xong. Apply `_transitionSnapshot`, **không restore position** (scene spawn point xử lý) |

#### Shared restore logic

| Method | Mô tả |
|---|---|
| `ApplyData(data, player, inventory, restorePosition)` *(IEnumerator, private)* | Core restore: HP, level/XP, `ResetAllStats()` + replay major stat points, inventory items, equipment + modifiers, skill tree nodes, quest/scene state. Raise `OnInventoryChangedEvent` sau khi xong |

#### Scene state management

| Method | Mô tả |
|---|---|
| `MarkEnemyKilled(sceneName, enemyId)` | Ghi `enemyId` vào `_sceneStates[sceneName].killedEnemyIds` |
| `MarkChestOpened(sceneName, chestId)` | Tương tự cho chest |
| `GetSceneState(sceneName)` | Trả về `SceneStateData` hoặc null — `SceneEntityManager` dùng để hide enemy/chest đã xử lý |

#### Quest management

| Method | Mô tả |
|---|---|
| `MarkQuestComplete(sceneName)` | Thêm vào `_completedQuestScenes` |
| `IsQuestComplete(sceneName)` | Check `_completedQuestScenes.Contains` |
| `SetQuestProgress(sceneName, int)` | Cập nhật tiến độ quest số |
| `GetQuestProgress(sceneName)` | Trả về progress, mặc định 0 |

#### New Game

| Method | Mô tả |
|---|---|
| `StartNewGame(startSceneName)` | Clear tất cả in-memory state, xóa save file nếu có, `LoadScene(startSceneName)` |

---

### 4.4 Lớp phụ trợ: `PlayerSaveData`

```
PlayerSaveData
 ├── sceneName, posX, posY          ← vị trí và scene
 ├── currentHealth                  ← HP hiện tại
 ├── currentExp, currentLevel, expToNextLevel
 ├── strengthPoints, agilityPoints, intelligencePoints, vitalityPoints
 ├── inventory  : List<ItemSaveEntry>    ← { itemId, stackSize }
 ├── equipment  : List<EquipSaveEntry>   ← { slotType, itemId }
 ├── skillTree  : SkillTreeSaveData      ← { skillPoints, List<NodeSaveEntry> }
 ├── completedQuestScenes : List<string>
 ├── sceneStates : List<SceneStateData>  ← { sceneName, killedEnemyIds, openedChestIds }
 └── questProgress : List<QuestProgressEntry> ← { sceneName, progress }
```

> **Lưu ý thiết kế:** Stat points được lưu dưới dạng **số điểm phân bổ** (int), không lưu giá trị stat tuyệt đối. Khi load, `ResetAllStats()` được gọi trước, rồi `AddMajorStatPoint()` replay lại đúng số lần — đảm bảo công thức side-effect luôn nhất quán.

---

### 4.5 Design Patterns sử dụng

| Pattern | Mô tả |
|---|---|
| **Snapshot** | `_transitionSnapshot` capture state trước khi scene unload, apply sau khi scene mới load — tách biệt save-on-disk và save-in-memory |
| **Repository** | `_sceneStates`, `_completedQuestScenes`, `_questProgress` là các in-memory stores với CRUD operations |
| **Coroutine-deferred restore** | `ApplyAfterFullLoad()` yield 1 frame sau scene load để đảm bảo Awake/Start của mọi object trong scene đã chạy xong trước khi restore |
| **Service Locator** | `ServiceLocator.Register<SaveManager>(this)` trong Awake — toàn bộ code truy cập qua `ServiceLocator.Get<SaveManager>()` |

---

## Tóm tắt quan hệ giữa 4 lớp

```
EventBus<T>
    ↑ raise / ↓ receive
    │
Entity ←────────────────────────────────────────────────────────┐
│ StateMachine, rb, col, animator                                │
│ IsKnocked / IsShocked / isDead                                 │
│ KnockBack / Shock coroutines                                   │
│ CheckGrounded / CheckWall mỗi frame                            │
│                                                                │
├── Player (extends Entity)              SaveManager ────────────┤
│    input binding, 14 states             │ BuildSaveData()      │
│    cooldown timers                      │ ApplyData()          │
│    Die() → raise PlayerDiedEvent        │ SceneState tracking  │
│                                         │ Portal snapshot       │
├── Enemy (extends Entity)               │                       │
│    detection, patrol, FSM              │                       │
│    Die() → raise EnemyDiedEvent        │                       │
│                                         ▼                      │
└──────────────── EntityStat ────────────────────────────────────┘
                    │ GetPhysicalDamageValue() → EntityCombat
                    │ GetMigiationValue()       → EntityHealth.TakeDamage
                    │ AddMajorStatPoint()       → SaveManager.ApplyData (replay)
                    └── MajorStats / OffensiveStats / DefensiveStats (SO)
```
