using entity;
using UnityEngine;

namespace player
{
    /// <inheritdoc/>
    public class PlayerVfx : EntityVfx
    {
        private Player _player;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
            knockBackMat = _player.Data.Material;
        }

       
    }
}
