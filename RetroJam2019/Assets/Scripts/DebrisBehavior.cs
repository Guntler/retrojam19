using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBehavior : BoardItemBehavior
{
    Animator animComp;
    public List<float> frameTimes = new List<float>();
    public bool ForceAnim = false;

    protected override void Start()
    {
        base.Start();
        animComp = GetComponent<Animator>();

        if (!ForceAnim)
        {
            var rand = Random.Range(0, frameTimes.Count);
            print(rand);
            animComp.Play("DebrisAnim", 0, frameTimes[rand]);
        }

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
