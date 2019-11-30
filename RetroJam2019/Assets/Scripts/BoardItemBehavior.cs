using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardItemBehavior : MonoBehaviour
{
    public Vector2 BoardPosition = new Vector2(0, 0);
    protected GameManager gameCtrl;
    protected GlobalEventController eventCtrl;
    protected bool isEventReady = false;
    public Vector2 LastBoardPosition;

    protected virtual void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        gameCtrl = GameManager.GetInstance();
    }
}
