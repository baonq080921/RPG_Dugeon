#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using Stats;

namespace RPGTests
{
    /// <summary>
    /// Pure-logic unit tests for the <see cref="Stat"/> value/modifier model.
    /// Technique: Equivalence Partitioning over the modifier operations.
    /// </summary>
    public class StatTests
    {
        [Test]
        public void GetValue_AfterReset_ReturnsBaseValue()
        {
            var stat = TestStatFactory.MakeStat(20f);
            Assert.AreEqual(20f, stat.GetValue(), 0.001f);
            Assert.AreEqual(20f, stat.GetBaseValue(), 0.001f);
        }

        [Test]
        public void AddModifier_IncreasesValue_WithoutChangingBase()
        {
            var stat = TestStatFactory.MakeStat(20f);
            stat.AddModifier(5f, "buff");
            Assert.AreEqual(25f, stat.GetValue(), 0.001f);
            Assert.AreEqual(20f, stat.GetBaseValue(), 0.001f, "base value must stay immutable");
        }

        [Test]
        public void RemoveModifier_ReversesAnAddModifier()
        {
            var stat = TestStatFactory.MakeStat(20f);
            stat.AddModifier(5f, "buff");
            stat.RemoveModifier(5f, "buff");
            Assert.AreEqual(20f, stat.GetValue(), 0.001f);
        }

        [Test]
        public void Reset_DiscardsAccumulatedModifiers()
        {
            var stat = TestStatFactory.MakeStat(20f);
            stat.AddModifier(15f, "buff");
            stat.Reset();
            Assert.AreEqual(20f, stat.GetValue(), 0.001f);
        }
    }
}
#endif
