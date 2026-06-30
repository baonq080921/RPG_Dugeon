# Balance Sheet — RPG Dungeon
**Ngày:** 22/06/2026  
**Engine:** Unity 2D — Mobile  
**Công thức tham chiếu:** `EntityStat.cs`, `PlayerLevel.cs`, `EnemyData.cs`, `CharacterData.cs`

---

## Mục lục
1. [Nguyên tắc cân bằng](#1-nguyên-tắc-cân-bằng)
2. [Player — Base Stats](#2-player--base-stats)
3. [Player — Starting Resources](#3-player--starting-resources)
4. [XP & Level Scaling](#4-xp--level-scaling)
5. [Major Stat — Điểm phân bổ theo cấp](#5-major-stat--điểm-phân-bổ-theo-cấp)
6. [Enemy Stats — Từng loại quái](#6-enemy-stats--từng-loại-quái)
7. [Skill Base Definitions](#7-skill-base-definitions)
8. [Skill Tree — Costs & Structure](#8-skill-tree--costs--structure)
9. [Kiểm chứng cân bằng (Sanity Check)](#9-kiểm-chứng-cân-bằng-sanity-check)

---

## 1. Nguyên tắc cân bằng

| Nguyên tắc | Mục tiêu |
|---|---|
| Player Level 1 giết SlimeSmall | 3–4 hit |
| SlimeSmall giết Player Level 1 | 12–15 hit (survivable) |
| Skeleton (Level 5) vs Player Level 3 | Nguy hiểm, cần dodge — 5–6 hit chết |
| Boss (Level 10) vs Player Level 6 | Rất nguy hiểm — 4 hit chết nếu không dodge |
| Crit không vượt quá 150% base damage trung bình | Để không phá vỡ balance |
| Evasion cap = 85%, Crit cap = 100% | Có trong code, giữ nguyên |
| Armor/Resistance dùng Diminishing returns: `K / (K + stat)` với K=100 | Prevent stacking phá game |

---

## 2. Player — Base Stats

> **Set trong Inspector:** các ScriptableObject `DefensiveStats`, `OffensiveStats`, `MajorStats`

### 2.1 DefensiveStats SO

| Stat | Base Value | Ghi chú |
|---|---|---|
| `MaxHealth` | **100** | HP thực tế = 100 + VIT×10 |
| `Armor` | **5** | Mitigation = 100/(100+5) = **95.2%** incoming dmg |
| `Evasion` | **3** | % né đòn = 3 + AGI×0.5, cap 85% |
| `ElementalResistance` | **0** | Bonus từ INT×0.5 |
| `HealthRegen` | **2** | Regen/tick, capped tại 5% MaxHP = 5 |
| `KnockBackThreshold` | **0.3** | Đòn > 30% HP → Heavy knockback |

### 2.2 OffensiveStats SO

| Stat | Base Value | Ghi chú |
|---|---|---|
| `Damage` | **15** | Dmg thực tế = 15 + STR |
| `CritChance` | **5** | % = 5 + AGI×0.3, cap 100% |
| `CritPower` | **150** | % = 150 + STR×0.5 |
| `AttackMultiplier` | **1.0** | Tốc độ animation attack = 1.0 + AGI×0.5 |
| `ElementalDamage` | **8** | Electric dmg = 8 + INT |

### 2.3 MajorStats SO

| Stat | Base Value | Ghi chú |
|---|---|---|
| `Strength` | **0** | Phân bổ khi level up |
| `Agility` | **0** | Phân bổ khi level up |
| `Intelligence` | **0** | Phân bổ khi level up |
| `Vitality` | **0** | Phân bổ khi level up |

### 2.4 CharacterData SO

| Field | Giá trị đề xuất | Ghi chú |
|---|---|---|
| `MoveSpeed` | **5.5** | |
| `JumpForce` | **12** | |
| `MaxJumpCount` | **2** | Double jump |
| `AirControlFactor` | **0.85** | Kiểm soát hướng khi trên không |
| `SlideDownSpeed` | **0.4** | Hệ số chậm khi wall slide |
| `WallJumpForce` | **(8, 14)** | x=8, y=14 |
| `DashSpeed` | **18** | |
| `DashDuration` | **0.2** | giây |
| `DashCooldown` | **2.5** | giây |
| `ComboLimit` | **3** | 3-hit combo |
| `TimeResetCombo` | **0.2** | giây không attack → reset combo |
| `ComboEndDelay` | **0.8** | Cooldown sau khi hết combo |
| `AirAttackCooldown` | **0.8** | Cooldown JumpAttack |
| `AttackVelocities` | **(4,0), (5,0), (6,1)** | Velocity cho hit 1, 2, 3 |

---

## 3. Player — Starting Resources

> **Set trong Inspector:** `PlayerInventory` component trên Player prefab

| Field | Giá trị | Lý do |
|---|---|---|
| `_startingMoney` | **50** | Đủ mua 1 item rẻ trong shop — khuyến khích khám phá ngay từ đầu |
| `_startingSkillPoints` | **3** | Đủ unlock 2 skill cơ bản (Dash + TimeEcho mỗi cái cost 1) và dư 1 điểm |
| `_inventoryMaxSize` | **12** | Đủ, không quá lớn cho mobile UI |

---

## 4. XP & Level Scaling

> **Công thức:** `ExpToNextLevel = baseExpRequired × Level ^ expGrowthRate`  
> **Set trong Inspector:** `PlayerLevel` component — `_baseExpRequired = 100`, `_expGrowthRate = 1.5`

| Level | XP cần để lên | Cộng dồn | Số quái cần giết (SlimeSmall 25 XP) |
|---|---|---|---|
| 1 → 2 | **100** | 100 | 4 con |
| 2 → 3 | **283** | 383 | 11 con |
| 3 → 4 | **520** | 903 | 21 con |
| 4 → 5 | **800** | 1,703 | 32 con |
| 5 → 6 | **1,118** | 2,821 | 45 con |
| 6 → 7 | **1,470** | 4,291 | 59 con |
| 7 → 8 | **1,852** | 6,143 | 74 con |
| 8 → 9 | **2,263** | 8,406 | 91 con |
| 9 → 10 | **2,700** | 11,106 | 108 con |

> **XP Formula khi player cao level hơn enemy:**  
> `FinalXP = BaseExp × Clamp(1 − (PlayerLevel − EnemyLevel) × 0.05, 0.1, 1)`  
> Ví dụ: Player Level 5 giết enemy Level 1 → nhận `BaseExp × 0.8` (giảm 20%)

---

## 5. Major Stat — Điểm phân bổ theo cấp

> Mỗi lần level up, player chọn **1 Major Stat** để tăng. Gợi ý allocation cho từng build:

| Level | STR build | AGI build | INT build | VIT build |
|---|---|---|---|---|
| 2 | +1 STR | +1 AGI | +1 INT | +1 VIT |
| 3 | +1 STR | +1 AGI | +1 INT | +1 VIT |
| 4 | +1 STR | +1 AGI | +1 INT | +1 VIT |
| 5 | +1 STR | +1 AGI | +1 INT | +1 VIT |

### Effect của 5 điểm STR (ví dụ)

| Stat | Tăng thêm |
|---|---|
| Damage | +5 |
| CritPower | +2.5% |
| Tổng DMG tăng | ~33% |

### Effect của 5 điểm VIT

| Stat | Tăng thêm |
|---|---|
| MaxHealth | +50 HP → tổng **150 HP** |
| Armor | +5 → tổng Armor = 10 → mitigation = 100/110 = **90.9%** |

---

## 6. Enemy Stats — Từng loại quái

> **Set trong:** `EnemyData` SO + `DefensiveStats` SO + `OffensiveStats` SO của từng enemy prefab

### 6.1 Slime Small *(Level 1 — Farm fodder)*

| Stat | Giá trị |
|---|---|
| Level | **1** |
| MaxHealth | **40** |
| Damage (base) | **8** |
| Armor | **0** |
| Evasion | **0%** |
| MoveSpeed | **2.5** |
| AttackRange | **1.0** |
| DetectionRange | **5** |
| BaseExp | **25** |
| StunDuration | **0.3s** |
| IdleTime | **1.5s** |

### 6.2 Slime Normal *(Level 2 — Bước đệm)*

| Stat | Giá trị |
|---|---|
| Level | **2** |
| MaxHealth | **70** |
| Damage (base) | **12** |
| Armor | **2** |
| Evasion | **2%** |
| MoveSpeed | **3.0** |
| AttackRange | **1.2** |
| DetectionRange | **6** |
| BaseExp | **40** |
| StunDuration | **0.3s** |
| IdleTime | **2.0s** |

### 6.3 Slime (Large) *(Level 3 — Challenging mob)*

| Stat | Giá trị |
|---|---|
| Level | **3** |
| MaxHealth | **110** |
| Damage (base) | **16** |
| Armor | **5** |
| Evasion | **4%** |
| MoveSpeed | **3.5** |
| MoveMultiplier (chase) | **1.5** |
| AttackRange | **1.5** |
| DetectionRange | **6.5** |
| BaseExp | **60** |
| StunDuration | **0.35s** |
| IdleTime | **2.0s** |

### 6.4 Skeleton *(Level 5 — Elite mob)*

| Stat | Giá trị |
|---|---|
| Level | **5** |
| MaxHealth | **160** |
| Damage (base) | **22** |
| Armor | **10** |
| Evasion | **8%** |
| MoveSpeed | **4.0** |
| MoveMultiplier (chase) | **1.5** |
| AttackRange | **1.8** |
| DetectionRange | **7.0** |
| BaseExp | **100** |
| StunDuration | **0.4s** |
| IdleTime | **2.5s** |
| minDistanceRetreat | **1.2** |

### 6.5 EnemyDeathRippler *(Level 10 — Boss)*

| Stat | Giá trị |
|---|---|
| Level | **10** |
| MaxHealth | **600** |
| Damage (base) | **40** |
| Armor | **20** |
| Evasion | **10%** |
| MoveSpeed | **4.5** |
| MoveMultiplier | **1.8** |
| JumpForce | **15** |
| AttackRange | **2.5** |
| DetectionRange | **12.0** |
| BaseExp | **500** |
| StunDuration | **0.15s** *(boss khó stun hơn)* |
| minDistanceRetreat | **1.5** |
| `SkillCoolDown` | **10s** |
| `SkillDuration` | **5s** |
| `SkillSpawnInterval` | **0.15s** |
| `SkillDamageFactor` | **1.5** |
| `_chanceToTeleport` | **0.25** (25%, +5% mỗi lần không tele) |

---

## 7. Skill Base Definitions

> **Set trong:** `SkillBaseDefinition` SO của từng skill — assign vào `SkillButtonHandler`

### 7.1 Dash

| Field | Base | Ghi chú |
|---|---|---|
| `Duration` | **0.2s** | Thời gian dash |
| `Cooldown` | **2.5s** | |
| `Damage` | **0** | Dash base không gây sát thương |
| `HealingAmount` | **0** | |
| `CDAmountPercent` | **0** | |
| `moveSpeed` | **0** | Dùng `DashSpeed` từ CharacterData |

### 7.2 Counter

| Field | Base | Ghi chú |
|---|---|---|
| `Duration` | **0.5s** | Cửa sổ parry |
| `Cooldown` | **5s** | |
| `Damage` | **0** | Sát thương do counter hit tự tính |
| `HealingAmount` | **0** | |

### 7.3 TimeEcho

| Field | Base | Ghi chú |
|---|---|---|
| `Duration` | **3.0s** | Echo tồn tại 3 giây |
| `Cooldown` | **8s** | |
| `Damage` | **12** | Sát thương của echo attack |
| `HealingAmount` | **0** | Upgrade HealOnEcho sẽ set giá trị này |
| `CDAmountPercent` | **0** | Upgrade HealOnEcho+Duration sẽ set |

### 7.4 Dismantle

| Field | Base | Ghi chú |
|---|---|---|
| `Duration` | **0.8s** | Animation + hiệu ứng AOE |
| `Cooldown` | **6s** | |
| `Damage` | **25** | AOE mạnh, cooldown dài bù đắp |
| `HealingAmount` | **0** | |

### 7.5 Domain Expansion (Ultimate)

| Field | Base | Ghi chú |
|---|---|---|
| `Duration` | **5s** | Thời gian domain active |
| `Cooldown` | **30s** | Rất dài — dùng đúng lúc mới hiệu quả |
| `Damage` | **50** | Sát thương AOE ultimate |
| `HealingAmount` | **0** | |

---

## 8. Skill Tree — Costs & Structure

> **Set trong:** `SkillTreeData` SO — field `Cost`  
> **Tổng điểm cần để unlock hết:** **32 skill points**  
> **Starting skill points:** 3 → player có thể unlock 2 skill tier 1 ngay lập tức

### Bố cục cây kỹ năng

```
[Dash] ─────────────────────────────────────────────────
  └── Dash Base (1pt)
        ├── Clone on Start (2pt)
        │     └── Clone Start + Arrival (3pt)          ← nhánh Clone
        └── Shard on Start (2pt)
              └── Shard Start + Arrival (3pt)          ← nhánh Shard

[TimeEcho] ──────────────────────────────────────────────
  └── TimeEcho Base (1pt)
        ├── Extra Echo Attack (2pt)
        │     └── Extra Echo Mahoraga (3pt)            ← nhánh tấn công
        └── Heal on Echo (2pt)
              └── Heal + Reduce CoolDown (3pt)         ← nhánh hỗ trợ

[Dismantle] ─────────────────────────────────────────────
  └── Dismantle Base (2pt)
        └── Dismantle Upgrade (3pt)

[Domain Expansion] ──────────────────────────────────────
  └── Domain Expansion (5pt)                           ← Ultimate, phải tích lũy
```

### Chi tiết từng node

| Node | SkillUpgrade | Cost | Effect |
|---|---|---|---|
| **Dash Base** | `Dash` | **1** | Mở khóa kỹ năng Dash |
| Clone on Start | `Dash_CloneOnStart` | **2** | Tạo phân thân tại điểm bắt đầu dash |
| Clone Start + Arrival | `Dash_CloneOnStartAndArrival` | **3** | Tạo phân thân cả đầu lẫn cuối |
| Shard on Start | `Dash_ShardOnStart` | **2** | Bắn projectile tại điểm bắt đầu *(nhánh thay thế)* |
| Shard Start + Arrival | `Dash_ShardOnStartAndArrival` | **3** | Bắn projectile cả 2 đầu *(nhánh thay thế)* |
| **TimeEcho Base** | `TimeEcho` | **1** | Mở khóa TimeEcho clone |
| Extra Echo Attack | `TimeEcho_ExtraEchoAttack` | **2** | Echo thực hiện thêm đòn side kick |
| Extra Echo Mahoraga | `TimeEcho_ExtraEchoAttackMaho` | **3** | Echo dùng đòn Mahoraga (damage cao hơn) |
| Heal on Echo | `TimeEcho_HealOnEcho` | **2** | Echo hồi HP khi kích hoạt *(nhánh thay thế)* |
| Heal + CD Reduce | `TimeEcho_HealOnEchoAndDuration` | **3** | Hồi HP + giảm CD tất cả skill *(nhánh thay thế)* |
| **Dismantle Base** | `Dismantle` | **2** | Mở khóa Dismantle AOE |
| Dismantle Upgrade | `Dismantle_Upgrade` | **3** | Tăng damage và phạm vi Dismantle |
| **Domain Expansion** | `Domain` | **5** | Ultimate — freeze + AOE cực mạnh |

### Skill Points Economy (ước tính toàn game)

| Nguồn | Số điểm |
|---|---|
| Bắt đầu game (`_startingSkillPoints`) | **3** |
| Chest Room 1 | **2** |
| Chest Room 2 | **2** |
| Chest Room 3 (boss room) | **3** |
| Quest reward | **3** |
| **Tổng ước tính** | **~13 điểm** |

> Player có thể unlock **8–9 nodes** trong một playthrough — cần chọn hướng build không unlock hết được cùng lúc.

---

## 9. Kiểm chứng cân bằng (Sanity Check)

### Player Level 1 vs SlimeSmall

| Phép tính | Kết quả |
|---|---|
| Player Damage = 15 + 0 STR | **15** |
| SlimeSmall Armor = 0 → mitigation = 100/100 | **1.0** |
| Damage sau mitigation | **15** |
| SlimeSmall HP = 40 | Chết sau **3 hit** ✅ |
| SlimeSmall Damage = 8 | |
| Player Armor = 5 → mitigation = 100/105 = 0.952 | |
| Player nhận = 8 × 0.952 | **7.6** |
| Player HP = 100 | Chết sau **13 hit** ✅ (survivable) |

### Player Level 3 (2 STR, 1 VIT) vs Skeleton Level 5

| Phép tính | Kết quả |
|---|---|
| Player Damage = 15 + 2 STR = 17 | |
| Skeleton Armor = 10 → mitigation = 100/110 = 0.909 | |
| Damage sau mitigation | **15.5** |
| Skeleton HP = 160 | Chết sau **~11 hit** ✅ |
| Skeleton Damage = 22 | |
| Player Armor = 5+1 VIT=6 → mitigation = 100/106 = 0.943 | |
| Player nhận = 22 × 0.943 | **20.7** |
| Player HP = 100 + 10 VIT = 110 | Chết sau **~5 hit** ✅ (nguy hiểm, cần dodge) |

### Player Level 6 (3 STR, 2 AGI, 1 VIT) vs Boss Level 10

| Phép tính | Kết quả |
|---|---|
| Player Damage = 15 + 3 STR = 18 | |
| Boss Armor = 20 → mitigation = 100/120 = 0.833 | |
| Damage sau mitigation | **15** |
| Boss HP = 600 | Chết sau **~40 hit** ✅ (trận dài, cần skill) |
| Boss Damage = 40 | |
| Player Armor = 5+1 VIT=6 → mitigation = 100/106 = 0.943 | |
| Player nhận = 40 × 0.943 | **37.7** |
| Player HP = 100 + 10 VIT = 110 | Chết sau **~3 hit** ⚠️ (rất nguy hiểm — đúng với Boss) |

### Diminishing Returns — Armor Scaling Table

| Armor tổng | Mitigation | % Giảm damage |
|---|---|---|
| 0 | 1.000 | 0% |
| 5 | 0.952 | 4.8% |
| 10 | 0.909 | 9.1% |
| 20 | 0.833 | 16.7% |
| 50 | 0.667 | 33.3% |
| 100 | 0.500 | 50% *(hard cap thực tế)* |
| 200 | 0.333 | 66.7% |

> Sau 100 armor, mỗi điểm thêm chỉ cho lợi nhuận rất nhỏ — tránh tank build bất khả chiến bại ✅

---

## 10. Đối chiếu Sheet ↔ Asset thực tế *(mismatch log — 28/06/2026)*

> Các số ở Mục 1–9 là **thiết kế mục tiêu**. Khi đọc giá trị thật trong `.asset`, nhiều chỗ đã **lệch**. Cần đồng bộ trước khi tin vào sanity-check.

### 10.1 Skill Tree — Cost thật trong asset ≠ Mục 8

| Node (asset) | Cost asset | Cost sheet (Mục 8) | Lệch |
|---|---|---|---|
| QuickDash | 1 | 1 | ✅ |
| EchoStep / Upgrade | 2 / 3 | 2 / 3 | ✅ |
| TimeLostShard / Upgrade | 2 / 3 | 2 / 3 | ✅ |
| TimeEcho | 1 | 1 | ✅ |
| EchoTimeLessAttack | **5** | 2 | ❌ |
| EchoTimeMahoAttack | **8** | 3 | ❌ |
| HealingWeep | 2 | 2 | ✅ |
| TimeEchoHealingCD | **4** | 3 | ❌ |
| Dismantle / Rewind | **3 / 6** | 2 / 3 | ❌ |
| DomainExpansion | **10** | 5 | ❌ |

- **Tổng tất cả node = 50 SP.** Nhưng có **4 node mang nhánh xung khắc** (`confilctNode`) nên KHÔNG mua được hết.
- Một build hoàn chỉnh (chọn 1 nhánh mỗi chỗ xung khắc):
  - Nhẹ nhất (TimeEcho đi nhánh Heal): **32 SP**
  - Nặng nhất (TimeEcho đi nhánh Maho-Attack): **39 SP**
- → **Ngân sách SP mục tiêu tại boss = 40** (đủ cho build nặng nhất + đệm).

### 10.2 Enemy — Level/Exp/Drop thật ≠ Mục 6

| Enemy (asset) | Level thật | BaseExp thật | SkillPoint | Gold | Sheet nói Level |
|---|---|---|---|---|---|
| EnemySkeleton | **1** | 20 | 5 | 10 | 5 ❌ |
| EnemyArcher | **1** | 20 | 5 | 20 | *(thiếu trong sheet)* ❌ |
| EnemySlimeData (big) | 2 | 50 | 5 | 15 | 3 ❌ |
| EnemySlime Normal | 1 | 25 | 5 | 10 | 2 ❌ |
| EnemySlime Small | 1 | 12 | 3 | 5 | 1 ✅ |
| **EnemyDeathRippler (BOSS)** | **1** ⚠️ | 20 | 1 | 10 | 10 ❌❌ |

**Vấn đề lớn nhất:**
1. **Boss đang Level 1, BaseExp 20** — không phải L10/500 như sanity-check. Toàn bộ phép tính độ khó ở Mục 9 KHÔNG áp dụng cho asset hiện tại.
2. **Mọi quái cho 5 SkillPoint/mạng.** Chỉ ~8 mạng là đủ full skill tree (40 SP). Hiện 3 phòng có ~15 quái → ~75 SP → **thừa gần gấp đôi**.
3. **Archer** không có trong sheet nhưng lại là quái chính ở Room2/3.

### 10.3 Kinh tế SP — Mục 8 lỗi thời

Mục 8 giả định SP đến từ rương + quest (~13 điểm). Thực tế **enemy drop 5 SP/mạng** (`EnemyDrop.DropGoldAndSkillPoint`), nên nguồn SP chủ yếu là **giết quái**, không phải rương. → Cần tính lại theo số quái (xem Mục 11).

---

## 11. Bố trí quái theo phòng — Room Pacing & Budget *(28/06/2026)*

> **Mục tiêu:** đến khi vào phòng boss, player **vừa đủ**: (a) SP full skill tree, (b) Gold mua đồ chuẩn bị, (c) Level đủ để boss "vừa đủ khó".

### 11.1 Ngân sách mục tiêu tại cửa boss

| Tài nguyên | Mục tiêu | Khởi điểm | Cần từ dungeon |
|---|---|---|---|
| **Skill Point** | 40 (full build nặng nhất) | 3 (`_startingSkillPoints`) | **~37** |
| **Gold** | ~200 (chi ~180 mua đồ boss) | 50 (`_startingMoney`) | **~150** |
| **Player Level** | **L4** (3 lần chọn stat) — xem ghi chú | L1 | **903 XP** cộng dồn |

> **Ghi chú Level:** sanity-check Mục 9 nhắm L6 (2.821 XP) — **không thực tế** cho dungeon 4 phòng ít quái (cần ~35–40 mạng). Với ~18 quái, mốc khả thi là **L4** (903 XP). Muốn L6 thì phải ~2× số quái hoặc tăng mạnh BaseExp.

### 11.2 Drop đề xuất mỗi loại quái *(set trong `EnemyData`)*

> Hạ **SkillPoint/mạng từ 5 → 2–3** để SP "vừa đủ" thay vì thừa.

| Loại | Level | SkillPoint | Gold | BaseExp |
|---|---|---|---|---|
| Skeleton (thường) | 1 | **2** | 8 | 45 |
| Archer | 2–3 | **2** | 10 | 55 |
| Slime Normal | 2 | **2** | 8 | 45 |
| Slime Large | 3 | **3** | 15 | 70 |
| Skeleton Elite | 4 | **3** | 15 | 70 |
| **Boss Rippler** | **4–5** ⚠️ | 1 | 10 | 400 |

### 11.3 Số lượng & level quái mỗi phòng

> Quy mô map: nhỏ → ít quái, lớn → nhiều quái. (Hiện trạng quét được: Room1=6 skel, Room2=1 skel+2 archer+3 slime, Room3=1 skel+2 archer, Room4=boss.)

| Phòng | Quy mô | Đội hình đề xuất | Σ quái | SP | Gold | XP |
|---|---|---|---|---|---|---|
| **Room 1** (mở màn) | Nhỏ | 5× Skeleton L1 | 5 | 10 | 40 | 225 |
| **Room 2** | Vừa | 4× (Skel/Slime L2) + 2× Archer L2 | 6 | 12 | 52 | 290 |
| **Room 3** (sát boss) | Lớn | 3× (Slime/Archer L3) + 2× Skeleton Elite L4 + 2× Archer L3 | 7 | 16 | 74 | 385 |
| **Room 4** | Boss | 1× Rippler L4–5 (+minion last-attack) | — | — | — | — |
| **CỘNG** | | | **18** | **38** | **166** | **900** |

### 11.4 Kiểm chứng "vừa đủ"

| Tài nguyên | Khởi điểm + drop | Mục tiêu | Kết luận |
|---|---|---|---|
| Skill Point | 3 + 38 = **41** | 32–39 (full build) | ✅ vừa đủ + đệm nhỏ |
| Gold | 50 + 166 = **216** | chi ~180 | ✅ dư ~36 |
| XP | **900** | 903 (L4) | ✅ chạm L4 ngay cửa boss |

### 11.5 Việc cần làm để áp dụng

1. **Boss `EnemyData`:** đổi Level 1 → **4–5**, BaseExp 20 → **400** (để boss đáng giá & độ khó đúng). Tinh chỉnh HP/Damage ở `RipplerOffensiveStats`/`RipplerDefensiveStat` theo Mục 9.
2. **Hạ SkillPoint** trong `EnemyData` mọi quái: 5 → **2** (elite 3) — nếu không SP thừa gấp đôi.
3. **Set Level quái tăng dần theo phòng** (1 → 2 → 3–4) để XP không bị phạt bởi công thức `1 − (PL−EL)×0.05` và giữ độ khó.
4. **Thêm 3 quái** so với hiện tại (15 → 18) và phân bố lại như bảng 11.3.
5. Cập nhật Mục 6 & 8 cho khớp asset (hoặc sửa asset cho khớp sheet) — hiện đang lệch.

> **Đòn bẩy tinh chỉnh:** muốn khó hơn → giảm Gold/SP mỗi mạng hoặc giảm số quái; muốn dễ hơn → tăng. Giữ tỉ lệ XP để mốc Level đúng kỳ vọng.
