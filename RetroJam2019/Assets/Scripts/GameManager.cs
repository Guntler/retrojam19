using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float TILE_X = 1;
    public static float TILE_Y = 1;
    public static int SCORE_PER_DEBRIS = 10;
    public static int SCORE_PER_ROCKET = 50;

    private static GameManager s_Instance;
    void Awake()
    {
        print("Game controller is awake");
        s_Instance = this;
    }

    public static GameManager GetInstance()
    {
        if (s_Instance)
            return s_Instance;
        else
            return null;
    }

    AudioSource bgmSrc;
    GlobalEventController eventCtrl;
    
    public GameObject Player;
    float evtTimeAccumulator = 0;
    int evtTicker = 0;
    public float minimumTickTime = 0.1f;
    private bool isTicking = true;

    public int Score
    {
        get; set;
    }

    public int Lives
    {
        get; set;
    }

    public GameBoard Board
    {
        get; set;
    }

    private bool isEventReady = false;

    void Start()
    {
        Score = 0;
        Lives = 5;
        eventCtrl = GlobalEventController.GetInstance();
        bgmSrc = GetComponent<AudioSource>();
        Board = new GameBoard();
    }

    void StartTickCallback(GameEvent e)
    {
        isTicking = true;
    }

    void StopTickCallback(GameEvent e)
    {
        isTicking = false;
    }

    void StopBgmCallback(GameEvent e)
    {
        bgmSrc.Stop();
    }

    private void FixedUpdate()
    {
        if (!isEventReady)
        {
            eventCtrl.QueueListener(typeof(StartTickEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), StartTickCallback));
            eventCtrl.QueueListener(typeof(StopTickEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), StopTickCallback));
            eventCtrl.QueueListener(typeof(RocketCollidedEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnRocketCollided));
            eventCtrl.QueueListener(typeof(AddScoreEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnAddScore));
            eventCtrl.QueueListener(typeof(StopBackgroundMusicEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), StopBgmCallback));
            eventCtrl.QueueListener(typeof(CheckHighScoreEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), CheckScoreCallback));
            eventCtrl.QueueListener(typeof(RegisterNewEntryEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), RegisterNameCallback));

            isEventReady = true;
        }

        if (isTicking)
        {
            evtTimeAccumulator += Time.fixedDeltaTime;

            if (evtTimeAccumulator >= minimumTickTime)
            {
                evtTicker++;
                evtTimeAccumulator -= minimumTickTime;

                if (evtTicker > 10)
                {
                    evtTicker = 0;
                }

                Tick();
            }
        }
    }

    private void Tick()
    {
        eventCtrl.BroadcastEvent(typeof(Update100Evt), new Update100Evt());

        if (evtTicker % 2 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update200Evt), new Update200Evt());
        }

        if (evtTicker % 5 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update500Evt), new Update500Evt());
        }

        if (evtTicker % 10 == 0)
        {
            eventCtrl.BroadcastEvent(typeof(Update1000Evt), new Update1000Evt());
        }
    }

    void OnRocketCollided(GameEvent e)
    {
        eventCtrl.BroadcastEvent(typeof(StopBackgroundMusicEvt), new StopBackgroundMusicEvt());

        Lives--;

        if (Lives <= 0)
        {
            eventCtrl.BroadcastEvent(typeof(GameEndEvt), new GameEndEvt());
            Player.GetComponent<TruckBehavior>().DestroyTruck();
        }
    }

    public void RemoveLives()
    {
        Lives = 0;
        eventCtrl.BroadcastEvent(typeof(GameEndEvt), new GameEndEvt());
    }

    void OnAddScore(GameEvent e)
    {
        AddScoreEvt ev = (AddScoreEvt)e;
        Score += ev.ScoreAmt;
        eventCtrl.BroadcastEvent(typeof(UpdateScoreEvt), new UpdateScoreEvt(Score));
    }


    /// <summary>
    /// UTILITY FUNCTION
    /// </summary>
    /// <returns></returns>
    public List<data_HighScoreEntry> ReadHighScoreList()
    {
        string path = Application.dataPath + "/Resources/Data/HighScores.txt";
        FileStream f = File.OpenRead(path);

        string lines = "";
        if (f != null && f.CanRead)
        {
            lines = File.ReadAllText(path);
        }

        List<data_HighScoreEntry> entryList = new List<data_HighScoreEntry>();

        string currentLine = "";

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == '\n')
            {
                continue;
            }

            currentLine += lines[i];
        }

        if(currentLine.Contains(","))
        {
            string[] fields = currentLine.Split(',');

            if (fields.Length > 0)
            {
                data_HighScoreEntry entry = new data_HighScoreEntry();
                entry.Name = fields[0];
                entry.Score = Int32.Parse(fields[1]);

                entryList.Add(entry);
            }
        }
        

        return entryList;
    }


    /// <summary>
    /// UTILITY FUNCTION
    /// </summary>
    /// <param name="entryList"></param>
    void WriteHighScoreList(List<data_HighScoreEntry> entryList)
    {
        string path = Application.dataPath + "/Resources/Data/HighScores.txt";
        FileStream f = File.OpenRead(path);

        List<string> lines = new List<string>();

        foreach(data_HighScoreEntry en in entryList)
        {
            string line = en.Name + "," + en.Score + "\n";
            lines.Add(line);
        }

        using (StreamWriter outputFile = new StreamWriter(path))
        {
            foreach (string line in lines)
                outputFile.WriteLine(line);
        }

    }

    /// <summary>
    /// CHECK HIGH SCORE
    /// </summary>
    /// <param name="e"></param>
    void CheckScoreCallback(GameEvent e)
    {
        List<data_HighScoreEntry> entryList = ReadHighScoreList();

        if(entryList.Count > 0 && entryList[0].Score > Score)
        {
            eventCtrl.BroadcastEvent(typeof(ShowNameEntryEvt), new ShowNameEntryEvt(Score));
        }
        else
        {
            GlobalEventController.GetInstance().BroadcastEvent(typeof(ShowHighScoreTable), new ShowHighScoreTable());
        }
    }

    void RegisterNameCallback(GameEvent e)
    {
        RegisterNewEntryEvt ev = (RegisterNewEntryEvt)e;

        List<data_HighScoreEntry> entryList = ReadHighScoreList();

        entryList.Insert(0, ev.entry);
        entryList.RemoveAt(entryList.Count - 1);
        WriteHighScoreList(entryList);

        eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("displayTableTimer", 1, () => { GlobalEventController.GetInstance().BroadcastEvent(typeof(ShowHighScoreTable), new ShowHighScoreTable()); }));
    }

}