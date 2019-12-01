using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            eventController.QueueListener(typeof(GameStartEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnGameStart));
            isEventReady = true;
        }
    }

    private void OnDestroy()
    {
        eventController.RemoveListener(typeof(GameStartEvt), OnGameStart);
        eventController.RemoveListener(typeof(RocketCollidedEvt), OnRocketCollide);
    }

    void OnRocketCollide(GameEvent e)
    {
        var image = transform.Find("Life" + Lives);
        image.GetComponent<Image>().enabled = false;
        Lives--;
    }

    void OnGameStart(GameEvent e)
    {
        Lives = 5;
        for (var i = 1; i <= 5; i++)
        {
            var image = transform.Find("Life" + i);
            image.GetComponent<Image>().enabled = true;
        }
    }
}
