using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsPause { get; private set; }

    void Awake()
    {
        Application.targetFrameRate = 60;
        ServiceLocator.Register<GameManager>(this);
    }

    /// <summary>
    /// Sets the game pause state. Called by UI systems that freeze time.
    /// </summary>
    /// <param name="pause">True to pause, false to resume.</param>
    public void SetPause(bool pause)
    {
        IsPause = pause;
        EventBus<GamePauseChangedEvent>.Raise(new GamePauseChangedEvent(pause));
    }

}
