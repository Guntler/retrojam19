using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUIBehavior : MonoBehaviour
{
    private int Lives = 5;
    private bool isEventReady = false;
    private GlobalEventController eventController;
    // Start is called before the first frame update
    void Start()
    {
        eventController = GlobalEventController.GetInstance();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isEventReady)
        {
            eventController.QueueListener(typeof(RocketCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnRocketCollide));
            isEventReady = true;
        }
    }

    void OnRocketCollide(GameEvent e)
    {
        var image = transform.Find("Life" + Lives);
        Destroy(image.gameObject);
        Lives--;
    }
}
