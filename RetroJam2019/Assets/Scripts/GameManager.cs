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

    GlobalEventController eventCtrl;
    float evtTimeAccumulator = 0;
    int evtTicker = 0;
    readonly int minimumTickTime = 100;

    public GameBoard Board
    {
        get; set;
    }

    void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        Board = new GameBoard();
    }

    private void FixedUpdate()
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

    private void Tick()
    {
        eventCtrl.BroadcastEvent(typeof(Update100Evt), new Update100Evt());

        if (evtTicker % 5 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update500Evt), new Update500Evt());
        }

        if (evtTicker % 10 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update1000Evt), new Update1000Evt());
        }
    }
}