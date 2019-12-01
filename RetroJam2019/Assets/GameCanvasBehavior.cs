using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvasBehavior : MonoBehaviour
{
    public AudioSettings SelectSfx;
    public GameObject RocketDestroyedPrefab;
    public GameObject RocketAwayPrefab;
    public GameObject GameOverPrefab;
    public GameObject HighScoreTablePrefab;

    GlobalEventController eventCtrl;

    void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.QueueListener(typeof(RocketSafeEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), RocketSafeCallback));
        eventCtrl.QueueListener(typeof(RocketCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), RocketDestroyedCallback));
        eventCtrl.QueueListener(typeof(ShowHighScoreTable), new GlobalEventController.Listener(gameObject.GetInstanceID(), HighScoreTableCallback));
        eventCtrl.QueueListener(typeof(GameEndEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), GameEndCallback));
    }

    void RocketSafeCallback(GameEvent e)
    {
        Instantiate(RocketAwayPrefab, transform);
    }

    void RocketDestroyedCallback(GameEvent e)
    {
        Instantiate(RocketDestroyedPrefab, transform);
    }

    void GameEndCallback(GameEvent e)
    {
        Instantiate(GameOverPrefab, transform);
    }

    void HighScoreTableCallback(GameEvent e)
    {
        Instantiate(HighScoreTablePrefab, transform);
    }

    private void OnDestroy()
    {
        eventCtrl.BroadcastEvent(typeof(PlayBackgroundClip), new PlayBackgroundClip(SelectSfx));

        eventCtrl.RemoveListener(typeof(RocketSafeEvt), RocketSafeCallback);
        eventCtrl.RemoveListener(typeof(RocketCollidedEvt), RocketDestroyedCallback);
    }
}
