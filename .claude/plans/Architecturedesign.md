# ARCHITECTURE DESIGN
# RPG Dungeon — Software Architecture Document

**Phiên bản:** 1.0  
**Ngày:** 19/06/2026  
**Engine:** Unity (Universal Render Pipeline)  
**Nền tảng:** Mobile — Android / iOS

---

## MỤC LỤC

1. [Kiến trúc tổng thể](#1-kiến-trúc-tổng-thể)
2. [Package Diagram — Cấu trúc thư mục & Namespace](#2-package-diagram--cấu-trúc-thư-mục--namespace)
3. [Package Dependency Diagram](#3-package-dependency-diagram)
4. [Class Diagram — Chi tiết từng package](#4-class-diagram--chi-tiết-từng-package)

---

## 1. KIẾN TRÚC TỔNG THỂ

### Loại kiến trúc
Project sử dụng **Layered Architecture (4 tầng)** kết hợp **Event-Driven Communication**, không theo MVC/MVP thuần túy do đặc thù của Unity engine (MonoBehaviour vừa là logic vừa là component trên GameObject).

### 4 Tầng kiến trúc

```
┌─────────────────────────────────────────────────────┐
│              LAYER 4 — Presentation                 │
│   UI/, VFX (EntityVfx), AnimationEvent, BackGround  │
├─────────────────────────────────────────────────────┤
│              LAYER 3 — Domain / Logic               │
│   Entity, Player, Enemy, StateMachine,              │
│   Combat, Skills, Inventory, Quest, Stats           │
├─────────────────────────────────────────────────────┤
│              LAYER 2 — Infrastructure               │
│   EventBus, ServiceLocator, ObjectPool,             │
│   SaveManager, GameBootstrapper, Pool               │
├─────────────────────────────────────────────────────┤
│              LAYER 1 — Data                         │
│   ScriptableObjects: CharacterData, EnemyData,      │
│   ItemData, QuestData, SkillBaseDefinition...       │
│   Save structs: PlayerSaveData, ItemSaveEntry...    │
└─────────────────────────────────────────────────────┘
```

### Các Design Pattern đi kèm

| Pattern | Hiện diện | Ví dụ cụ thể |
|---|---|---|
| **Layered Architecture** | Chính | 4 tầng như trên |
| **Event-Driven** | Cross-cutting | `EventBus<T>` dùng xuyên tất cả tầng |
| **State Machine** | Trong Domain layer | `StateMachine` cho Player, Enemy, Boss |
| **Service Locator** | Trong Infrastructure | `ServiceLocator.Get<SaveManager>()` |
| **Object Pool** | Trong Infrastructure | `ObjectPool<T>` cho Enemy, VFX, Skills |
| **Data-Driven Design** | Trong Data layer | ScriptableObject cho mọi config |
| **Component-Based** | Unity native | MonoBehaviour composition |

---

## 2. PACKAGE DIAGRAM — Cấu trúc thư mục & Namespace

```
Assets/_Scripts/
│
├── Base/              (namespace: Base)         ← Foundation utilities
├── StateMachine/      (namespace: stateMachine) ← Generic FSM
├── Interfaces/        (namespace: Interfaces)   ← Contracts
├── Enum/              (global)                  ← Shared enums
├── Stats/             (namespace: Stats)        ← Stat ScriptableObjects
│
├── Entity/            (global)                  ← Core entity abstraction
│
├── Player/            (namespace: player)
│   ├── PlayerBase/    ← Player MonoBehaviours
│   ├── PlayerState/   ← All player FSM states
│   ├── PlayerSkill/   ← Skill logic & skill objects
│   │   └── Domain-Skill/
│   ├── PlayerInventory/
│   └── Data/          ← CharacterData, SkillBaseDefinition
│
├── Enemies/           (namespace: enemy)
│   ├── EnemyState/    ← Shared enemy FSM states
│   ├── EnemySkeleton/
│   ├── EnemySlime/
│   ├── EnemyBoss/     ← EnemyDeathRippler + states
│   └── Data/          ← EnemyData
│
├── Quest/             (namespace: Quest)
├── Inventory/         (global)
│   └── Data/          ← ItemData, EquipmentData, ItemCraftData
├── Save/              (namespace: Save)
├── NPC/               (namespace: NPC)
├── Store/             (global)
├── Effect/            (global)
│   └── Data/          ← ItemEffectData variants
│
├── UI/                (namespace: UI)
│   ├── UISkillTree/
│   ├── UI_Inventory/
│   ├── UILevelUp/
│   └── UI_StoreMerchant/
│
├── Scene/             (namespace: scene)
├── System/            (namespace: System)
├── Sound/             (global)
├── Pool/              (global)
├── AnimationEvent/    (global)
├── Trap/              (global)
├── InteractiveObject/ (global)
└── BackGround/        (global)
```

---

## 3. PACKAGE DEPENDENCY DIAGRAM

### Sơ đồ phụ thuộc

```
[ Enum ]  [ Interfaces ]  [ Base ]  [ StateMachine ]
   ↑            ↑            ↑             ↑
   └────────────┴────────────┴─────────────┘
                             │
                        [ Entity ] ←──── [ Stats ]
                        /       \
                       ↓         ↓
                  [ Player ]   [ Enemy ]
                       │
                  [ Inventory ] ←── [ Effect ]
                       │         ↖── [ Store ]
                  [ Quest ]
                       │
                  [ Save ] ←──────────────┐
                       ↑                  │
                  [ UI ] ──────────────────┤
                  [ NPC ] ────────────────►│
                  [ Scene ] ──────────────►│
                  [ System ]
                  [ Pool ]
```

### Bảng phụ thuộc chi tiết

| Package | Phụ thuộc vào |
|---|---|
| `Base` | _(không phụ thuộc ai)_ |
| `Interfaces` | _(không phụ thuộc ai)_ |
| `Enum` | _(không phụ thuộc ai)_ |
| `StateMachine` | _(không phụ thuộc ai)_ |
| `Stats` | `Enum` |
| `Entity` | `Base`, `Interfaces`, `StateMachine`, `Stats`, `Enum` |
| `Player` | `Entity`, `Base`, `Interfaces`, `Stats`, `Inventory`, `Save`, `Effect`, `Enum` |
| `Enemy` | `Entity`, `Base`, `Interfaces`, `Stats`, `Enum` |
| `Inventory` | `Stats`, `Effect`, `Enum` |
| `Quest` | `Base`, `Save` |
| `Save` | `Player`, `Inventory`, `Quest`, `UI` |
| `UI` | `Base`, `Player`, `Inventory`, `Quest`, `Stats` |
| `NPC` | `Base`, `Quest` |
| `Scene` | `Base`, `Save`, `Player` |
| `Pool` | `Player`, `Enemy`, `Base` |
| `Effect` | `Base` |
| `Store` | `Inventory` |
| `System` | `Base` |
| `AnimationEvent` | `Entity`, `Player`, `Enemy` |
| `InteractiveObject` | `Inventory`, `Base` |

---

## 4. CLASS DIAGRAM — Chi tiết từng package

> **Ký hiệu:**
> - `──extends──►` : Kế thừa (Inheritance)
> - `──implements──►` : Triển khai interface
> - `──depends──►` : Phụ thuộc / sử dụng
> - `(abstract)` : Class trừu tượng
> - `«interface»` : Interface
> - `«SO»` : ScriptableObject

---

### Package: Base

```
EventBus<T>

IEventBinding<T>  «interface»
    └── EventBinding<T>  ──implements──► IEventBinding<T>

«interface» IEvent

ServiceLocator
ObjectPool<T>
MonoBehaviourPool  (MonoBehaviour)
GameBootstrapper   (static)
Helper             (MonoBehaviour)
CameraResolutionFitter (MonoBehaviour)
DebugCustom
```

---

### Package: Interfaces

```
«interface» IHit
«interface» IHitVFX
«interface» ICounterable
«interface» ICollectable
«interface» IInteractable
```

---

### Package: StateMachine

```
StateMachine
    └── depends──► EntityState
```

---

### Package: Stats

```
«SO» MajorStats
«SO» OffensiveStats
«SO» DefensiveStats

Stat   (referenced by all Stats SOs as field)
```

---

### Package: Entity

```
Entity  (abstract, MonoBehaviour)
    ├── field──► EntityStat
    ├── field──► EntityHealth
    ├── field──► EntityVfx
    ├── field──► EntityCombat
    └── field──► StateMachine

EntityStat        (MonoBehaviour)
    └── depends──► MajorStats, OffensiveStats, DefensiveStats

EntityHealth      (abstract, MonoBehaviour) ──implements──► IHit

EntityVfx         (abstract, MonoBehaviour) ──implements──► IHitVFX

EntityCombat      (MonoBehaviour)
    └── depends──► EntityStat, EntityVfx

EntityState
    └── depends──► StateMachine

EntityStatusHandler (MonoBehaviour)
    └── depends──► EntityHealth, EntityVfx

EntityDropManager (MonoBehaviour)
```

---

### Package: Player

```
Entity
    └── Player  (MonoBehaviour)
            ├── field──► PlayerHealth
            ├── field──► PlayerCombat
            ├── field──► PlayerVfx
            ├── field──► PlayerLevel
            ├── field──► PlayerInventory
            ├── field──► SkillButtonHandler
            └── field──► AfterImageEffect

PlayerHealth    ──extends──► EntityHealth
PlayerCombat    ──extends──► EntityCombat
PlayerVfx       ──extends──► EntityVfx
PlayerStats     ──extends──► EntityStat
PlayerLevel     (MonoBehaviour)  ──depends──► EventBus<EnemyDiedEvent>
PlayerInteract  (MonoBehaviour)  ──implements──► IInteractable
SkillButtonHandler (MonoBehaviour)

PlayerInventory (MonoBehaviour)  ──extends──► InventoryBase

── Player States ──

EntityState
    └── PlayerState  (abstract)
            ├── PlayerIdleState
            ├── PlayerMoveState
            ├── PlayerJumpState
            ├── PlayerFallState
            ├── PlayerWallSildeState
            ├── PlayerWallJumpState
            ├── PlayerDashState
            ├── PlayerAttackState
            │       └── PlayerJumpAttackState  ──extends──► PlayerAttackState
            ├── PlayerCounterState
            ├── PlayerKnockBackState
            ├── PlayerDeadState
            ├── PlayerDismantleState
            └── PlayerDomainExpasionState

── Skills ──

SkillBase  (MonoBehaviour)
    ├── SkillDash
    ├── SkillCounter
    ├── SkillDismantle
    ├── SkillTimeEcho
    └── DomainExpasionSkill

── Skill Objects ──

SkillObject_Base  (MonoBehaviour)
    ├── SkillProjectileBase  ──extends──► SkillObject_Base
    ├── SkillObjectTimeEcho  ──extends──► SkillObject_Base
    ├── SkillObjectDismantle ──extends──► SkillObject_Base
    └── DomainExpasionSkillObject ──extends──► SkillObject_Base

── Data ──

«SO» CharacterData
«SO» SkillBaseDefinition
```

---

### Package: Enemy

```
Entity
    └── Enemy  (abstract, MonoBehaviour)  ──implements──► ICounterable
            ├── EnemySkeleton
            └── EnemySlime
            └── EnemyDeathRippler

EnemyHealth  ──extends──► EntityHealth
EnemyVfx     ──extends──► EntityVfx
EnemyPool    (MonoBehaviour)  ──depends──► ObjectPool<Enemy>

── Enemy States ──

EntityState
    └── EnemyState  (abstract)
            ├── EnemyIdleState
            ├── EnemyMoveState
            ├── EnemyChaseState
            ├── EnemyAttackState
            └── EnemyStunState

EnemySkeletonDeathState  ──extends──► EnemyState
EnemySlimeDeathState     ──extends──► EnemyState

EnemyGroundedState  ──extends──► EnemyState
    ├── EnemyDeathRipplerTeleportState     ──extends──► EnemyGroundedState
    └── EnemyDeathRipplerTeleportBackState ──extends──► EnemyGroundedState

EnemyDeathRipplerIdleState    ──extends──► EnemyState
EnemyDeathRipplerChaseState   ──extends──► EnemyState
EnemyDeathRipplerBattleState  ──extends──► EnemyState
EnemyDeathRipplerAttackState  ──extends──► EnemyState
EnemyDeathRipplerStunState    ──extends──► EnemyState
EnemyDeathRipplerDeathState   ──extends──► EnemyState

── Data ──

«SO» EnemyData
```

---

### Package: Stats

```
«SO» MajorStats
    └── field──► Stat (x4: Strength, Agility, Intelligence, Vitality)

«SO» OffensiveStats
    └── field──► Stat (x5: Damage, CritChance, CritPower, AttackMultiplier, ElementalDamage)

«SO» DefensiveStats
    └── field──► Stat (x6: MaxHealth, Armor, Evasion, ElementalResistance, KnockBackThreshold, HealthRegen)

Stat
```

---

### Package: Inventory

```
InventoryBase  (MonoBehaviour)
    └── PlayerInventory  ──extends──► InventoryBase

ItemInventory
    ├── depends──► ItemData
    └── depends──► EntityStat

ItemInventoryEquipment
    └── depends──► ItemInventory

── Data ──

«SO» ItemData
    └── EquipmentData  ──extends──► ItemData
            └── ItemCraftData  ──extends──► EquipmentData

ItemModifier
ItemListData
```

---

### Package: Quest

```
«SO» QuestData  (abstract)
    ├── KillQuestData      ──extends──► QuestData  ──depends──► EventBus<EnemyDiedEvent>
    ├── RescueNpcQuestData ──extends──► QuestData  ──depends──► EventBus<NpcRescuedEvent>
    └── Scavahunt          ──extends──► QuestData

SceneQuestController  (MonoBehaviour)
    ├── depends──► QuestData
    └── depends──► SaveManager
```

---

### Package: Save

```
SaveManager  (MonoBehaviour)
    ├── depends──► PlayerSaveData
    ├── depends──► ItemDataRegistry
    └── depends──► ServiceLocator

«SO» ItemDataRegistry
    └── depends──► ItemData

PlayerSaveData
    ├── ItemSaveEntry      (nested)
    ├── EquipSaveEntry     (nested)
    ├── SkillTreeSaveData  (nested)
    │       └── NodeSaveEntry (nested)
    ├── SceneStateData     (nested)
    └── QuestProgressEntry (nested)

SaveSystemTester  (MonoBehaviour)
```

---

### Package: NPC

```
Npc  (MonoBehaviour)  ──implements──► IInteractable
    └── RescuableNpc  ──extends──► Npc
            └── depends──► EventBus<NpcRescuedEvent>

NPCMerchant  (MonoBehaviour)
```

---

### Package: Effect

```
AfterImageEffect  (MonoBehaviour)
AfterImageGhost   (MonoBehaviour)
HitEffect         (MonoBehaviour)
PoolableVfx       (MonoBehaviour)

ItemEffectData    (abstract)
    ├── ItemEffectIceData   ──extends──► ItemEffectData
    └── ItemEffectDracula   ──extends──► ItemEffectData
```

---

### Package: UI

```
UIManager  (MonoBehaviour)

── HUD ──
SkillButton          (MonoBehaviour)  ──depends──► SkillButtonHandler
SkillCooldownUI      (MonoBehaviour)  ──depends──► SkillButtonHandler
SkillCooldownConnector (MonoBehaviour)
UIQuestHUD           (MonoBehaviour)  ──depends──► EventBus<QuestProgressEvent>
HealthBar            (MonoBehaviour)

── Menus ──
UIMainMenu           (MonoBehaviour)  ──depends──► SaveManager
UICanvasChange       (MonoBehaviour)
UIFloating_Panel     (MonoBehaviour)
UIMenu_Toggle        (MonoBehaviour)

── Skill Tree ──
UISkillTree          (MonoBehaviour)
UITreeNode           (MonoBehaviour)
UIConnectedHandler   (MonoBehaviour)
UIConnectedLine      (MonoBehaviour)
UIToolTip            (MonoBehaviour)
UISkillToolTip       (MonoBehaviour)

── Inventory ──
UiItemSlotBase       (MonoBehaviour)
    ├── UIItemSlot       ──extends──► UiItemSlotBase
    └── UIEquipmentSlot  ──extends──► UiItemSlotBase

UI_Inventory         (MonoBehaviour)
UIItemActionPanel    (MonoBehaviour)
UIEquipmentActionPanel (MonoBehaviour)
UIStatPanel          (MonoBehaviour)  ──depends──► EntityStat

── Level Up ──
UILevelUpPanel       (MonoBehaviour)  ──depends──► EventBus<PlayerLevelUpEvent>

── Store / Merchant ──
UI_StoreMerchant     (MonoBehaviour)
UI_StoreItemEntry    (MonoBehaviour)
UI_CatergoriestSlot  (MonoBehaviour)
UI_CarftIngerdientSlot (MonoBehaviour)
UI_ItemDetailsSlot   (MonoBehaviour)
```

---

### Package: Scene

```
PersistentObject     (MonoBehaviour)
SceneSpawnPoint      (MonoBehaviour)
CameraConfinerBounds (MonoBehaviour)
CameraZoneTrigger    (MonoBehaviour)
SceneEntityManager   (MonoBehaviour)  ──depends──► SaveManager
```

---

### Package: System

```
GameManager  (MonoBehaviour)
    ├── depends──► ServiceLocator
    └── depends──► EventBus<GamePauseChangedEvent>

GameMessages  (static)
MyMenu        (MonoBehaviour)
```

---

### Package: Pool

```
SkillObjectDismantlePool  (MonoBehaviour)  ──depends──► ObjectPool<SkillObjectDismantle>
SkillObjectTimeEchoPool   (MonoBehaviour)  ──depends──► ObjectPool<SkillObjectTimeEcho>
```

---

### Package: AnimationEvent

```
EntityAnimationEvent  (MonoBehaviour)  ──depends──► Entity
PlayerAnimationEvent  (MonoBehaviour)  ──depends──► Player
EnemyAnimationEvent   (MonoBehaviour)  ──depends──► Enemy
SkillAnimationEvent   (MonoBehaviour)  ──depends──► SkillBase
```

---

### Package: InteractiveObject

```
ObjectChestBase       (MonoBehaviour)
    ├── ObjectChestDropItem    ──extends──► ObjectChestBase
    └── ObjectChestDropMission ──extends──► ObjectChestBase

ItemObjectPickable    (MonoBehaviour)  ──implements──► ICollectable
```

---

### Package: Trap

```
BearTrap  (MonoBehaviour)  ──implements──► IHit
```

---

### Package: BackGround

```
ParrallexBG         (MonoBehaviour)
ParrallexController (MonoBehaviour)
    └── depends──► ParrallexBG
```

---

### Package: Sound

```
SoundManager  (MonoBehaviour)
    └── depends──► ServiceLocator
```

---

## 5. TỔNG KẾT QUAN HỆ QUAN TRỌNG

### Chuỗi kế thừa Entity

```
MonoBehaviour
    └── Entity (abstract)
            ├── Player
            └── Enemy (abstract)
                    ├── EnemySkeleton
                    ├── EnemySlime
                    └── EnemyDeathRippler  (Boss)
```

### Chuỗi kế thừa Health

```
MonoBehaviour + IHit
    └── EntityHealth (abstract)
            ├── PlayerHealth
            └── EnemyHealth
```

### Chuỗi kế thừa VFX

```
MonoBehaviour + IHitVFX
    └── EntityVfx (abstract)
            ├── PlayerVfx
            └── EnemyVfx
```

### Chuỗi kế thừa State

```
EntityState
    ├── PlayerState (abstract)
    │       └── [14 player states]
    └── EnemyState (abstract)
            ├── [5 shared enemy states]
            ├── EnemySkeletonDeathState
            ├── EnemySlimeDeathState
            ├── EnemyGroundedState
            │       ├── EnemyDeathRipplerTeleportState
            │       └── EnemyDeathRipplerTeleportBackState
            └── [6 boss-specific states]
```

### Chuỗi kế thừa Item Data

```
ScriptableObject
    └── ItemData
            └── EquipmentData
                    └── ItemCraftData
```

### Chuỗi kế thừa Quest

```
ScriptableObject
    └── QuestData (abstract)
            ├── KillQuestData
            ├── RescueNpcQuestData
            └── Scavahunt
```
