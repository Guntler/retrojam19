using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehavior : BoardItemBehavior
{
    public SpriteRenderer sprComp;
    public TruckBehavior truckObj;
    public BoardItemBehavior attachedTo;
    
    private bool evtReady = false;

    protected override void Start()
    {
        base.Start();
        sprComp = GetComponent<SpriteRenderer>();
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
        //Destroy(gameObject);
    }

    public void DeliverCar()
    {
        sprComp.enabled = false;

        eventCtrl.BroadcastEvent(typeof(DeliverCarEvt), new DeliverCarEvt(this));
        if (attachedTo != null && !(attachedTo is TruckBehavior))
        {   
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("deliverNextCar" + gameObject.GetInstanceID(), 0.5f, () => { ((CarBehavior)attachedTo).DeliverCar(); }));
        }
        else
        {
            eventCtrl.BroadcastEvent(typeof(DeliveredDebrisEvt), new DeliveredDebrisEvt());
            eventCtrl.BroadcastEvent(typeof(StartTickEvt), new StartTickEvt());
        }

        gameCtrl.Board.RemoveItem(this);
        
        Destroy(gameObject);
    }

    public void DestroyCar()
    {
        sprComp.enabled = false;

        if (attachedTo != null && !(attachedTo is TruckBehavior))
        {
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("destroyNextCar" + gameObject.GetInstanceID(), 0.25f, () => { ((CarBehavior)attachedTo).DestroyCar(); }));
        }
        else
        {
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("destroyNextCar" + gameObject.GetInstanceID(), 0.5f, () => { Destroy(attachedTo.gameObject); }));
        }

        gameCtrl.Board.RemoveItem(this);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(TruckCollidedEvt), TruckDestroyedCallback);
    }
}

