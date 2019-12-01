using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncineratorBehavior : BoardItemBehavior
{
    public GameObject IncinFullPrefab;
    public Vector3 IncinFullOffset = new Vector3(0, 1, 0);

    void FixedUpdate()
    {
        if (!isEventReady)
        {
            isEventReady = true;

            gameCtrl.Board.AddItem(this, BoardPosition);
            eventCtrl.QueueListener(typeof(DeliverCarEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), DeliverCarCallback));
        }

    }

    void DeliverCarCallback(GameEvent e)
    {
        print("Delivering car");
        Instantiate(IncinFullPrefab, transform.position - IncinFullOffset, new Quaternion(0,0,0,0));
    }
}
