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
            eventCtrl.QueueListener(typeof(TruckCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), TruckDestroyedCallback));
            evtReady = true;
        }
    }

    public void MoveCar()
    {
        LastBoardPosition = BoardPosition;
        var newPos = gameCtrl.Board.MoveItemToPosition(this, attachedTo.LastBoardPosition);
        transform.position = gameCtrl.Board.GetWorldPosition(new Vector2(BoardPosition.x, -BoardPosition.y));
    }

    public void TruckDestroyedCallback(GameEvent e)
    {
        Destroy(gameObject);
    }

    public void DeliverCar()
    {
        eventCtrl.BroadcastEvent(typeof(AddScoreEvt), new AddScoreEvt());
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(TruckCollidedEvt), TruckDestroyedCallback);
    }
}

