using Base;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Base enemy entity. Holds all states and player-detection helpers used by every enemy type.
    /// </summary>
    public class Enemy : Entity
    {
        [field: SerializeField] public EnemyData enemyData { get; private set; }

        public EnemyIdleState enemyIdleState { get; protected set; }
        public EnemyMoveState enemyMoveState { get; protected set; }
        public EnemyChaseState enemyChaseState { get; protected set; }
        public EnemyAttackState enemyAttackState { get; protected set; }

        /// <summary>The player transform found by the last <see cref="IsPlayerDetected"/> call.</summary>
        public Transform DetectedPlayer { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            enemyIdleState = new EnemyIdleState(this, stateMachine, "Idle");
            enemyMoveState = new EnemyMoveState(this, stateMachine, "Move");
            enemyChaseState = new EnemyChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemyAttackState(this, stateMachine, "Attack");
        }

        protected override void Start()
        {
            base.Start();
            stateMachine.Initialize(enemyIdleState);
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

        /// <summary>Applies a vertical jump impulse using <see cref="EnemyData.JumpForce"/>.</summary>
        public void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, enemyData.JumpForce);
        }

        /// <summary>Returns true when the player is within <see cref="EnemyData.AttackRange"/>.</summary>
        public bool IsPlayerInAttackRange()
        {
            return Physics2D.OverlapCircle(transform.position, enemyData.AttackRange, enemyData.WhatIsPlayer);
        }



        public bool ShouldEnemyRetreat()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, new Vector2(direction, 0), enemyData.minDistanceRetreat, enemyData.WhatIsPlayer);
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

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.DetectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.AttackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + (direction * enemyData.minDistanceRetreat), transform.position.y));
        }

    }
}
