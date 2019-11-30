using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_Instance;

    void Awake()
    {
        print("Game controller is awake");
        s_Instance = this;
    }
}