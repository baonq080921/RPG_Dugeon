# Game Balance Plan — RPG Dungeon (YuJi)

## Formulas (reference everything else against these)

```
Physical Damage   = Damage + Strength
Crit Chance       = CritChance + Agility × 0.3        (cap 100%)
Crit Power        = CritPower + Strength × 0.5        → multiplier applied on crit
Max HP            = MaxHealth + Vitality × 10
Armor Mitigation  = 100 / (100 + Armor + Vitality)    → damage-taken multiplier
Elem Mitigation   = 100 / (100 + ElemRes + Intel×0.5)
Evasion           = Evasion + Agility × 0.5            (cap 85%)
HP Regen/sec      = min(HealthRegen, MaxHP × 0.05)

Dismantle damage  = skillDamage + playerPhys × 0.5
Domain tick       = skillDamage + physDamage × scaleFactor
                  + skillDamage + elemDamage × (scaleFactor × 0.3)   ← weaker side
```

> **⚠ Crit Power formula — verify before setting values:**
> The code in EntityStat.cs → GetPhysicalDamageValue may read:
> `finalDamage = totalDamage + (critPower / 100)` — at value 5 this adds 0.05 (useless).
> It should be `totalDamage * (critPower / 100)` so that critPower = 150 means 1.5× on crit.
> Read the file and fix the formula if needed before applying CritPower = 150.

---

## Player (YuJi) — Starting ScriptableObject Values

### OffensiveStatsYuJi.asset

| Stat            | Current | Recommended |
|-----------------|---------|-------------|
| Damage          | 1000    | **20**      |
| CritChance      | 1%      | **5%**      |
| CritPower       | 5       | **150**     |
| AttackMultiplier| 1       | **1.0**     |
| ElementalDamage | 0       | **0**       |

### DefensiveStatYuJi.asset

| Stat                 | Current | Recommended |
|----------------------|---------|-------------|
| MaxHealth            | 1000    | **100**     |
| Armor                | 90      | **10**      |
| Evasion              | 10%     | **10%**     |
| ElementalResistance  | 90%     | **10%**     |
| KnockBackThreshold   | 0.5     | **0.5**     |
| HealthRegen          | 0       | **1**       |

### MajorStat.asset

| Stat         | Current | Recommended |
|--------------|---------|-------------|
| Strength     | 0       | **8**       |
| Agility      | 2       | **3**       |
| Intelligence | 0       | **0**       |
| Vitality     | 10      | **10**      |

### Effective Starting Combat Stats (after formulas)

```
Physical Damage  = 20 + 8 = 28 per hit
Crit Chance      = 5 + 3×0.3 = 5.9%
Crit Multiplier  = 150 + 8×0.5 = 154%  → crit hit = 28 × 1.54 = 43
Max HP           = 100 + 10×10 = 200
Armor mitigation = 100 / (100+10+10) = 0.833  → takes 83% of physical hits
Evasion          = 10 + 3×0.5 = 11.5%
Elem mitigation  = 100 / (100+10+0) = 0.909   → takes 91% of elemental hits
HP Regen         = min(1, 200×0.05) = 1/sec
```

---

## Enemy Values

### Skeleton — Tier 1 (Level 1 area)

| Stat                 | Current | Recommended |
|----------------------|---------|-------------|
| MaxHealth            | 1500    | **120**     |
| Damage               | 30      | **30**      |
| ElementalDamage      | 0       | **0**       |
| Armor                | 20      | **0**       |
| Evasion              | 0%      | **0%**      |
| ElementalResistance  | 50%     | **20%**     |
| Strength/Agility/Intel/Vitality | 0/0/0/0 | **0/0/0/0** |
| BaseExp              | 20      | **20**      |

```
Player vs Skeleton:
  28 damage (armor = 0, no reduction) → dies in ceil(120/28) = 5 hits
  With 1 crit in combo: 4 × 28 + 1 × 43 → kills in 4 hits ✓
Skeleton vs Player:
  30 × 0.833 = 25 per hit → kills player in 200/25 = 8 hits ✓
```

### Slime Normal — Tier 1 (Level 1 area)

| Stat                 | Current | Recommended |
|----------------------|---------|-------------|
| MaxHealth            | 2000    | **80**      |
| Damage               | 20      | **15**      |
| ElementalDamage (Ice)| 40      | **25**      |
| Armor                | 2       | **0**       |
| Evasion              | 20%     | **20%**     |
| ElementalResistance  | 10%     | **0%**      |
| BaseExp              | 50      | **15**      |

```
Player vs Slime:
  28 damage (no armor) → dies in ceil(80/28) = 3 hits
  20% evasion → ~3.75 effective hits ✓ (feels slippery)
Slime vs Player:
  Phys:  15 × 0.833 = 12.5
  Ice:   25 × 0.909 = 22.7   (player 10% elem resist)
  Total ≈ 35/hit → kills player in 200/35 = 5.7 hits
  → More dangerous than Skeleton per hit (punishes no elemental resist build)
```

### Slime Small — Tier 1 (mini / group summon)

| Stat                 | Recommended |
|----------------------|-------------|
| MaxHealth            | **40**      |
| Damage               | **8**       |
| ElementalDamage (Ice)| **12**      |
| Armor                | **0**       |
| Evasion              | **25%**     |
| ElementalResistance  | **0%**      |
| BaseExp              | **8**       |

```
Dies in 1-2 player hits. Dangerous in groups: 3 small slimes ≈ 1 normal slime threat.
```

### Skeleton Elite — Tier 2 (Level 3 area)

| Stat                 | Recommended |
|----------------------|-------------|
| MaxHealth            | **350**     |
| Damage               | **50**      |
| Armor                | **15**      |
| Evasion              | **0%**      |
| ElementalResistance  | **30%**     |
| BaseExp              | **60**      |

```
Assumes player at Level 3 (STR 10, VIT 12 from 2 level-ups):
  Player damage: 30 → hits elite for 30 × (100/115) = 26.1 → dies in 350/26 ≈ 13 hits
  Elite hits player (HP 220, Armor 22): 50 × (100/122) = 41 → dies in 220/41 ≈ 5 hits
  → Requires skill use. Domain Expansion trivializes it (correct for first real test).
```

### Slime Boss — Tier 2 (Level 3 area)

| Stat                 | Recommended |
|----------------------|-------------|
| MaxHealth            | **500**     |
| Damage               | **30**      |
| ElementalDamage (Ice)| **50**      |
| Armor                | **5**       |
| Evasion              | **30%**     |
| ElementalResistance  | **0%**      |
| BaseExp              | **80**      |

---

## Skill Values

### Dismantle

**SkillDefinition-Dismatle.asset**

| Value           | Current | Recommended |
|-----------------|---------|-------------|
| Damage          | 30      | **50**      |
| Cooldown        | 12s     | **10s**     |
| CDAmountPercent | 0.5     | **0.5**     |
| moveSpeed       | 15      | **15**      |
| Duration        | 0.5s    | **0.5s**    |

```
Effective damage = 50 + 28×0.5 = 64   (≈ 2.3× basic attack)
2 hits on different enemies = 2×0.5 = 100% CD reduction → full reset ✓
```

**SkillTree-DismantleRewind.asset** (upgrade)

| Value          | Current | Recommended |
|----------------|---------|-------------|
| UpgradeDamage  | 90      | **100**     |
| UpgradeCoolDown| 12s     | **10s**     |
| UpgradeCD      | 0.5     | **0.5**     |

```
With upgrade: 100 + 28×0.5 = 114   (≈ 4× basic attack)
```

---

### TimeEcho Clone

**SkillDefinition-TimeEcho.asset**

| Value    | Current | Recommended |
|----------|---------|-------------|
| Damage   | 50      | **25**      |
| Duration | 15s     | **12s**     |
| Cooldown | 1s      | **1s**      |

```
Clone hits for 25 ≈ 0.9× player basic attack.
Adds roughly 25-50% extra DPS alongside player. Feels like a powerful companion, not overpowered.
```

**SkillTree-TimeEchoAttack.asset** (upgrade)

| Value           | Current | Recommended |
|-----------------|---------|-------------|
| UpgradeDamage   | 100     | **50**      |
| UpgradeCoolDown | 15s     | **12s**     |
| UpgradeDuration | 3s      | **5s**      |

---

### Domain Expansion

**SkillTree-Domain.asset**

| Value           | Current | Recommended |
|-----------------|---------|-------------|
| UpgradeDamage   | 300     | **150**     |
| UpgradeCoolDown | 180s    | **180s**    |
| UpgradeDuration | 3s      | **5s**      |
| UpgradeHealing  | 300     | **0**       |

**DomainExpansionSkill.cs** — find the `SetUpDamageForDomain(...)` call and set:
```csharp
scaleFactor = 1.5f
```

```
Per tick  = 150 + 28×1.5 = 192 (physical — YuJi has 0 elemental so elemental side = 150+0 = 150)
Total tick = 192 + 150 = 342

5 ticks over 5 seconds = 1710 total AOE damage
  → Instantly kills all Tier 1 enemies (120 HP) on first tick ✓
  → Kills Tier 2 Skeleton (350 HP) by tick 2 ✓
  → Deals heavy burst on boss (design bosses with 2000–5000 HP)
  → Appropriate power level for a 3-minute ultimate ✓
```

---

## Level Progression (Level 1–10)

Each level-up grants **2 major stat points**. Recommended physical build path: **+1 STR, +1 VIT per level**.

| Level | STR | VIT | Phys Damage | Max HP | Armor | Mitigation |
|-------|-----|-----|-------------|--------|-------|------------|
| 1     | 8   | 10  | 28          | 200    | 20    | 83.3%      |
| 2     | 9   | 11  | 29          | 210    | 21    | 82.6%      |
| 3     | 10  | 12  | 30          | 220    | 22    | 82.0%      |
| 5     | 12  | 14  | 32          | 240    | 24    | 80.6%      |
| 8     | 15  | 17  | 35          | 270    | 27    | 78.7%      |
| 10    | 17  | 19  | 37          | 290    | 29    | 77.5%      |

---

## Implementation Checklist

- [ ] Verify CritPower formula in `EntityStat.cs → GetPhysicalDamageValue`
- [ ] Update `Assets/ScriptableObject/Character/YuJI/OffensiveStatsYuJi.asset`
- [ ] Update `Assets/ScriptableObject/Character/YuJI/DefensiveStatYuJi.asset`
- [ ] Update `Assets/ScriptableObject/Character/YuJI/MajorStat.asset`
- [ ] Update `Assets/ScriptableObject/Enemy/EnemySkeleton/` (Offensive + Defensive + Major + EnemyData)
- [ ] Update `Assets/ScriptableObject/Enemy/EnemySlime/SlimeOffensiveStats.asset`
- [ ] Update `Assets/ScriptableObject/Enemy/EnemySlime/SlimeDefensiveStat.asset`
- [ ] Update `Assets/ScriptableObject/Enemy/EnemySlime/SlimeNormal/` variant
- [ ] Update `Assets/ScriptableObject/Enemy/EnemySlime/SlimeSmall/` variant
- [ ] Update `Assets/ScriptableObject/SKill/Skill-Dismantle/SkillDefinition-Dismatle.asset`
- [ ] Update `Assets/ScriptableObject/SKill/Skill-Dismantle/SkillTree-DismantleRewind.asset`
- [ ] Update `Assets/ScriptableObject/SKill/Skill-TimeEcho/SkillDefinition-TimeEcho.asset`
- [ ] Update `Assets/ScriptableObject/SKill/Skill-TimeEcho/SkillTree-TimeEchoAttack.asset`
- [ ] Update `Assets/ScriptableObject/SKill/SkillDomain/SkillTree-Domain.asset`
- [ ] Set `scaleFactor = 1.5f` in `DomainExpasionSkill.cs`
