#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using Stats;
using UnityEngine;

namespace RPGTests
{
    /// <summary>
    /// Unit tests for the combat-math core (<see cref="EntityStat"/>): damage mitigation,
    /// evasion / crit caps, derived health, and level-up stat allocation.
    /// Technique: Equivalence Partitioning + Boundary Value Analysis + Decision Table.
    /// </summary>
    public class EntityStatTests
    {
        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
        }

        private EntityStat Build(EntityType type, MajorStats major, DefensiveStats def, OffensiveStats off)
        {
            var stat = TestStatFactory.MakeEntityStat(type, major, def, off);
            _go = stat.gameObject;
            return stat;
        }

        // ---- Function 1: damage / defense math -------------------------------

        [Test]
        public void GetHealthValue_BaseHealthPlusVitalityBonus()
        {
            // TC-STAT-HEALTH: maxHealth 100 + vitality 5 * 10 = 150
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(vit: 5),
                TestStatFactory.MakeDefensiveStats(maxHealth: 100),
                TestStatFactory.MakeOffensiveStats());

            Assert.AreEqual(150f, stat.GetHealthValue(), 0.001f);
        }

        [Test]
        public void GetMitigation_Armor100_HalvesIncomingDamage()
        {
            // TC-DMG-01: mitigation = 100 / (100 + armor); armor 100 => 0.5
            var stat = Build(EntityType.Enemy,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(armor: 100),
                TestStatFactory.MakeOffensiveStats());

            Assert.AreEqual(0.5f, stat.GetMigiationValue(), 0.001f);
        }

        [Test]
        public void GetMitigation_ZeroArmor_NoReduction()
        {
            // BVA: armor 0 => mitigation = 1 (full damage taken)
            var stat = Build(EntityType.Enemy,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(armor: 0),
                TestStatFactory.MakeOffensiveStats());

            Assert.AreEqual(1f, stat.GetMigiationValue(), 0.001f);
        }

        [Test]
        public void GetEvasion_IsClampedTo85Percent()
        {
            // TC-DMG-06 / BVA: configured evasion 999 must clamp to the 85% cap
            var stat = Build(EntityType.Enemy,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(evasion: 999),
                TestStatFactory.MakeOffensiveStats());

            Assert.AreEqual(85f, stat.GetEnvasionValue(), 0.001f);
        }

        [Test]
        public void GetPhysicalDamage_ZeroCritChance_ReturnsBaseDamageNoCrit()
        {
            // Decision Table (crit branch = false): damage 10 + strength 4 = 14, isCrit false
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(str: 4),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats(damage: 10, critChance: 0));

            float dmg = stat.GetPhysicalDamageValue(out bool isCrit);
            Assert.AreEqual(14f, dmg, 0.001f);
            Assert.IsFalse(isCrit);
        }

        [Test]
        public void GetElementalDamage_PlayerNormal_IsElectric()
        {
            // Decision Table: PlayerNormal => Electric, value = elementalDamage 5 + intelligence 3 = 8
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(intel: 3),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats(elementalDamage: 5));

            float elem = stat.GetElementalDamageValue(out ElementType element);
            Assert.AreEqual(ElementType.Electric, element);
            Assert.AreEqual(8f, elem, 0.001f);
        }

        [Test]
        public void GetElementalDamage_PlainEnemy_DealsNoElemental()
        {
            // Decision Table: a plain Enemy deals 0 elemental damage, element None
            var stat = Build(EntityType.Enemy,
                TestStatFactory.MakeMajorStats(intel: 3),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats(elementalDamage: 5));

            float elem = stat.GetElementalDamageValue(out ElementType element);
            Assert.AreEqual(ElementType.None, element);
            Assert.AreEqual(0f, elem, 0.001f);
        }

        [Test]
        public void GetCritChanceDisplay_IsClampedTo100()
        {
            // BVA: critChance 90 + agility 100 * 0.3 = 120 must clamp to 100
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(agi: 100),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats(critChance: 90));

            Assert.AreEqual(100f, stat.GetCritChanceDisplayValue(), 0.001f);
        }

        // ---- Function 3: stat allocation on level-up -------------------------

        [Test]
        public void AddVitalityPoint_IncreasesMaxHealthBy5()
        {
            // TC-STAT-01: +1 Vitality applies MaxHealth +5 (armor +1) via VitalityLevelUp
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(maxHealth: 100),
                TestStatFactory.MakeOffensiveStats());

            float before = stat.GetHealthValue();
            stat.AddMajorStatPoint(StatType.Vitality);
            Assert.AreEqual(before + 5f, stat.GetHealthValue(), 0.001f);
        }

        [Test]
        public void AddStrengthPoint_IncreasesDamageByOne()
        {
            // TC-STAT-02: +1 Strength applies Damage +1 via StrenghLevelUp
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats(damage: 10));

            float before = stat.GetDamageDisplayValue();
            stat.AddMajorStatPoint(StatType.Strength);
            Assert.AreEqual(before + 1f, stat.GetDamageDisplayValue(), 0.001f);
        }

        [Test]
        public void AddStrengthPoint_ThreeTimes_TracksAllocatedPoints()
        {
            // TC-STAT-05: allocated-points counter reflects three allocations
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats());

            stat.AddMajorStatPoint(StatType.Strength);
            stat.AddMajorStatPoint(StatType.Strength);
            stat.AddMajorStatPoint(StatType.Strength);
            Assert.AreEqual(3, stat.GetAllocatedPoints(StatType.Strength));
        }

        [Test]
        public void ResetAllStats_ClearsAllocatedPoints()
        {
            // TC-STAT-06: ResetAllStats returns allocation counters to zero
            var stat = Build(EntityType.PlayerNormal,
                TestStatFactory.MakeMajorStats(),
                TestStatFactory.MakeDefensiveStats(),
                TestStatFactory.MakeOffensiveStats());

            stat.AddMajorStatPoint(StatType.Agility);
            stat.ResetAllStats();
            Assert.AreEqual(0, stat.GetAllocatedPoints(StatType.Agility));
        }
    }
}
#endif
