#if UNITY_INCLUDE_TESTS
using System.Reflection;
using Stats;
using UnityEngine;

namespace RPGTests
{
    /// <summary>
    /// Test helper that builds real <see cref="Stat"/> / stat ScriptableObjects.
    /// The production stat fields use private setters and a private <c>baseValue</c>
    /// backing field, so tests inject values through reflection rather than the
    /// (non-existent) public constructors — this keeps the tests exercising the
    /// real production types instead of duplicated fakes.
    /// </summary>
    public static class TestStatFactory
    {
        private static readonly FieldInfo BaseValueField =
            typeof(Stat).GetField("baseValue", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>Creates a real <see cref="Stat"/> with the given base value, already reset.</summary>
        public static Stat MakeStat(float baseValue)
        {
            var stat = new Stat();
            BaseValueField.SetValue(stat, baseValue);
            stat.Reset();
            return stat;
        }

        /// <summary>Sets an auto-property backing field (e.g. <c>Strength</c>) on a stat SO.</summary>
        public static void SetProp(object target, string propertyName, Stat value)
        {
            var field = target.GetType().GetField($"<{propertyName}>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        /// <summary>Builds a <see cref="MajorStats"/> SO with the four major stats.</summary>
        public static MajorStats MakeMajorStats(float str = 0, float agi = 0, float intel = 0, float vit = 0)
        {
            var so = ScriptableObject.CreateInstance<MajorStats>();
            SetProp(so, "Strength", MakeStat(str));
            SetProp(so, "Agility", MakeStat(agi));
            SetProp(so, "Intelligence", MakeStat(intel));
            SetProp(so, "Vitality", MakeStat(vit));
            return so;
        }

        /// <summary>Builds a <see cref="DefensiveStats"/> SO. Armor and evasion default to 0.</summary>
        public static DefensiveStats MakeDefensiveStats(
            float maxHealth = 100, float armor = 0, float evasion = 0,
            float elementalResistance = 0, float knockBackThreshold = 0.5f, float healthRegen = 0)
        {
            var so = ScriptableObject.CreateInstance<DefensiveStats>();
            SetProp(so, "MaxHealth", MakeStat(maxHealth));
            SetProp(so, "Amor", MakeStat(armor));
            SetProp(so, "Envasion", MakeStat(evasion));
            SetProp(so, "ElementalResitance", MakeStat(elementalResistance));
            SetProp(so, "KnockBackThreshHold", MakeStat(knockBackThreshold));
            SetProp(so, "HealthRegen", MakeStat(healthRegen));
            return so;
        }

        /// <summary>Builds an <see cref="OffensiveStats"/> SO.</summary>
        public static OffensiveStats MakeOffensiveStats(
            float damage = 10, float critPower = 150, float critChance = 0,
            float attackMultiplier = 1, float elementalDamage = 0)
        {
            var so = ScriptableObject.CreateInstance<OffensiveStats>();
            SetProp(so, "Damage", MakeStat(damage));
            SetProp(so, "CritPower", MakeStat(critPower));
            SetProp(so, "CritChance", MakeStat(critChance));
            SetProp(so, "AttackMultiplier", MakeStat(attackMultiplier));
            SetProp(so, "ElementalDamage", MakeStat(elementalDamage));
            return so;
        }

        /// <summary>
        /// Builds a GameObject with a fully wired <see cref="EntityStat"/> from the given stat SOs.
        /// The EntityStat private SO fields are injected by reflection.
        /// </summary>
        public static EntityStat MakeEntityStat(EntityType type, MajorStats major, DefensiveStats def, OffensiveStats off)
        {
            var go = new GameObject("TestEntityStat");
            var entityStat = go.AddComponent<EntityStat>();
            InjectPrivate(entityStat, "_entityType", type);
            InjectPrivate(entityStat, "_majorStats", major);
            InjectPrivate(entityStat, "_defensiveStats", def);
            InjectPrivate(entityStat, "_offensiveStats", off);
            entityStat.ResetAllStats();
            return entityStat;
        }

        /// <summary>Sets a private serialized field on a MonoBehaviour by name.</summary>
        public static void InjectPrivate(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
#endif
