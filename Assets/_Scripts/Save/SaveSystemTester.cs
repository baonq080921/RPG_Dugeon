using Base;
using UnityEngine;

namespace Save
{
    /// <summary>
    /// Temporary tester — attach to any GameObject in the scene, use the context-menu buttons.
    /// Delete this component when the save system is confirmed working.
    /// </summary>
    public class SaveSystemTester : MonoBehaviour
    {
        [ContextMenu("Test Save")]
        private void TestSave()
        {
            var mgr = ServiceLocator.Get<SaveManager>();
            if (mgr == null) { Debug.LogError("[Tester] SaveManager not in ServiceLocator."); return; }
            mgr.Save();
            Debug.Log($"[Tester] Save done. File exists: {mgr.HasSave()}");
        }

        [ContextMenu("Test Load")]
        private void TestLoad()
        {
            var mgr = ServiceLocator.Get<SaveManager>();
            if (mgr == null) { Debug.LogError("[Tester] SaveManager not in ServiceLocator."); return; }
            mgr.Load();
        }

        [ContextMenu("Test New Game")]
        private void TestNewGame()
        {
            var mgr = ServiceLocator.Get<SaveManager>();
            if (mgr == null) { Debug.LogError("[Tester] SaveManager not in ServiceLocator."); return; }
            string startScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"[Tester] Starting new game → reloading '{startScene}' with all progress cleared.");
            mgr.StartNewGame(startScene);
        }

        [ContextMenu("Print Save Path")]
        private void PrintSavePath()
        {
            Debug.Log($"[Tester] Save file: {System.IO.Path.Combine(Application.persistentDataPath, "save.json")}");
        }
    }
}
