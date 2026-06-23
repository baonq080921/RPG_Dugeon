using System;
using System.Collections;
using Base;
using TMPro;
using UnityEngine;

namespace NPC
{
    /// <summary>
    /// An NPC that can be rescued once by the player.
    /// Attach this alongside (or instead of) <see cref="Npc"/> on any NPC
    /// that should count toward a <see cref="Quest.RescueNpcQuestData"/>.
    /// Fires <see cref="NpcRescuedEvent"/> on the first interaction and then
    /// disables itself so it cannot be rescued again.
    /// </summary>
    public class RescuableNpc : Npc
    {
        public bool IsRescued { get; private set; }
        private Coroutine _talkCoroutine;
        [SerializeField] private UIFloating_Panel _uIFloating_Panel;
        [SerializeField] private TextMeshProUGUI _talkText;
        public override void OnInteract()
        {
            base.OnInteract();
            if (IsRescued) return;
            if(_talkCoroutine != null) StopCoroutine(_talkCoroutine);
            _talkCoroutine = StartCoroutine(PlayTalkDialouge(_dialougeStr));
            IsRescued = true;
            RaiseOnPersistent();
            EventBus<NpcRescuedEvent>.Raise(new NpcRescuedEvent());
        }

        IEnumerator PlayTalkDialouge(string dialougeText)
        {
            _uIFloating_Panel.ShowPanel(true);
            _talkText.text = dialougeText;
            _talkText.maxVisibleCharacters = 0;
            while(_talkText.maxVisibleCharacters < dialougeText.Length)
            {
                _talkText.maxVisibleCharacters++;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
            _uIFloating_Panel.ShowPanel(false);
            gameObject.SetActive(false);
        }
    }
}
