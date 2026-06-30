using System;
using Base;
using Interfaces;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Base enemy entity. Holds all states and player-detection helpers used by every enemy type.
    /// </summary>
    public class Enemy : Entity, ICounterable, IScenePersistable
    {
        [field: SerializeField] public EnemyData enemyData { get; private set; }

        /// <inheritdoc/>
        public override LayerMask LayerMask => enemyData.WhatIsPlayer;
        /// <inheritdoc/>

        public EnemyState enemyIdleState { get; protected set; }
        public EnemyState enemyMoveState { get; protected set; }
        public EnemyState enemyChaseState { get; protected set; }
        public EnemyState enemyAttackState { get; protected set; }
        public EnemyState enemyStunState { get; protected set; }

        /// <summary>Scene-unique identifier generated automatically per scene instance. Never changes after first assignment.</summary>
        [SerializeField] private string _sceneEntityId;
        public string SceneEntityId => _sceneEntityId;
        

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Skip the prefab asset itself — only assign IDs to scene instances so every
            // placed enemy gets its own GUID rather than inheriting one from the prefab.
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) return;
            if (string.IsNullOrEmpty(_sceneEntityId))
            {
                _sceneEntityId = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        /// <summary>The player transform found by the last <see cref="IsPlayerDetected"/> call.</summary>
        public Transform DetectedPlayer { get; private set; }

        /// <inheritdoc/>
        public event Action OnPersisted;

        /// <summary>Invokes <see cref="OnPersisted"/>. Call from subclasses when the enemy's persistent state changes.</summary>
        protected void RaiseOnPersisted() => OnPersisted?.Invoke();
        public bool CanCounter { get ; set ; }

        public float moveSpeed{get; private set;}
        public float attackSpeed {get; private set;}

        private EnemyDrop _enemyDrop;
        protected override void Awake()
        {
            base.Awake();
            _enemyDrop = GetComponent<EnemyDrop>();
            InitializeStates();
        }

        /// <summary>
        /// Creates all state instances for this enemy. Override in subclasses to supply enemy-specific states.
        /// Called at the end of <see cref="Awake"/>, before <see cref="Start"/>.
        /// </summary>
        protected virtual void InitializeStates()
        {
            enemyIdleState = new EnemyIdleState(this, stateMachine, "Idle");
            enemyMoveState = new EnemyMoveState(this, stateMachine, "Move");
            enemyChaseState = new EnemyChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemyAttackState(this, stateMachine, "Attack");
            enemyStunState = new EnemyStunState(this, stateMachine, "Hit");
        }

        protected override void Start()
        {
            base.Start();
            stateMachine.Initialize(enemyIdleState);
            moveSpeed = enemyData.MoveSpeed;
            attackSpeed = entityStat.GetAttackMultiplier();
        }

        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Returns true when the player is within <see cref="EnemyData.DetectionRange"/>.
        /// Caches the result in <see cref="DetectedPlayer"/>.
        /// </summary>
        public bool IsPlayerDetected()
        {
            var col = Physics2D.OverlapCircle(transform.position, enemyData.DetectionRange, enemyData.WhatIsPlayer);
            if (col == null)
            {
                DetectedPlayer = null;
                return false;
            }

            Vector2 toPlayer = col.transform.position - transform.position;
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, _whatIsWall);
            if (wallHit.collider != null)
            {
                DetectedPlayer = null;
                return false;
            }

            DetectedPlayer = col.transform;
            return true;
        }


        public float DistanceToPlayer()
        {
            if (DetectedPlayer == null) return float.MaxValue;
            float distance = Vector2.Distance(DetectedPlayer.position, transform.position);
            return Mathf.Abs(distance);
        }

        /// <summary>Applies a vertical jump impulse using <see cref="EnemyData.JumpForce"/>.</summary>
        public void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, enemyData.JumpForce);
        }

        /// <summary>Returns true when the player is within <see cref="EnemyData.AttackRange"/>.</summary>
        public bool IsInAttackRange()
        {
            return Physics2D.OverlapCircle(transform.position, enemyData.AttackRange, enemyData.WhatIsPlayer);
        }
        /// <summary>
        /// Make enemy invincible when doing some action
        /// </summary> <summary>
        /// 
        /// </summary>
        public override void UnTargetableEnemy(bool canTarget)
        {
            if (canTarget)
                this.gameObject.layer = LayerMask.NameToLayer("Untargetable");
            else
                this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        }



        public bool ShouldEnemyRetreat()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y-0.8f), new Vector2(direction, 0), enemyData.minDistanceRetreat, enemyData.WhatIsPlayer);
            Collider2D collider2D = raycastHit2D.collider;
            if(collider2D == null) return false;

            Entity entity = collider2D.GetComponent<Entity>();
            if(entity != null)
            {
                float distance = Mathf.Abs(transform.position.x - entity.transform.position.x);
                // DebugCustom.Log(distance.ToString());
                return distance < enemyData.minDistanceRetreat ? true : false;
            }
            return false;
        }


        public void FacePlayer()
        {
            if (DetectedPlayer == null) return;
            float newDir = DetectedPlayer.position.x > transform.position.x ? 1f : -1f;
            if (newDir == direction) return;
            SetDirection(newDir);
            Flip(newDir);
        }

        public override void Die()
        {
            isDead = true;
            _enemyDrop.DropItems();
            _enemyDrop.DropGoldAndSkillPoint();
            EventBus<OnEnemyDiedEvent>.Raise(new OnEnemyDiedEvent(enemyData.Level, enemyData.BaseExp));
            RaiseOnPersisted();
            EventBus<EnemyDiedForBossEvent>.Raise(new EnemyDiedForBossEvent(this));
        }

        /// <inheritdoc/>
        public void RestoreState() => gameObject.SetActive(false);

        public virtual void ChangeToDiedState(){}
        public virtual void SpecialAttack(){}


        public override void ApplyEffect(float scaleFactor, ElementType elementType)
        {
            if(elementType == ElementType.Electric)
            {
                
            }
            if(elementType == ElementType.Ice)
            {
                moveSpeed = moveSpeed - moveSpeed*scaleFactor;
            }
        }

        public override void ResetEffect()
        {
            base.ResetEffect();
            moveSpeed = enemyData.MoveSpeed;
        }


        

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.DetectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y -0.8f), new Vector3(transform.position.x + (direction * enemyData.minDistanceRetreat), transform.position.y -0.8f));
        }

        /// <summary>
        /// Resets the enemy to its initial state when retrieved from the pool.
        /// Called automatically by <see cref="EnemyPool"/> — do not call directly.
        /// </summary>
        public void OnGetFromPool()
        {
            isDead = false;
            col.enabled = true;
            animator.enabled = true;
            ResetKnockbackState();
            rb.velocity = Vector2.zero;
            GetComponent<EntityHealth>().ResetHealth();
            ResetAllAnimatorBools();
            stateMachine.Initialize(enemyIdleState);
        }

        /// <summary>
        /// Returns this enemy to the <see cref="EnemyPool"/> registered in <see cref="ServiceLocator"/>.
        /// Falls back to <see cref="Object.Destroy"/> if no pool is registered.
        /// </summary>
        public void ReturnToPool()
        {
            var pool = ServiceLocator.Get<EnemyPool>();
            if (pool == null)
            {
                Destroy(gameObject);
                return;
            }
            pool.Return(this);
        }

        public void EnableCounter()
        {
            throw new System.NotImplementedException();
        }

        public void DisableCounter()
        {
            throw new System.NotImplementedException();
        }

        public void HandleCounter()
        {
            if(CanCounter == false) return;
            stateMachine.ChangeState(enemyStunState);
        }
    }
}