using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsPause { get; private set; }
    private Coroutine _endGameCoroutine;

    void Awake()
    {
        if (ServiceLocator.Get<GameManager>() != null)
        {
            Destroy(gameObject);
            return;
        }

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


    public void EndGame()
    {

        if(_endGameCoroutine != null) StopCoroutine(_endGameCoroutine);
        _endGameCoroutine = StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndGameScene");
    }

}
