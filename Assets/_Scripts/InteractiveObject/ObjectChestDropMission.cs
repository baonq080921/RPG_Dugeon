using Base;
using UnityEngine;

namespace InteractiveObject
{
    public class ObjectChestDropMission : ObjectChestBase
    {
        [SerializeField] private int _skillPointReward;
        [SerializeField] private bool _triggerQuestEvent = true;

        protected override void DropChestItem()
        {
            base.DropChestItem();

            if (_triggerQuestEvent)
                EventBus<NpcRescuedEvent>.Raise(new NpcRescuedEvent());

            if (_skillPointReward > 0)
                EventBus<SkillPointRewardEvent>.Raise(new SkillPointRewardEvent(_skillPointReward));
        }
    }
}
