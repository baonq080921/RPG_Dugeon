using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace player
{
    public class PlayerCombat : EntityCombat
    {
        private Player _player;
        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
        }

    
        public bool IsPerformedCounter()
        {
            DetectTargetColliders();

            bool isCounter = false;
            foreach(var target in targetColliders)
            {
                ICounterable counter = target.GetComponent<ICounterable>();
                IHit hit = target.GetComponent<IHit>();
                if(counter == null) continue;

                if (counter.CanCounter)
                {
                    counter?.HandleCounter();
                    hit?.TakeDamage(_player.Data.CounterDamage, _player.CanKnockBackOnHit,target.transform);
                    isCounter = true;
                }
            }
            return isCounter;
        }
    }
}
