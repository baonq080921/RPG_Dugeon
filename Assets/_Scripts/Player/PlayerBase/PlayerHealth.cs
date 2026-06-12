using System;
using Base;
using UnityEngine;

namespace player
{
    /// <inheritdoc/>
    public class PlayerHealth : EntityHealth
    {
        public event Action OnPlayerTalkingDamage;
        private Player _player;
        private EventBinding<PlayerAddHealthAmount> _healBinding;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
            InvokeRepeating(nameof(RegenerateHealth), 0f, 1f);
        }

        protected virtual void OnEnable()
        {
            _healBinding = new EventBinding<PlayerAddHealthAmount>(OnHealReceived);
            EventBus<PlayerAddHealthAmount>.Register(_healBinding);
        }

        protected virtual void OnDisable()
        {
            EventBus<PlayerAddHealthAmount>.Deregister(_healBinding);
        }

        private void OnHealReceived(PlayerAddHealthAmount healEvent) => HealHP(healEvent.Amount);



        /// <inheritdoc/>
        public override bool TakeDamage(float damage,float elementalDamage,ElementType elementType,Transform target)
        {
            bool canDamage = base.TakeDamage(damage,elementalDamage,elementType,target);
            if (!canDamage) return false;   
            if(_player.isDead) return false;
            _player.ApplyKnockBack(damage);
            OnPlayerTalkingDamage?.Invoke();
            _player.stateMachine.ChangeState(_player.playerKnockBackState);
            return true;
        }
    }
}
