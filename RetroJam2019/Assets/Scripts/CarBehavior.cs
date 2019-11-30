using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehavior : BoardItemBehavior
{
    public TruckBehavior truckObj;
    public BoardItemBehavior attachedTo;
    private bool evtReady = false;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        if (!evtReady)
        {
           // eventCtrl.QueueListener(typeof(Update200Evt), new GlobalEventController.Listener(gameObject.GetInstanceID(), Update200EvtCallback));
            evtReady = true;
        }
    }

    public void MoveCar()
    {
        var newPos = gameCtrl.Board.MoveItemToPosition(this, attachedTo.LastBoardPosition);
        LastBoardPosition = BoardPosition;
        BoardPosition = newPos;
        transform.position = gameCtrl.Board.GetWorldPosition(new Vector2(BoardPosition.x, -BoardPosition.y));
    }
}

