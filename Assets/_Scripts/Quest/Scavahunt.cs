
using System;
using Base;
using Quest;
using UnityEngine;

[CreateAssetMenu(fileName = "ScavangeHunt", menuName = "RPG/Quest/SavangeHunt  Quest")]
public class ScavaHunt : QuestData
{
    public override bool IsCompleted => throw new NotImplementedException();

    public override string ProgressText => $"{huntDown} / {totalHuntDown}";
    [field:SerializeField]public int huntDown{get; private set;} = 0;
    [field:SerializeField] public int totalHuntDown;
    [field:SerializeField] public int skillPoint{get; private set;}

    private EventBinding<NpcRescuedEvent> _eventBinding;

    public override void Reset() => huntDown = 0;
    
    public override void StartTracking()
    {
        _eventBinding = new EventBinding<NpcRescuedEvent>(OnScavaHunt);
        EventBus<NpcRescuedEvent>.Register(_eventBinding);

    }

    public override void StopTracking()
    {
        if (_eventBinding != null)
                EventBus<NpcRescuedEvent>.Deregister(_eventBinding);
    }

     private void OnScavaHunt(NpcRescuedEvent e)
        {
            if (IsCompleted) return;
            huntDown++;
            NotifyProgress();
            if (IsCompleted)
                NotifyCompleted();
        } 
}