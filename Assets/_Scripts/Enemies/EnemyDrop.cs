using Base;
using UnityEngine;

namespace enemy
{
    
    public class EnemyDrop : EntityDrop
    {
        private Enemy _enemy;

        void Awake()
        {
            _enemy =GetComponent<Enemy>();
        }

        public void DropGoldAndSkillPoint()
        {
            float gold = _enemy.enemyData.Gold;
            float skillPoint = _enemy.enemyData.SkillPoint;
            EventBus<MoneyAddRewardEvent>.Raise(new MoneyAddRewardEvent(gold));
            EventBus<SkillPointRewardEvent>.Raise(new SkillPointRewardEvent(skillPoint));
        }
    }
}