using UnityEngine;

namespace UI
{
    public class UIFloating_Panel : MonoBehaviour
    {


        void Start()
        {
            ShowPanel(false);
        }

        /// <summary>
        /// Use for all the npc that have panel to conversation with the player
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowPanel(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

    }
}
