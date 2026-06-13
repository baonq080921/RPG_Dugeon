using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace player
{
    public class PlayerCombat : EntityCombat
    {
        public event Action OnPlayerAttacking;
        /// <summary>Fired for each enemy the player's attack lands on.</summary>
        public event Action<Transform> OnPlayerHitEnemy;
        private Player _player;
        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
        }

        public override void PerformedAttack()
        {
            base.PerformedAttack();
            if (!IsdetectTargetColliders()) return;

            OnPlayerAttacking?.Invoke();
            foreach (var target in targetColliders)
            {
                if (target.GetComponent<IHit>() != null)
                    OnPlayerHitEnemy?.Invoke(target.transform);
            }
        }

    
        public bool IsPerformedCounter()
        {
            if(!IsdetectTargetColliders()) return false;
            bool isCounter = false;
            foreach(var target in targetColliders)
            {
                ICounterable counter = target.GetComponent<ICounterable>();
                IHit hit = target.GetComponent<IHit>();
                if(counter == null) continue;

                if (counter.CanCounter)
                {
                    counter?.HandleCounter();
                    isCounter = true;
                }
            }
            return isCounter;
        }
    }
}
