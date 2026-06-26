using System;
using Base;
using entity;
using UnityEngine;

namespace player
{
    /// <inheritdoc/>
    public class PlayerHealth : EntityHealth
    {
        public event Action OnPlayerTalkingDamage;
        private Player _player;
        private EventBinding<PlayerAddHealthAmountEvent> _healBinding;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
            // InvokeRepeating(nameof(RegenerateHealth), 0f, 1f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _healBinding = new EventBinding<PlayerAddHealthAmountEvent>(OnHealReceived);
            EventBus<PlayerAddHealthAmountEvent>.Register(_healBinding);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<PlayerAddHealthAmountEvent>.Deregister(_healBinding);
        }

        private void OnHealReceived(PlayerAddHealthAmountEvent healEvent) => HealHP(healEvent.Amount);



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
