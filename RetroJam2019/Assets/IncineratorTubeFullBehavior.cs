using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncineratorTubeFullBehavior : MonoBehaviour
{
    public float MovementRate = 0.25f;
    public int UnitsToMove = 4;

    private float unitsMoved = 0;
    GlobalEventController eventCtrl;

    private void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("incinFullTimer_" + gameObject.GetInstanceID(), 0.15f, MoveTubeFull));
    }

    void MoveTubeFull()
    {
        unitsMoved += MovementRate;
        transform.position += new Vector3(0, -MovementRate, 0);
        print("Units " + unitsMoved + " - " + UnitsToMove);
        if (unitsMoved > UnitsToMove)
        {
            Destroy(gameObject);
        }
        else
        {
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("incinFullTimer_" + gameObject.GetInstanceID(), 0.15f, MoveTubeFull));
        }
    }
}
