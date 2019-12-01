using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBehavior : MonoBehaviour
{
    public GameObject PressAnyKeyObject;
    bool canSkipGameOver = false;

    public void GameOverCallback()
    {
        GlobalEventController.GetInstance().BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("gameOverPress_" + gameObject.GetInstanceID(), 2.5f, () => {
            PressAnyKeyObject.SetActive(true);
            canSkipGameOver = true;
        }));
        GlobalEventController.GetInstance().BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("gameOver_" + gameObject.GetInstanceID(), 30, () => { Destroy(transform.parent.gameObject); }));
    }

    private void Update()
    {
        if(Input.anyKey && canSkipGameOver)
        {
            canSkipGameOver = false;

            GlobalEventController.GetInstance().BroadcastEvent(typeof(StopTimerEvent), new StopTimerEvent("gameOver_" + gameObject.GetInstanceID()));
            GlobalEventController.GetInstance().BroadcastEvent(typeof(CheckHighScore), new CheckHighScore());
            Destroy(transform.parent.gameObject);
        }
    }
}
