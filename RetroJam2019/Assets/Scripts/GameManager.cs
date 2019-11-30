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

    void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
    }


}