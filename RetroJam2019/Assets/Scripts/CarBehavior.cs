using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehavior : BoardItemBehavior
{
    public Animator animComp;
    public SpriteRenderer sprComp;
    public TruckBehavior truckObj;
    public BoardItemBehavior attachedTo;
    public BoardItemBehavior attachesTo;
    
    private bool evtReady = false;
    public bool IsCornerBox = false;

    protected override void Start()
    {
        base.Start();
        sprComp = GetComponent<SpriteRenderer>();
        animComp = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!evtReady)
        {
            eventCtrl.QueueListener(typeof(TruckCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), TruckDestroyedCallback));
            evtReady = true;
        }
    }

    private void Update()
    {
        if (attachedTo.BoardPosition.x > BoardPosition.x)
        {
            animComp.SetBool("IsLeft", false);
        }
        else if (attachedTo.BoardPosition.x < BoardPosition.x)
        {
            animComp.SetBool("IsLeft", true);
        }

        if (attachedTo.BoardPosition.y > BoardPosition.y)
        {
            animComp.SetBool("IsUp", false);
            animComp.SetBool("IsFacingSide", false);
        }
        else if (attachedTo.BoardPosition.y < BoardPosition.y)
        {
            animComp.SetBool("IsUp", true);
            animComp.SetBool("IsFacingSide", false);
        }
        else if (attachedTo.BoardPosition.y == BoardPosition.y)
        {
            animComp.SetBool("IsFacingSide", true);
        }



        if (attachedTo.BoardPosition.x == BoardPosition.x && attachedTo.BoardPosition.y != BoardPosition.y
                && attachesTo != null && attachesTo.BoardPosition.y == BoardPosition.y)
        {
            animComp.SetBool("IsCornerBox", true);
            IsCornerBox = true;
        }
        else
        {
            animComp.SetBool("IsCornerBox", false);
            IsCornerBox = false;
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
        eventCtrl.BroadcastEvent(typeof(StopTimerEvent), new StopTimerEvent("deliverNextCar" + gameObject.GetInstanceID()));
        eventCtrl.BroadcastEvent(typeof(StopTimerEvent), new StopTimerEvent("destroyNextCar" + gameObject.GetInstanceID()));
        eventCtrl.RemoveListener(typeof(TruckCollidedEvt), TruckDestroyedCallback);
    }
}

