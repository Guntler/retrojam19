﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIBehavior : MonoBehaviour
{
    public GameObject ScoreMultiplierPrefab;
    public float UpdateInterval = 0.25f;
    public int ScoreUpdateRate = 5;
    public AudioSettings ScoreUpSfx;

    private int currentScore = 0;
    private int targetScore = 0;
    private Coroutine currentUpdateRoutine;
    private Text textComp;
    private GlobalEventController eventCtrl;


    void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.QueueListener(typeof(UpdateScoreEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), UpdateScoreCallback));
        eventCtrl.QueueListener(typeof(CreateScoreMultiplierTextEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), CreateScoreMultiplierTextCallback));
        eventCtrl.QueueListener(typeof(GameStartEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnGameStart));
        eventCtrl.QueueListener(typeof(GameEndEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnGameEnd));
        textComp = GetComponent<Text>();
    }

    void CreateScoreMultiplierTextCallback(GameEvent e)
    {
        CreateScoreMultiplierTextEvt ev = (CreateScoreMultiplierTextEvt)e;
        GameObject scoreMult = Instantiate(ScoreMultiplierPrefab, transform.parent);
        scoreMult.GetComponent<Text>().text = "x" + ev.Multiplier.ToString();
    }

    void UpdateScoreCallback(GameEvent e)
    {
        UpdateScoreEvt ev = (UpdateScoreEvt)e;
        targetScore = ev.ScoreAmt;

        if(currentUpdateRoutine != null)
        {
            StopCoroutine(currentUpdateRoutine);
        }

        currentUpdateRoutine = StartCoroutine(UpdateScore());
    }

    void OnGameStart(GameEvent e)
    {
        targetScore = 0;
        currentScore = 0;
        textComp.text = currentScore.ToString();

        if (currentUpdateRoutine != null)
        {
            StopCoroutine(currentUpdateRoutine);
        }
    }

    void OnGameEnd(GameEvent e)
    {
        if (currentUpdateRoutine != null)
        {
            StopCoroutine(currentUpdateRoutine);
        }
        currentScore = targetScore;
        textComp.text = currentScore.ToString();
    }
    IEnumerator UpdateScore()
    {
        while(currentScore != targetScore)
        {
            eventCtrl.BroadcastEvent(typeof(PlayOneshotClipEvent), new PlayOneshotClipEvent(ScoreUpSfx));
            
            currentScore = Mathf.Clamp(currentScore + ScoreUpdateRate, 0, targetScore);
            textComp.text = currentScore.ToString();
            yield return new WaitForSeconds(UpdateInterval);
        }

        yield return null;
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(UpdateScoreEvt), UpdateScoreCallback);
        eventCtrl.RemoveListener(typeof(CreateScoreMultiplierTextEvt), CreateScoreMultiplierTextCallback);
        eventCtrl.RemoveListener(typeof(GameStartEvt), OnGameStart);
        eventCtrl.RemoveListener(typeof(GameEndEvt), OnGameEnd);
    }
}
