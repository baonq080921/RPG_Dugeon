#if UNITY_INCLUDE_TESTS
using Interfaces;
using NUnit.Framework;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPGTests
{
    /// <summary>
    /// Unit tests for damage application on the real <see cref="EntityHealth"/> pipeline:
    /// mitigation, lethal-hit death, and HP clamping.
    /// Technique: Equivalence Partitioning + Boundary Value Analysis.
    /// </summary>
    public class EntityHealthTests
    {
        /// <summary>Minimal concrete entity so <see cref="EntityHealth"/> can resolve Die().</summary>
        private class TestEntity : Entity
        {
            public override LayerMask LayerMask => 0;
        }

        /// <summary>Minimal concrete health component (EntityHealth has no abstract members).</summary>
        private class TestHealth : EntityHealth { }

        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
        }

        private TestHealth BuildHealth(float maxHealth, float armor, float evasion)
        {
            _go = new GameObject("HealthRig");
            _go.AddComponent<TestEntity>();

            var entityStat = _go.AddComponent<EntityStat>();
            TestStatFactory.InjectPrivate(entityStat, "_entityType", EntityType.Enemy);
            TestStatFactory.InjectPrivate(entityStat, "_majorStats", TestStatFactory.MakeMajorStats());
            TestStatFactory.InjectPrivate(entityStat, "_defensiveStats",
                TestStatFactory.MakeDefensiveStats(maxHealth: maxHealth, armor: armor, evasion: evasion));
            TestStatFactory.InjectPrivate(entityStat, "_offensiveStats", TestStatFactory.MakeOffensiveStats());
            entityStat.ResetAllStats();

            // Health bar slider must exist before the health component's Awake runs.
            var sliderGo = new GameObject("Slider");
            sliderGo.transform.SetParent(_go.transform);
            sliderGo.AddComponent<Slider>();

            var health = _go.AddComponent<TestHealth>();
            health.ResetHealth();
            return health;
        }

        [Test]
        public void TakeDamage_AppliesArmorMitigation()
        {
            // TC-DMG-01: armor 100 => mitigation 0.5; 40 raw damage removes 20 HP
            var health = BuildHealth(maxHealth: 100, armor: 100, evasion: 0);

            bool hit = health.TakeDamage(40f, 0f, ElementType.None, null);

            Assert.IsTrue(hit);
            Assert.AreEqual(80f, health.CurrentHealth, 0.001f);
        }

        [Test]
        public void TakeDamage_LethalHit_ClampsToZeroAndDies()
        {
            // TC-DMG-04 / BVA: overkill damage clamps HP at 0 and triggers death
            var health = BuildHealth(maxHealth: 100, armor: 0, evasion: 0);
            var entity = _go.GetComponent<TestEntity>();

            health.TakeDamage(250f, 0f, ElementType.None, null);

            Assert.AreEqual(0f, health.CurrentHealth, 0.001f);
            Assert.IsTrue(entity.isDead);
        }

        [Test]
        public void HealHP_DoesNotExceedMaxHealth()
        {
            // TC-DMG-08 / BVA: healing past max clamps to max
            var health = BuildHealth(maxHealth: 100, armor: 0, evasion: 0);
            health.TakeDamage(50f, 0f, ElementType.None, null); // -> 50

            health.HealHP(9999f);

            Assert.AreEqual(100f, health.CurrentHealth, 0.001f);
        }

        [Test]
        public void RestoreHealth_ClampsWithinValidRange()
        {
            // Save-load path uses RestoreHealth; value above max must clamp to max
            var health = BuildHealth(maxHealth: 100, armor: 0, evasion: 0);

            health.RestoreHealth(9999f);

            Assert.AreEqual(100f, health.CurrentHealth, 0.001f);
        }

        [Test]
        public void TakeDamage_NegativeDamage_MustNotHealAboveMax()
        {
            // TC-DMG-07 (DEFECT): negative damage is not validated, so ReduceHP
            // computes Max(0, 100 - (-50)) = 150, healing the entity beyond its max.
            // This assertion encodes the CORRECT expected behaviour and is expected
            // to FAIL until TakeDamage clamps negative input. See Testing report §X.5.
            var health = BuildHealth(maxHealth: 100, armor: 0, evasion: 0);

            health.TakeDamage(-50f, 0f, ElementType.None, null);

            Assert.LessOrEqual(health.CurrentHealth, 100f,
                "Negative damage must never raise HP above MaxHealth.");
        }
    }
}
#endif
