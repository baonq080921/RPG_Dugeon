using System.Collections;
using UnityEngine;

namespace Effect
{
    /// <summary>
    /// Attach to any one-shot animation-based effect prefab. Call <see cref="Spawn"/> instead of
    /// Instantiate — it pulls from a shared static pool and automatically returns when
    /// the animation finishes playing.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PoolableVfx : MonoBehaviour
    {
        private static readonly System.Collections.Generic.Stack<PoolableVfx> s_pool = new();
        private static Transform s_container;

        private Animator _anim;

        private void Awake() => _anim = GetComponent<Animator>();

        /// <summary>
        /// Plays this effect at <paramref name="position"/>, reusing a pooled instance when available.
        /// </summary>
        public static void Spawn(PoolableVfx prefab, Vector3 position)
        {
            PoolableVfx vfx;
            if (s_pool.Count > 0)
            {
                vfx = s_pool.Pop();
                vfx.gameObject.SetActive(true);
            }
            else
            {
                if (s_container == null)
                {
                    s_container = new GameObject("[PoolableVfxPool]").transform;
                    DontDestroyOnLoad(s_container.gameObject);
                }
                vfx = Instantiate(prefab, s_container);
            }

            vfx.transform.position = position;
            vfx._anim.Play(0, -1, 0f);
            vfx.StartCoroutine(vfx.ReturnWhenDone());
        }

        private IEnumerator ReturnWhenDone()
        {
            // Wait one frame for the animator to enter the state
            yield return null;
            yield return new WaitUntil(() =>
                _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            gameObject.SetActive(false);
            s_pool.Push(this);
        }
    }
}
