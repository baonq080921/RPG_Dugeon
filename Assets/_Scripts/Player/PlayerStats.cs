using Base;
using UnityEngine;
namespace player
{
    public class PlayerStats : EntityStat
    {

        private EventBinding<ResetStats> eventResetBinding;
        void OnEnable()
        {
            eventResetBinding = new EventBinding<ResetStats>(ResetAllStats);
            EventBus<ResetStats>.Register(eventResetBinding);
        }

        void ODisable()
        {
            EventBus<ResetStats>.Deregister(eventResetBinding);
        }


    }
}