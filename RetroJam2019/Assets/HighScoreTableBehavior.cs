using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct data_HighScoreEntry
{
    public string Name;
    public int Score;
    public int Index;
}

public class HighScoreTableBehavior : MonoBehaviour
{
    public Vector3 StartingPosition;
    public float YOffset = -32;
    public GameObject PressAnyKeyObject;
    bool canSkipGameOver = false;

    public GameObject EntryPrefab;
    public float RateToDisplay = 2.5f;

    int currentEntry = 0;

    List<data_HighScoreEntry> entries = new List<data_HighScoreEntry>();

    GlobalEventController eventCtrl;

    void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
        
    }

    public void BeginDisplay()
    {
        print("DISPLAYING HIGH SCORE TABLE");
        entries = GameManager.GetInstance().ReadHighScoreList();
        if (entries.Count > 0)
        {
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("displayScoreTimer", RateToDisplay/3, () => { DisplayEntry(); }));
        }
        else
        {
            PressAnyKeyObject.SetActive(true);
        }
    }

    void DisplayEntry()
    {
        GameObject entry = Instantiate(EntryPrefab, transform);
        entry.GetComponent<RectTransform>().anchoredPosition = StartingPosition + new Vector3(0, (currentEntry * YOffset), 0);
        ScoreEntryBehavior obj = entry.GetComponent<ScoreEntryBehavior>();
        data_HighScoreEntry dataEntry = entries[currentEntry];
        obj.PlayerName = dataEntry.Name;
        obj.ScoreAmt = dataEntry.Score;
        obj.EntryIndex = currentEntry+1;

        currentEntry++;

        if(currentEntry < entries.Count)
        {
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("displayScoreTimer", RateToDisplay, () => { DisplayEntry(); }));
        }
        else
        {
            PressAnyKeyObject.SetActive(true);
            canSkipGameOver = true;
        }
    }

    private void Update()
    {
        if (Input.anyKey && canSkipGameOver)
        {
            canSkipGameOver = false;

            eventCtrl.BroadcastEvent(typeof(GameStartEvt), new GameStartEvt());
            Destroy(gameObject);
        }
    }
}
