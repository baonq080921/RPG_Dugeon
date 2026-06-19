using Base;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIAlert : MonoBehaviour 
{
    private EventBinding<AlertNotiEvent> _eventBindingAlert;
    [SerializeField] private TextMeshProUGUI _alertNotiTmp;
    private Sequence _alertSequence;

    private void OnEnable() 
    {
        _eventBindingAlert = new EventBinding<AlertNotiEvent>(AlertNotificationUI);
        EventBus<AlertNotiEvent>.Register(_eventBindingAlert);

    }

    private void OnDisable()
    {
        EventBus<AlertNotiEvent>.Deregister(_eventBindingAlert);
    }


    void AlertNotificationUI(AlertNotiEvent alertNotiEvent)
    {
        _alertNotiTmp.text = alertNotiEvent.alertMessage;
        _alertNotiTmp.color = alertNotiEvent.color.a > 0 ? alertNotiEvent.color : Color.white;
        _alertNotiTmp.rectTransform.anchoredPosition = alertNotiEvent.position;
        _alertNotiTmp.DOKill();
        _alertSequence?.Kill();
        _alertSequence = DOTween.Sequence();
        _alertSequence.Append(_alertNotiTmp.DOFade(1, 0.25f).SetUpdate(true).SetEase(Ease.InBack));
        _alertSequence.AppendInterval(0.5f).SetUpdate(true);
        _alertSequence.Append(_alertNotiTmp.DOFade(0, 0.25f).SetUpdate(true).SetEase(Ease.OutBack));
    }
}