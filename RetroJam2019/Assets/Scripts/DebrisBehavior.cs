using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBehavior : BoardItemBehavior
{
    protected override void Start()
    {
        base.Start();

        eventCtrl.QueueListener(typeof(PickedUpDebrisEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), PickedUpDebrisCallback));
    }

    void PickedUpDebrisCallback(GameEvent e)
    {
        PickedUpDebrisEvt ev = (PickedUpDebrisEvt)e;

        if(ev.PickedItem == this)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!isEventReady)
        {
            isEventReady = true;

            gameCtrl.Board.AddItem(this, BoardPosition);

        }
    }

    private void OnDestroy()
    {
        gameCtrl.Board.RemoveItem(this);
        eventCtrl.RemoveListener(typeof(PickedUpDebrisEvt), PickedUpDebrisCallback);
    }
}
