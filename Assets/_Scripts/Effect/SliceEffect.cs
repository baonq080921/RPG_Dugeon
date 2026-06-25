using System;
using UnityEngine;

namespace Effect
{
    public class SliceEffect : MonoBehaviour
    {
        public Action OnComplete;
        [SerializeField] private Animator _animator;
        public bool isDone { get; private set; }

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetAnimation()
        {
            isDone = true;
            _animator.SetBool("isDone", isDone);
            OnComplete?.Invoke();
        }

        /// <summary>Resets state and restarts the animation from frame 0 for pool reuse.</summary>
        public void ResetAnimation()
        {
            isDone = false;
            _animator.SetBool("isDone", isDone);
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}
