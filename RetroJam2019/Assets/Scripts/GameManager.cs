using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float TILE_X = 1;
    public static float TILE_Y = 1;

    private static GameManager s_Instance;
    void Awake()
    {
        print("Game controller is awake");
        s_Instance = this;
    }

    public static GameManager GetInstance()
    {
        if (s_Instance)
            return s_Instance;
        else
            return null;
    }

    GlobalEventController eventCtrl;
    float evtTimeAccumulator = 0;
    int evtTicker = 0;
    public float minimumTickTime = 0.1f;
    private bool isTicking = false;

    public int Score
    {
        get; set;
    }

    public int Lives
    {
        get; set;
    }

    public GameBoard Board
    {
        get; set;
    }

    private bool isEventReady = false;

    void Start()
    {
        Score = 0;
        Lives = 5;
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.QueueListener(typeof(StartTickEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), StartTickCallback));
        eventCtrl.QueueListener(typeof(StopTickEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), StopTickCallback));
        Board = new GameBoard();
    }

    void StartTickCallback(GameEvent e)
    {
        isTicking = true;
    }

    void StopTickCallback(GameEvent e)
    {
        isTicking = false;
    }

    private void FixedUpdate()
    {
        if (!isEventReady)
        {
            eventCtrl.QueueListener(typeof(RocketCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnRocketCollided));
            eventCtrl.QueueListener(typeof(AddScoreEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnAddScore));
            isEventReady = true;
        }

        if (isTicking)
        {
            evtTimeAccumulator += Time.fixedDeltaTime;

            if (evtTimeAccumulator >= minimumTickTime)
            {
                evtTicker++;
                evtTimeAccumulator -= minimumTickTime;

                if (evtTicker > 10)
                {
                    evtTicker = 0;
                }

                Tick();
            }
        }
    }

    private void Tick()
    {
        eventCtrl.BroadcastEvent(typeof(Update100Evt), new Update100Evt());

        if (evtTicker % 2 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update200Evt), new Update200Evt());
        }

        if (evtTicker % 5 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update500Evt), new Update500Evt());
        }

        if (evtTicker % 10 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update1000Evt), new Update1000Evt());
        }
    }

    void OnRocketCollided(GameEvent e)
    {
        Lives--;

        if (Lives <= 0)
        {
            eventCtrl.BroadcastEvent(typeof(GameEndEvt), new GameEndEvt());
        }
    }

    void OnAddScore(GameEvent e)
    {
        Score += 1;
    }
}