using System.Collections;
using Base;
using enemy;
using player;
using Unity.VisualScripting;
using UnityEngine;
namespace enemy
{
    /// <summary>Boss enemy with its own dedicated state set.</summary>
    public class EnemyDeathRippler : Enemy
    {
        public EnemyDeathRipplerDeathState enemyDeathState { get; private set; }
        public EnemyDeathRipplerBattleState enemyDeathRipplerBattleState { get; private set; }
        public EnemyDeathRipplerTeleportState enemyDeathRipplerTeleportState { get; private set; }
        public EnemyDeathRipplerTeleportBackState enemyDeathRipplerTeleportBackState { get; private set; }
        public EnemyDeathRipplerUltimateState enemyDeathRipplerUltimateState {get; private set;}
        private float _offsetY = 2;
        [SerializeField] private float _chanceToTeleport = 0.25f;
        private float _defaultChanceTeleport;

        [SerializeField] private Collider2D _aeraCollider;
        [SerializeField] private float _teleportBehindOffset = 2f;
        public bool IsTeleportTriggered {get; private set;}
        public bool HasAggro { get; private set; }
        private Vector3? _overrideTeleportPoint;
        private Coroutine _specialAttackCoroutine;
        [SerializeField] private EnemyDeathRipplerUlt _enemyDeathRipplerUltPrefab;
        public bool canUlt {get; private set;}
        private float _ultCoolDownTimer;
        [SerializeField] private float _ultSpawnOffSet ; 
        private float offsetY = 2f;
        private Player _player;
    
        protected override void Awake()
        {
            base.Awake();
            _defaultChanceTeleport = _chanceToTeleport;
            _ultCoolDownTimer = enemyData.SkillCoolDown;
            canUlt = false;
        }
        protected override void InitializeStates()
        {
            enemyIdleState = new EnemyDeathRipplerIdleState(this, stateMachine, "Idle");
            enemyChaseState = new EnemyDeathRipplerChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemyDeathRipplerAttackState(this, stateMachine, "Attack");
            enemyStunState = new EnemyDeathRipplerStunState(this, stateMachine, "Hit");
            enemyDeathState = new EnemyDeathRipplerDeathState(this, stateMachine, "Died");
            enemyDeathRipplerBattleState = new EnemyDeathRipplerBattleState(this, stateMachine, "Battle");
            enemyDeathRipplerTeleportState = new EnemyDeathRipplerTeleportState(this, stateMachine, "CanTelePort");
            enemyDeathRipplerTeleportBackState = new EnemyDeathRipplerTeleportBackState(this, stateMachine, "CanTelePort");
            enemyDeathRipplerUltimateState = new EnemyDeathRipplerUltimateState(this, stateMachine,"canUlt");
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.M))
            {
                transform.position = FindTeleportPoint();
            }

            TickUltCoolDown();
        }

        private void TickUltCoolDown()
        {
            if(_ultCoolDownTimer <= 0) {
                canUlt = true;
                return;
            }
            _ultCoolDownTimer -= Time.deltaTime;
        }

        public void ResetUltCoolDownTick() 
        {
            _ultCoolDownTimer = enemyData.SkillCoolDown;
            canUlt = false;
        }
        public void StopSpecialAttack()
        {
            if(_specialAttackCoroutine == null) return;
            StopCoroutine(_specialAttackCoroutine);
            _specialAttackCoroutine = null;
        }

        public override void SpecialAttack()
        {
            base.SpecialAttack();
            if(_specialAttackCoroutine != null) StopCoroutine(_specialAttackCoroutine);
            _specialAttackCoroutine = StartCoroutine(SpecialAttackCoroutine());

        }

        IEnumerator SpecialAttackCoroutine()
        {
            _player = ServiceLocator.Get<Player>();
            while (true)
            {
                var ult = Instantiate(_enemyDeathRipplerUltPrefab);
                ult.SetUpUltimateSkill(entityStat.GetPhysicalDamageValue(out _),entityStat.GetElementalDamageValue(out _), enemyData.SkillDamageFactor);
                var offset = _player.direction > 0 ? _ultSpawnOffSet : _ultSpawnOffSet * _player.direction;
                ult.transform.position = new Vector3(_player.transform.position.x + offset,_player.transform.position.y + _offsetY);
                yield return new WaitForSeconds(enemyData.SkillSpawnInterval);

            }
        }


        public override void ChangeToDiedState()
        {
            base.ChangeToDiedState();
            stateMachine.ChangeState(enemyDeathState);
        }


        public bool ShouldTelePort()
        {
            if(Random.Range(0f,1f) < _chanceToTeleport)
            {
            _chanceToTeleport = _defaultChanceTeleport;
            return true;
            }
                _chanceToTeleport += 0.05f; // increase 5% for each time boss can teleport
                return false;
        }

        public void SetTeleportTriggered(bool isTrigger) => IsTeleportTriggered = isTrigger;

        public void TriggerAggro() => HasAggro = true;
        /// <summary>
        /// Caculate the positon to teleport use this for skill that need to close enemy or doing something that need to close to enemy
        /// 
        /// </summary> <summary>
        /// 
        /// </summary>

        public void PrepareBehindPlayerTeleport()
        {
            if (DetectedPlayer == null) return;
            // direction points from boss toward player, so adding it past the player puts us on the far side
            float targetX = DetectedPlayer.position.x + direction * _teleportBehindOffset;
            Vector2 raycastPos = new Vector2(targetX, _aeraCollider.bounds.max.y);
            RaycastHit2D ray = Physics2D.Raycast(raycastPos, Vector2.down, Mathf.Infinity, _whatIsGround);
            if (ray.collider != null)
                _overrideTeleportPoint = ray.point + new Vector2(0, _offsetY);
        }

        public Vector3 FindTeleportPoint()
        {
            if (_overrideTeleportPoint.HasValue) // check if we caculate the land point
            {
                Vector3 dest = _overrideTeleportPoint.Value;
                _overrideTeleportPoint = null;
                return dest;
            }
            //if not precaculate the enemy to teleport just randomly make it tele in size the area we define
            int maxAttempts = 10;
            float bossSizeHalf = col.bounds.size.x / 2;
            for (int i = 0; i < maxAttempts; i++)
            {
                float randomX = Random.Range(_aeraCollider.bounds.min.x + bossSizeHalf, _aeraCollider.bounds.max.x - bossSizeHalf);
                Vector2 raycastPos = new Vector2(randomX, _aeraCollider.bounds.max.y);
                RaycastHit2D ray = Physics2D.Raycast(raycastPos, Vector2.down, Mathf.Infinity, _whatIsGround);
                if (ray.collider != null)
                    return ray.point + new Vector2(0, _offsetY);
            }
            return transform.position;
        }
    }
}
