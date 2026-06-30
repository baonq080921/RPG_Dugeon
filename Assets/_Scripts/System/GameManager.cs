using System.Collections;
using System.Collections.Generic;
using Base;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameState
{
    public bool IsPause { get; private set; }
    private Coroutine _endGameCoroutine;
    private EventBinding<EndGameEvent> _eventEndGameBinding;

    void Awake()
    {
        if (ServiceLocator.Get<IGameState>() != null)
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;
        ServiceLocator.Register<IGameState>(this);
    }
    void OnEnable()
    {
        _eventEndGameBinding = new EventBinding<EndGameEvent>(EndGame);
        EventBus<EndGameEvent>.Register(_eventEndGameBinding);
    }

    void OnDisable()
    {
        EventBus<EndGameEvent>.Deregister(_eventEndGameBinding);    
    }

    /// <summary>
    /// Sets the game pause state. Called by UI systems that freeze time.
    /// </summary>
    /// <param name="pause">True to pause, false to resume.</param>
    public void SetPause(bool pause)
    {
        IsPause = pause;
        Time.timeScale = pause ? 0 : 1;
        EventBus<GamePauseChangedEvent>.Raise(new GamePauseChangedEvent(pause));
    }


    public void EndGame()
    {

        if(_endGameCoroutine != null) StopCoroutine(_endGameCoroutine);
        _endGameCoroutine = StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        var transition = scene.SceneTransitionManager.Instance;
        if (transition != null)
            transition.FadeToScene("EndGame");
        else
            SceneManager.LoadScene("EndGame");
    }

}
