using Base;
using entity;
using UnityEngine;
namespace player
{
    public class PlayerStats : EntityStat
    {

        private EventBinding<ResetStats> _eventResetBinding;
        void OnEnable()
        {
            _eventResetBinding = new EventBinding<ResetStats>(ResetAllStats);
            EventBus<ResetStats>.Register(_eventResetBinding);
            


        }

        void OnDisable()
        {
            EventBus<ResetStats>.Deregister(_eventResetBinding);


        }
    }
}