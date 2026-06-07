using UnityEngine;
namespace player
{
    
    /// <summary>
    /// Base class for all player skills. A skill starts locked; call <see cref="Unlock"/> to
    /// allow the player to activate it. Upgrading via the skill tree changes <see cref="skillUpgrade"/>
    /// which controls which variant runs.
    /// </summary>
    public class SkillBase : MonoBehaviour
    {
        [field:Header("General Details")]
        [field:SerializeField] public SkillBaseDefinition SkillBaseDefinition {get; private set;}
        [field:SerializeField] public SkillType skillType {get; private set;}
        [field:SerializeField] public SkillUpgrade skillUpgrade {get; private set;}
        [SerializeField] protected Player player;


        /// <summary>True once the player has unlocked this skill via the skill tree.</summary>
        public bool IsUnlocked { get; private set; }

        protected virtual void Awake(){}

        protected virtual void OnEnable()
        {
            UISkillTree.OnReset += ResetSkill;
        }

        protected virtual void OnDisable()
        {
            UISkillTree.OnReset -= ResetSkill;
        }
        /// <summary>Opens the activation gate for this skill.</summary>
        public void Unlock() => IsUnlocked = true;
        public void Lock() => IsUnlocked = false;

        /// <summary>
        /// Executes the skill's active effect. Called by the associated state when the skill activates.
        /// Override in each concrete skill class to implement upgrade-specific behavior.
        /// </summary>
        public virtual void ExecuteSkillEffect() { }

        private void ResetSkill()
        {
            Lock();
        }

        public void SetSkillUpgradeType(SkillUpgrade upgrade)
        {
            skillUpgrade = upgrade;
        }

        public virtual void SetUpgradeForSkill(UpgradeData upgrade)
        {
            SkillBaseDefinition.SetToUpgradeDurationAndCoolDown(upgrade.UpgradeCoolDown, upgrade.UpgradeDuration);
        }
    }
}