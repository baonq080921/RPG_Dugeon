using Base;
using Interfaces;
using player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public UISkillToolTip uISkillToolTip;
        public UISkillTree uISkillTree;
        public UI_Inventory uIInventory;
        public UICanvasChange uICanvasChange;

        private void Awake()
        {
            if (ServiceLocator.Get<UIManager>() != null) return;

            if (uISkillToolTip == null)
                uISkillToolTip = GetComponentInChildren<UISkillToolTip>();
            if (uISkillTree == null)
                uISkillTree = GetComponentInChildren<UISkillTree>();
            if (uIInventory == null)
                uIInventory = GetComponentInChildren<UI_Inventory>();
            if (uICanvasChange == null)
                uICanvasChange = GetComponentInChildren<UICanvasChange>();
            ServiceLocator.Register(this);

            // Expose the skill tree behind ISkillTreeState so the persistence (player) layer can
            // save/restore unlocked nodes without referencing UI types (breaks player ↔ UI).
            if (uISkillTree != null)
                ServiceLocator.Register<ISkillTreeState>(uISkillTree);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool hasPlayer = FindObjectOfType<Player>() != null;
            gameObject.SetActive(hasPlayer);
        }
    }
}
