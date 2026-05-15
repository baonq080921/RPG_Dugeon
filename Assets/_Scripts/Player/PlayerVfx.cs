using UnityEngine;

namespace player
{
    /// <inheritdoc/>
    public class PlayerVfx : EntityVfx
    {
        private Player _player;
       

        protected override Material KnockBackMat => _player.Data.Material;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
        }

       
    }
}
