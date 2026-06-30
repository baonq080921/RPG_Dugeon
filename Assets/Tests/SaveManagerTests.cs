#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.IO;
using Base;
using NUnit.Framework;
using Save;
using SaveData;
using UnityEngine;

namespace RPGTests
{
    /// <summary>
    /// Unit tests for <see cref="SaveManager"/> persistence logic: in-memory quest /
    /// scene-state tracking and the on-disk save path.
    /// Technique: Equivalence Partitioning + State Transition.
    /// The real player save file is backed up before the run and restored afterwards
    /// so these tests never destroy actual game progress.
    /// </summary>
    public class SaveManagerTests
    {
        private class FakePlayerPersistent : IIPlayerPersistent
        {
            public PlayerSaveData Data = new PlayerSaveData { sceneName = "Room0", currentHealth = 37f };
            public PlayerSaveData GetPlayerData() => Data;
            public IEnumerator RestoreFromSaveData(PlayerSaveData data, bool restorePosition) { yield break; }
        }

        private string _savePath;
        private string _backupPath;
        private GameObject _go;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "save.json");
            _backupPath = _savePath + ".testbak";
            if (File.Exists(_savePath))
                File.Copy(_savePath, _backupPath, overwrite: true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Restore the player's real save (or remove any test-created file).
            if (File.Exists(_backupPath))
            {
                File.Copy(_backupPath, _savePath, overwrite: true);
                File.Delete(_backupPath);
            }
            else if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
        }

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("SaveManagerRig");
            _go.AddComponent<SaveManager>(); // Awake() registers itself in ServiceLocator
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
        }

        private SaveManager Manager => ServiceLocator.Get<SaveManager>();

        // ---- In-memory quest / scene state (no disk) -------------------------

        [Test]
        public void MarkQuestComplete_ThenQuestIsReportedComplete()
        {
            Manager.MarkQuestComplete("Room2");
            Assert.IsTrue(Manager.IsQuestComplete("Room2"));
        }

        [Test]
        public void IsQuestComplete_UnknownScene_ReturnsFalse()
        {
            Assert.IsFalse(Manager.IsQuestComplete("UnknownScene"));
        }

        [Test]
        public void SetQuestProgress_IsReadBack()
        {
            // TC-SAVE-06: stored numeric quest progress is retrievable
            Manager.SetQuestProgress("Room2", 3);
            Assert.AreEqual(3, Manager.GetQuestProgress("Room2"));
        }

        [Test]
        public void GetQuestProgress_UnknownScene_DefaultsToZero()
        {
            Assert.AreEqual(0, Manager.GetQuestProgress("UnknownScene"));
        }

        [Test]
        public void MarkEntityPersisted_ThenEntityIsPersisted()
        {
            Manager.MarkEntityPersisted("Room1", "chest_01");
            Assert.IsTrue(Manager.IsEntityPersisted("Room1", "chest_01"));
        }

        [Test]
        public void IsEntityPersisted_UnknownEntity_ReturnsFalse()
        {
            Assert.IsFalse(Manager.IsEntityPersisted("Room1", "never_touched"));
        }

        // ---- On-disk save ----------------------------------------------------

        [Test]
        public void Save_WritesFileToDisk_AndHasSaveBecomesTrue()
        {
            // TC-SAVE-01: Save() with a registered player snapshot produces a save file
            ServiceLocator.Register<IIPlayerPersistent>(new FakePlayerPersistent());
            if (File.Exists(_savePath)) File.Delete(_savePath);

            Manager.Save();

            Assert.IsTrue(File.Exists(_savePath));
            Assert.IsTrue(Manager.HasSave());
        }

        [Test]
        public void Load_CorruptSaveFile_ShouldFailGracefully()
        {
            // TC-SAVE-07 (DEFECT): a corrupted save file makes JsonUtility.FromJson throw.
            // Load() does not wrap deserialization in try/catch, so the exception
            // propagates and would crash the game. This assertion encodes the CORRECT
            // expected behaviour (graceful handling) and is expected to FAIL until
            // Load() guards the deserialize call. See Testing report §X.5.
            File.WriteAllText(_savePath, "this is not valid json {{{{");

            Assert.DoesNotThrow(() => Manager.Load(),
                "Load() must handle a corrupted save file without throwing.");
        }
    }
}
#endif
