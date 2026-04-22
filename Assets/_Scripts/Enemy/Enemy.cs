using UnityEngine;
namespace Enemy
{
    public class Enemy : MonoBehaviour, IHit
    {
        [SerializeField] private float _health = 100f;
        private SpriteRenderer _sr;

        private float _currentTime;
        private float _lastHitTime;
        private float _hitFlashDuration = 0.2f;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _currentTime = Time.time;
            if(_currentTime >_lastHitTime + _hitFlashDuration)
            {
                if(_sr.color != Color.white)
                    _sr.color = Color.white; // Reset color after flash duration.
            }
        }

        public void TakeDamage(float damage)
        {
            if(_health <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            _health -= damage;
            _sr.color = Color.red; // Flash red on hit for visual feedback.
            _lastHitTime = Time.time;
            // Debug.Log($"Enemy took {damage} damage.");
        }
    }
}