using Base;
using entity;
using Save;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Tracks player level and experience. Attach to the Player GameObject alongside other player components.
    /// Listens for <see cref="EnemyDiedEvent"/> and applies the XP formula:
    /// FinalXP = BaseXP * Clamp(1 - (PlayerLevel - EnemyLevel) * 0.05, 0.1, 1)
    /// Raises <see cref="PlayerLevelUpEvent"/> on each level up so the UI can prompt a major-stat choice.
    /// </summary>
    public class PlayerLevel : MonoBehaviour
    {
        [field: SerializeField] public int   Level        { get; private set; } = 1;
        [field: SerializeField] public float CurrentExp   { get; private set; } = 0f;
        [field: SerializeField] public float ExpToNextLevel { get; private set; }

        [SerializeField] private float _baseExpRequired = 100f;
        [SerializeField] private float _expGrowthRate   = 1.5f;

        private EntityStat _stat;
        private EntityVfx _entityVfx;
        private EventBinding<EnemyDiedEvent> _enemyDiedBinding;

        private void Awake()
        {
            _stat = GetComponent<EntityStat>();
            _entityVfx = GetComponent<EntityVfx>();
            ExpToNextLevel = CalculateExpToNextLevel();
        }

        private void OnEnable()
        {
            _enemyDiedBinding = new EventBinding<EnemyDiedEvent>(OnEnemyDied);
            EventBus<EnemyDiedEvent>.Register(_enemyDiedBinding);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Deregister(_enemyDiedBinding);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            float finalXP = e.BaseExp * Mathf.Clamp(1f - (Level - e.EnemyLevel) * 0.05f, 0.1f, 1f);
            AddExp(finalXP);
        }

        private void AddExp(float amount)
        {
            CurrentExp += amount;
            bool leveledUp = false;
            while (CurrentExp >= ExpToNextLevel)
            {
                CurrentExp -= ExpToNextLevel;
                LevelUp();
                leveledUp = true;
            }
            if (!leveledUp)
                EventBus<PlayerXPChangedEvent>.Raise(new PlayerXPChangedEvent(CurrentExp, ExpToNextLevel, Level));
        }

        private void LevelUp()
        {
            Level++;
            ExpToNextLevel = CalculateExpToNextLevel();
            _entityVfx.CreateEffectLevelVFx();
            EventBus<PlayerLevelUpEvent>.Raise(new PlayerLevelUpEvent(_stat));
            // Reset bar to 0 for the new level; carry-over XP will appear on the next gain.
            EventBus<PlayerXPChangedEvent>.Raise(new PlayerXPChangedEvent(0f, ExpToNextLevel, Level));
        }

        private float CalculateExpToNextLevel()
        {
            return _baseExpRequired * Mathf.Pow(Level, _expGrowthRate);
        }


        public void RestoreExp(PlayerSaveData data)
        {
            CurrentExp     = data.currentExp;
            Level          = data.currentLevel;
            ExpToNextLevel = data.expToNextLevel;
            EventBus<PlayerXPChangedEvent>.Raise(new PlayerXPChangedEvent(CurrentExp, ExpToNextLevel, Level));
        }
    }
}
