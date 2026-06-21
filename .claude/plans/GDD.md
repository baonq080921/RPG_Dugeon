# GAME DESIGN DOCUMENT (GDD)
# RPG Dungeon

**Phiên bản:** 1.0  
**Ngày:** 17/06/2026  
**Engine:** Unity (Universal Render Pipeline)  
**Nền tảng:** Mobile — Android / iOS

---

## MỤC LỤC

1. [Tầm nhìn & Tổng quan](#1-tầm-nhìn--tổng-quan)
2. [Gameplay Core Loop](#2-gameplay-core-loop)
3. [Nhân vật chính](#3-nhân-vật-chính)
4. [Hệ thống chiến đấu](#4-hệ-thống-chiến-đấu)
5. [Hệ thống kỹ năng & Skill Tree](#5-hệ-thống-kỹ-năng--skill-tree)
6. [Hệ thống kẻ thù](#6-hệ-thống-kẻ-thù)
7. [Hệ thống thống số (Stats)](#7-hệ-thống-thống-số-stats)
8. [Hệ thống Level & Kinh nghiệm](#8-hệ-thống-level--kinh-nghiệm)
9. [Hệ thống Inventory & Equipment](#9-hệ-thống-inventory--equipment)
10. [Hệ thống Quest](#10-hệ-thống-quest)
11. [Hệ thống NPC & Cửa hàng](#11-hệ-thống-npc--cửa-hàng)
12. [Hệ thống lưu trò chơi](#12-hệ-thống-lưu-trò-chơi)
13. [Thiết kế màn chơi (Levels)](#13-thiết-kế-màn-chơi-levels)
14. [Giao diện người dùng (UI/UX) & HUD](#14-giao-diện-người-dùng-uiux--hud)
15. [Điều khiển (Control)](#15-điều-khiển-control)
16. [Luồng màn hình (Screen Flow)](#16-luồng-màn-hình-screen-flow)
17. [Hệ thống kỹ thuật](#17-hệ-thống-kỹ-thuật)

---

## 1. TẦM NHÌN & TỔNG QUAN

### 1.1 Mô tả game
RPG Dungeon là game hành động nhập vai 2D nền tảng mobile. Người chơi điều khiển một nhân vật chiến đấu xuyên qua các màn dungeon đầy kẻ thù, thu thập trang bị, nâng cấp kỹ năng và hoàn thành nhiệm vụ. Game lấy cảm hứng thẩm mỹ từ manga/anime Nhật Bản, thể hiện rõ qua tên nhân vật Yuji, kỹ năng Domain Expansion và Mahoraga.

### 1.2 Thể loại
2D Side-scrolling Action RPG

### 1.3 Đối tượng người chơi
Người chơi mobile yêu thích thể loại action RPG, độ tuổi 16–30, quen với anime/manga.

### 1.4 Điểm khác biệt cốt lõi
- Hệ thống nguyên tố kết hợp (Electric tích charge, Ice làm chậm, Dracula DoT)
- Cơ chế Counter real-time yêu cầu phản xạ cao
- Kỹ năng Ultimate "Domain Expansion" mang tính chiến lược
- Skill Tree nhánh hóa: mỗi kỹ năng có nhiều upgrade path khác nhau

---

## 2. GAMEPLAY CORE LOOP

```
Vào màn chơi
    ↓
Khám phá dungeon room
    ↓
Chiến đấu với enemy → Thu thập item drop
    ↓
Hoàn thành Quest trong màn
    ↓
Mở chest / tương tác NPC
    ↓
Chuyển sang room tiếp theo (portal)
    ↓
Nâng cấp: Level Up → chọn Major Stat
         Skill Tree → unlock kỹ năng mới
         Crafting / Mua đồ tại Merchant
    ↓
Lặp lại
```

**Session loop:** Người chơi có thể Save tại checkpoint. Khi Load lại, toàn bộ trạng thái (vị trí, HP, inventory, quest, skill tree) được khôi phục nguyên vẹn.

---

## 3. NHÂN VẬT CHÍNH

### 3.1 Thông tin
- **Tên:** Yuji
- **Loại:** Player-controlled character
- **Phong cách:** Cận chiến tốc độ cao kết hợp kỹ năng đặc biệt

### 3.2 Chuyển động

| Thông số | Mô tả |
|---|---|
| Di chuyển | Left/Right với tốc độ từ `CharacterData.MoveSpeed` |
| Nhảy | Nhảy 2 lần (double jump), lực nhảy từ `JumpForce` |
| Kiểm soát trên không | `AirControlFactor` giảm khả năng điều hướng khi bay |
| Trượt tường | `SlideDownSpeed` — trượt xuống chậm khi bám tường |
| Wall Jump | Bật tường với `WallJumpForce` vector riêng |

### 3.3 Trạng thái nhân vật (State Machine)

```
Idle ←→ Move
  ↓         ↓
Jump → Fall
  ↓
Attack (combo 1 → 2 → 3)
  ↓
JumpAttack
  ↓
Dash (invincible frame)
  ↓
Counter
  ↓
KnockBack (bị văng)
  ↓
Dead
```

Ngoài ra: `Dismantle` (trạng thái cast skill Dismantle), các skill state (TimeEcho, Domain Expansion).

### 3.4 Hệ thống Combo tấn công
- Combo tối đa: 3 đòn (cấu hình trong `CharacterData.ComboLimit`)
- Mỗi đòn có `AttackVelocity` vector riêng — nhân vật lướt về phía trước khi đánh
- Nếu người chơi bấm tiếp trong thời gian `TimeResetCombo` → tiếp tục combo
- Sau khi hết combo → cooldown `ComboEndDelay` trước khi combo lại
- `AttackMultiplier` từ stats ảnh hưởng tốc độ animation tấn công

---

## 4. HỆ THỐNG CHIẾN ĐẤU

### 4.1 Phát hiện va chạm
- `EntityCombat` dùng `Physics2D.OverlapCircleAll` với điểm kiểm tra (`_targetCheck`) và bán kính (`_targetRadius`)
- Target được lọc theo `LayerMask` — enemy và player ở layer riêng biệt

### 4.2 Công thức tính sát thương

```
FinalPhysical  = PhysicalDamage  × (1 - TargetArmor)
FinalElemental = ElementalDamage × (1 - TargetElementalResistance)
TotalDamage    = FinalPhysical + FinalElemental
```

**Crit:** Nếu `random(0,100) < CritChance` → sát thương nhân thêm `CritPower`  
**Evasion:** Nếu `random(0,100) < EvasionValue` của target → đòn đánh hoàn toàn bị né, không trừ HP

### 4.3 Hệ thống nguyên tố

#### Electric (Điện)
- Mỗi đòn điện tích thêm `_electricBuildUpCharge` vào thanh điện tích
- Khi điện tích đạt `_maxiumCharge` (mặc định 1.0):
  - Instantiate hiệu ứng sét đánh
  - Gây damage trực tiếp + gây trạng thái **Shock** (choáng) trong `_shockDuration` giây
  - Reset điện tích về 0

#### Ice (Băng)
- Áp dụng ngay khi trúng đòn băng
- Gây hiệu ứng làm chậm (`_slowPercent`) cho entity
- Có VFX đặc trưng màu xanh

#### Dracula (Máu/Bleed)
- Gây Damage over Time theo `tickInterval`
- Tổng thời gian: `duration` giây
- Re-apply reset lại thời gian (không stack)

### 4.4 Cơ chế Counter
- Khi player vào `PlayerCounterState`, `PlayerCombat.IsPerformedCounter()` kiểm tra enemy trong tầm
- Nếu enemy đang ở trạng thái có thể bị counter (`ICounterable.CanCounter = true`):
  - Gọi `enemy.HandleCounter()` → interrupt tấn công của enemy
  - Player thực hiện animation counter đặc biệt
- Nếu không counter được trong thời gian `Duration` → trở về Idle

### 4.5 KnockBack
- Entity bị KnockBack khi nhận damage vượt `KnockBackThreshold`
- Vector KnockBack: `CharacterData.KnockBack`, kéo dài `KnockBackDuration` giây

---

## 5. HỆ THỐNG KỸ NĂNG & SKILL TREE

### 5.1 Cơ chế kích hoạt kỹ năng

1. Người chơi nhấn nút on-screen → `SkillButton.onClick` → `SkillButtonHandler.PressSkill(skillName)`
2. `SkillButtonHandler` kiểm tra: skill đã unlock chưa? Còn cooldown không?
3. Nếu hợp lệ → đánh dấu `_pending[index] = true`
4. Trong `PlayerState.Update()` → gọi `TryConsumeSkill()` → chuyển sang skill state tương ứng
5. Khi thoát state → cooldown bắt đầu đếm ngược

### 5.2 Danh sách kỹ năng

#### Dash (Lướt)
- **Mô tả:** Lướt theo hướng di chuyển, có thể dùng trên không
- **Cơ chế:** `rb.gravityScale = 0` trong `DashDuration`, tốc độ = `DashSpeed`
- **After-Image:** Hiệu ứng bóng theo sau trong suốt dash
- **Upgrade:**
  - `Dash_CloneOnStart` — tạo phân thân tại điểm xuất phát
  - `Dash_CloneOnStartAndArrival` — phân thân tại điểm xuất phát và điểm đến
  - `Dash_ShardOnStart` — tạo mảnh vỡ tại điểm xuất phát
  - `Dash_ShardOnStartAndArrival` — mảnh vỡ tại cả hai điểm

#### Counter (Phản đòn)
- **Mô tả:** Phản đòn ngay khi enemy đang tấn công
- **Cơ chế:** Trong cửa sổ thời gian → check `ICounterable` trong tầm → xử lý counter
- **Yêu cầu:** Timing chính xác, phải có enemy đủ điều kiện trong tầm

#### Dismantle (Phân giải)
- **Mô tả:** Phóng projectile năng lượng về phía trước
- **Cơ chế:** Lấy object từ `SkillObjectDismantlePool`, phóng về phía enemy
- **Upgrade:**
  - `Dismantle` — 1 projectile
  - `Dismantle_Upgrade` — 2 projectile liên tiếp (delay `_launchDelay` giữa 2 phát)

#### TimeEcho (Phân thân thời gian)
- **Mô tả:** Triệu hồi bóng phân thân tấn công cùng lúc với player
- **Điều kiện:** Chỉ dùng được khi đứng trên mặt đất (`isGrounded`)
- **Upgrade:**
  - `TimeEcho` — phân thân cơ bản tấn công theo player
  - `TimeEcho_ExtraEchoAttack` — phân thân có thêm đòn tấn công phụ
  - `TimeEcho_ExtraEchoAttackMaho` — phân thân Mahoraga với đòn tấn công đặc biệt
  - `TimeEcho_HealOnEcho` — phân thân hồi HP khi tấn công
  - `TimeEcho_HealOnEchoAndCoolDownAllSkill` — hồi HP + giảm cooldown tất cả kỹ năng

#### Domain Expansion (Mở rộng lãnh địa) — Ultimate Skill
- **Mô tả:** Triệu hồi vùng domain AoE bao phủ toàn bộ khu vực, gây sát thương liên tục
- **Cơ chế:** Instantiate `DomainExpansionSkillObject`, tính damage từ cả physical lẫn elemental stat, kích hoạt trong `Duration` giây
- **Scale Factor:** Hệ số nhân thiệt hại riêng để cân bằng sức mạnh ultimate

### 5.3 Skill Tree
- `UISkillTree` hiển thị cây node kết nối bằng đường line (`UIConnectedLine`)
- Mỗi `UITreeNode` chứa `skillTreeData` (ScriptableObject) và trạng thái `isUnlocked`
- Node phụ thuộc vào node trước — chỉ unlock được khi node cha đã mở
- Tốn `SkillPoints` để unlock (thu được qua level up hoặc quest)
- Upgrade được lưu vào save file theo tên node

---

## 6. HỆ THỐNG KẺ THÙ

### 6.1 Cấu trúc AI (State Machine)

```
Idle (đứng chờ)
  ↓ phát hiện player
Chase (đuổi theo)
  ↓ vào tầm đánh
Attack (tấn công)
  ↓ trúng đòn counter / skill
Stun (choáng)
  ↓ HP = 0
Death (chết → ReturnToPool)
```

### 6.2 Thông số phát hiện người chơi

| Thông số | Mô tả |
|---|---|
| `DetectionRange` | Bán kính phát hiện player |
| `AttackRange` | Khoảng cách bắt đầu tấn công |
| `minDistanceRetreat` | Khoảng cách tối thiểu không tiến sát hơn |

### 6.3 Danh sách kẻ thù

#### EnemySkeleton
- **Phong cách:** Chiến binh cận chiến cơ bản
- **Death state:** Khi chết → văng lên với velocity (3, 10), sau 2 giây → ReturnToPool
- **Đặc điểm:** Có thể bị counter

#### EnemySlime (3 biến thể: Normal, Large, Small)
- **Phong cách:** Kẻ thù đơn giản, di chuyển chậm
- **Death state:** Khi chết → `CreateChild()` — sinh ra EnemySlime con nhỏ hơn
- **Đặc điểm:** Mechanic spawn con sau khi chết tạo áp lực cho người chơi

#### EnemyDeathRippler — Boss
- **Phong cách:** Boss tốc độ, linh hoạt, thoắt ẩn thoắt hiện
- **States đặc biệt:**
  - `BattleState` — đánh giá tình huống, vào attack hoặc chase
  - `TeleportState` — biến mất, dịch chuyển đến vị trí mới trong arena (invulnerable)
  - `TeleportBackState` — dịch chuyển về phía sau lưng player

- **Cơ chế Teleport:**
  - `ShouldTelePort()` tung xúc xắc với xác suất `_chanceToTeleport`
  - Nếu không teleport: xác suất tăng thêm 5% cho lần sau (tích lũy)
  - Nếu teleport: reset về xác suất ban đầu
  - `PrepareBehindPlayerTeleport()`: tính điểm đằng sau lưng player bằng Raycast xuống mặt đất
  - `FindTeleportPoint()`: fallback teleport ngẫu nhiên trong `_aeraCollider`

- **Aggro system:** `TriggerAggro()` — Boss vào trạng thái chiến đấu hoàn toàn

### 6.4 Object Pool cho Enemy
`EnemyPool` sử dụng `ObjectPool<Enemy>` — enemy sau khi chết không bị Destroy mà trả về pool, giảm GC pressure trên mobile.

---

## 7. HỆ THỐNG THỐNG SỐ (STATS)

### 7.1 Major Stats (chỉ Player)
Người chơi phân bổ điểm vào 4 chỉ số chính khi level up:

| Stat | Ảnh hưởng |
|---|---|
| **Strength** | Tăng sát thương vật lý |
| **Agility** | Tăng tốc độ, evasion |
| **Intelligence** | Tăng sát thương nguyên tố |
| **Vitality** | Tăng HP tối đa, hồi HP |

### 7.2 Offensive Stats

| Stat | Mô tả |
|---|---|
| Damage | Sát thương vật lý cơ bản |
| CritChance | % xác suất chí mạng |
| CritPower | Hệ số nhân khi chí mạng |
| AttackMultiplier | Hệ số nhân tốc độ tấn công |
| ElementalDamage | Sát thương nguyên tố |

### 7.3 Defensive Stats

| Stat | Mô tả |
|---|---|
| MaxHealth | HP tối đa |
| Armor | Giảm % sát thương vật lý nhận vào |
| Evasion | % né đòn hoàn toàn |
| ElementalResistance | Giảm % sát thương nguyên tố |
| KnockBackThreshold | Ngưỡng để bị KnockBack |
| HealthRegen | Lượng HP hồi tự động theo thời gian |

### 7.4 Stat Modifier System
- Trang bị gắn `ItemModifier[]` lên stat tương ứng khi equip
- Tháo đồ → remove modifier → stat trở về giá trị gốc
- Công thức: `FinalValue = BaseValue + sum(modifiers)`

---

## 8. HỆ THỐNG LEVEL & KINH NGHIỆM

### 8.1 Thu kinh nghiệm

```
FinalXP = EnemyBaseXP × Clamp(1 - (PlayerLevel - EnemyLevel) × 0.05, 0.1, 1.0)
```

- Giết enemy cùng cấp: nhận 100% EXP
- Cấp cao hơn enemy 10 bậc: chỉ nhận 50% EXP
- Luôn nhận tối thiểu 10% EXP (không bao giờ về 0)

### 8.2 Yêu cầu EXP

```
ExpToNextLevel = BaseExpRequired × Level ^ expGrowthRate
```

Curve lũy thừa — những level cao cần rất nhiều EXP hơn.

### 8.3 Phần thưởng Level Up
- Trigger `PlayerLevelUpEvent` → UI hiện bảng chọn Major Stat
- Người chơi chọn 1 trong 4 chỉ số để tăng
- VFX level up hiển thị trên nhân vật

---

## 9. HỆ THỐNG INVENTORY & EQUIPMENT

### 9.1 Inventory
- Danh sách `ItemInventory`, mỗi slot stack tối đa **99 đơn vị**
- Phân loại item qua `ItemTypes` enum
- Loại item: vật phẩm thường, equipment, crafting material

### 9.2 Equipment Slots
Phân theo `EquipSlotType`:
- Khi equip: `AddModifiers(playerStats)` + `AddItemEffect(player)`
- Khi unequip: `RemoveModifiers` + `RemoveItemEffect`

### 9.3 Item Effects (Passive từ trang bị)

| Effect | Mô tả |
|---|---|
| `ItemEffectData` | Hiệu ứng passive cơ bản |
| `ItemEffectIceData` | Thêm hiệu ứng băng khi tấn công |
| `ItemEffectDracula` | Hút máu / gây DoT khi tấn công |

### 9.4 Crafting
- `ItemCraftData` định nghĩa recipe: `RequirementItem[]` (loại nguyên liệu + số lượng)
- Người chơi craft tại `NPCMerchant` sau khi đủ nguyên liệu trong inventory

---

## 10. HỆ THỐNG QUEST

### 10.1 Cấu trúc
- `QuestData` (abstract ScriptableObject) — base cho mọi quest
- Mỗi scene có một `SceneQuestController` gắn quest riêng
- Progress lưu vào `SaveManager` theo tên scene

### 10.2 Loại quest

#### Kill Quest (Tiêu diệt)
- **Mục tiêu:** Giết đủ `RequiredKillCount` enemy
- **Cơ chế:** Listen `EnemyDiedEvent` trên EventBus
- **Ví dụ:** "Tiêu diệt 5 Skeleton"

#### Rescue NPC Quest (Giải cứu)
- **Mục tiêu:** Giải cứu đủ `RequiredRescueCount` NPC
- **Cơ chế:** Listen `NpcRescuedEvent`, kích hoạt khi tương tác với `RescuableNpc`
- **Ví dụ:** "Giải cứu 3 dân thường"

#### Scavenger Hunt (Tìm kiếm)
- Tìm và thu thập vật phẩm/rương trong màn chơi

### 10.3 Flow Quest

```
Vào scene → SceneQuestController.Start()
  → Nếu quest đã complete (từ save): raise QuestCompletedEvent, dừng
  → Nếu chưa: restore progress → StartTracking → raise QuestStartedEvent

Trong game:
  Event xảy ra → NotifyProgress → lưu progress → raise QuestProgressEvent → update UI

Hoàn thành:
  NotifyCompleted → StopTracking → MarkQuestComplete(sceneName) → raise QuestCompletedEvent
```

---

## 11. HỆ THỐNG NPC & CỬA HÀNG

### 11.1 NPC cơ bản
`Npc` — có thể tương tác, hiển thị dialogue text với hiệu ứng typewriter (từng ký tự xuất hiện dần).

### 11.2 RescuableNpc
- Kế thừa `Npc`, chỉ được giải cứu **một lần**
- Khi tương tác: play dialogue → raise `NpcRescuedEvent` → `gameObject.SetActive(false)`

### 11.3 NPCMerchant
- Mua item, bán item, craft item theo recipe
- UI `UI_StoreMerchant` với phân loại theo danh mục, hiển thị nguyên liệu cần và chi tiết item

---

## 12. HỆ THỐNG LƯU TRÒ CHƠI

### 12.1 Format
- **JSON** lưu tại `Application.persistentDataPath/save.json`
- Serialize bằng `Unity.JsonUtility`

### 12.2 Dữ liệu được lưu

| Nhóm | Nội dung |
|---|---|
| Player | Scene, vị trí XY, HP hiện tại |
| Level | Level, EXP hiện tại, EXP cần để lên cấp |
| Stats | Điểm phân bổ: Strength, Agility, Intelligence, Vitality |
| Inventory | ItemId + stackSize của mỗi item |
| Equipment | SlotType + ItemId của mỗi slot |
| Skill Tree | SkillPoints còn lại + danh sách node đã unlock (theo tên) |
| Quest | Danh sách scene đã complete + tiến độ quest đang làm |
| Scene State | Enemy đã giết, chest đã mở (per scene) |

### 12.3 Hai chế độ restore

| Chế độ | Khi nào | Có restore vị trí |
|---|---|---|
| **Full Load** | Continue từ main menu, đọc từ disk | Có |
| **Transition Snapshot** | Chuyển room qua portal, lưu trong memory | Không (dùng SpawnPoint) |

### 12.4 ItemDataRegistry
ScriptableObject duy trì bản đồ `ItemId → ItemData`, cho phép deserialize inventory từ string ID mà không cần reference trực tiếp.

---

## 13. THIẾT KẾ MÀN CHƠI (LEVELS)

### 13.0 Danh sách Room

| Scene | Vai trò |
|---|---|
| `Room0` | Tutorial — hướng dẫn cơ bản điều khiển và chiến đấu cho người chơi mới |
| `Room1` | Room đầu tiên khi New Game — dungeon entry |
| `Room2` | Room trung gian |
| `Room3` | Room cuối / Boss room (EnemyDeathRippler) |

Người chơi di chuyển qua các room theo thứ tự Room1 → Room2 → Room3 qua portal. Room0 (Tutorial) là màn chơi riêng biệt, không bắt buộc trong flow chính.

### 13.1 Cấu trúc Scene
Mỗi scene (Room) là một khu vực dungeon độc lập:

| Component | Vai trò |
|---|---|
| `SceneSpawnPoint` | Điểm xuất hiện khi vào scene |
| `SceneQuestController` | Quest riêng của scene |
| `SceneEntityManager` | Quản lý trạng thái enemy/chest, restore khi load |
| `CameraZoneTrigger` | Chuyển vùng confine camera |
| `CameraConfinerBounds` | Giới hạn biên camera trong khu vực |

### 13.2 Interactive Objects

| Object | Mô tả |
|---|---|
| `ItemObjectPickable` | Vật phẩm nhặt được (drop từ enemy hoặc đặt sẵn) |
| `ObjectChestDropItem` | Rương thường chứa item |
| `ObjectChestDropMission` | Rương nhiệm vụ |
| `BearTrap` | Bẫy gấu — gây damage và immobilize player |

---

## 14. GIAO DIỆN NGƯỜI DÙNG (UI/UX) & HUD

### 14.1 Layout HUD In-Game

```
┌─────────────────────────────────────────────┐
│ [Avatar] [═══════════ HP ══════════════] [🗃]│  ← Top bar
│                                             │
│         (gameplay area)                     │
│                                             │
│  ▲               [●][●][●]                  │
│ ◄ ►              [●][●][●]                  │
└─────────────────────────────────────────────┘
  Di chuyển          Skills
```

**Góc trên-trái:**
- Ô avatar nhân vật — **tap để mở panel Inventory / Skill Tree / Quest** (slide animation)
- Thanh HP màu đỏ kéo dài qua đầu màn hình, cập nhật real-time

**Góc trên-phải:**
- Icon rương/collectible (hiển thị item đặc biệt trong room)

**Góc dưới-trái — Di chuyển (3 nút mũi tên):**
- ◄ di chuyển trái
- ► di chuyển phải
- ▲ nhảy (hỗ trợ double jump)

**Góc dưới-phải — 6 nút kỹ năng tròn (2 hàng × 3 cột):**
- Basic Attack — đánh thường, kích hoạt combo 3 đòn
- Counter — phản đòn khi enemy tấn công
- Dash — lướt, có i-frame
- Dismantle — phóng projectile
- TimeEcho — triệu hồi phân thân
- Domain Expansion — Ultimate AoE

Mỗi nút có **radial fill cooldown** (đếm ngược theo giây) và **lock overlay** khi chưa unlock trong Skill Tree.

### 14.2 Panel System (mở bằng tap Avatar)

3 tab chuyển qua nhau bằng slide animation (DOTween):

| Tab | Nội dung |
|---|---|
| **Inventory** | Grid item, equipment slots, stat panel — mặc định hiển thị khi mở |
| **Skill Tree** | Cây node kết nối, tooltip, unlock bằng Skill Points |
| **Quest** | Tên quest + mô tả + progress (vd "3/5") |

**Auto-popup (không cần mở tab):**
- "Mission Complete" banner — fade in → giữ 2.5s → fade out khi quest xong
- `UILevelUpPanel` — hiện tự động khi level up, chọn 1 trong 4 Major Stat

### 14.3 Menu Panels Khác

| Panel | Mô tả |
|---|---|
| `UIMainMenu` | New Game / Continue (Continue grayed out nếu không có save) |
| `UI_StoreMerchant` | Mua / bán / craft tại NPC Merchant |

### 14.4 Nguyên tắc UX Mobile
- Tất cả nút đủ lớn để ngón tay chạm không nhầm
- Không sử dụng gamepad hoặc keyboard trong production
- Event-driven: `IPointerDownHandler` → method call trực tiếp, không polling

---

## 15. ĐIỀU KHIỂN (CONTROL)

### 15.1 Sơ đồ layout màn hình

```
┌─────────────────────────────────────────────┐
│                                             │
│              [Gameplay]                     │
│                                             │
│   ▲                        [●] [●] [●]      │
│  ◄ ►                       [●] [●] [●]      │
└─────────────────────────────────────────────┘
```

### 15.2 Nút di chuyển (góc dưới-trái)

| Nút | Hành động |
|---|---|
| ◄ | Di chuyển trái |
| ► | Di chuyển phải |
| ▲ | Nhảy / double jump |

Không có joystick. Di chuyển là on-screen button thuần túy — input đọc qua Unity Input System `OnScreenButton`, feed vào `movementInput` Vector2.

### 15.3 Nút kỹ năng (góc dưới-phải — 6 nút tròn)

| Nút | Chức năng |
|---|---|
| Basic Attack | Tấn công thường, combo 3 đòn |
| Counter | Phản đòn — cần timing chính xác |
| Dash | Lướt theo hướng di chuyển, có i-frame |
| Dismantle | Phóng projectile năng lượng |
| TimeEcho | Triệu hồi phân thân chiến đấu cùng |
| Domain Expansion | Ultimate — AoE gây sát thương toàn vùng |

### 15.4 Nút mở menu (góc trên-trái)

- **Tap vào Avatar** → mở/đóng panel Inventory / Skill Tree / Quest

---

## 16. LUỒNG MÀN HÌNH (SCREEN FLOW)

```
┌──────────────┐
│  Main Menu   │
│ [New Game]   │──────────────────→ Room1
│ [Continue]   │──→ (scene đã lưu)
└──────────────┘

Room0 (Tutorial) ← chơi riêng, không bắt buộc trong flow chính

Room1 ──(portal)──→ Room2 ──(portal)──→ Room3
  ↑                                       │
  └──────── Save tại Checkpoint ──────────┘

Transition: fade đen (0.5s) → load scene → spawn tại portal destination
```

**Chú thích:**
- **New Game** → load `Room1`, tạo save mới
- **Continue** → load scene được ghi trong `save.json`
- **Room0** là Tutorial Scene riêng — điểm vào do bạn quyết định (Main Menu button hoặc auto khi new game lần đầu)
- **Checkpoint** trong mỗi room cho phép Save tại chỗ; khi Load lại từ Main Menu sẽ spawn đúng room và vị trí đã lưu

---

## 17. HỆ THỐNG KỸ THUẬT

### 15.1 EventBus
`EventBus<T>` generic type-safe — mỗi event type có channel riêng biệt, không ảnh hưởng lẫn nhau.

| Event | Khi nào |
|---|---|
| `EnemyDiedEvent` | Enemy chết (mang Level và BaseXP) |
| `PlayerLevelUpEvent` | Player lên cấp |
| `PlayerDiedEvent` | Player chết |
| `QuestStartedEvent` | Quest bắt đầu trong scene |
| `QuestProgressEvent` | Tiến độ quest thay đổi |
| `QuestCompletedEvent` | Quest hoàn thành |
| `NpcRescuedEvent` | NPC được giải cứu |
| `TargetGotHitEvent` | Enemy trúng đòn (kèm Transform + isCrit) |
| `GamePauseChangedEvent` | Game pause/resume |
| `OnInventoryChangedEvent` | Inventory thay đổi, UI cần refresh |
| `ResetStats` | Reset toàn bộ stat |

### 15.2 Object Pool
`ObjectPool<T>` generic Stack-based, O(1) get/release. Dùng cho:
- VFX (hit effect, elemental effects)
- Skill objects (Dismantle projectile, TimeEcho clone)
- Enemy instances

### 15.3 ServiceLocator
Dependency injection đơn giản, tránh coupling giữa các system:

```csharp
ServiceLocator.Register<SaveManager>(this);
ServiceLocator.Get<SaveManager>()?.Save();
```

Service đã đăng ký: `SaveManager`, `GameManager`, `UIManager`, `PlayerSkillManager`

### 15.4 GameBootstrapper
Chạy `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` trước khi load bất kỳ scene nào:
- Instantiate `PERSISTENTOBJECT` prefab (DontDestroyOnLoad)
- Tạo `Helper`, `CameraResolutionFitter`, `SaveManager`

### 15.5 Camera Resolution Fitter
Đảm bảo game hiển thị đúng tỷ lệ trên các màn hình mobile khác nhau.

### 15.6 Parallax Background
`ParrallaxController` + `ParrallexBG` tạo hiệu ứng chiều sâu nhiều lớp cho nền dungeon.

---

## TỔNG KẾT

RPG Dungeon là game mobile action RPG có chiều sâu gameplay đáng kể với:

- **Combat:** Real-time, cơ chế Counter đòi hỏi skill, 3 nguyên tố kết hợp chiến lược
- **Progression:** Level system, Skill Tree nhánh hóa, equipment với passive effects
- **Content:** Quest system đa dạng (kill/rescue/scavenge), Boss với AI teleport phức tạp
- **Technical:** Architecture tốt với EventBus, ObjectPool, State Machine pattern, Save system toàn diện
