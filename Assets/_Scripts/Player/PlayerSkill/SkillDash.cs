using UnityEngine;
namespace player
{
    /// <summary>
    /// Dash skill. Executes an upgrade-specific effect when the player dashes.
    /// </summary>
    public class SkillDash : SkillBase
    {
        /// <inheritdoc/>
        public override void ExecuteSkillEffect()
        {
            switch (skillUpgrade)
            {
                case SkillUpgrade.Dash_CloneOnStart:
                    Debug.Log("Create a clone on start");
                    break;
                case SkillUpgrade.Dash_CloneOnStartAndArrival:
                    Debug.Log("Create a clone at start and arrival");
                    break;
                case SkillUpgrade.Dash_ShardOnStart:
                    Debug.Log("Create a shard on start");
                    break;
                case SkillUpgrade.Dash_ShardOnStartAndArrival:
                    Debug.Log("Create a shard at start and arrival");
                    break;
            }
        }
    }
}