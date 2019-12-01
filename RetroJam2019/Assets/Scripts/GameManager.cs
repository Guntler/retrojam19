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
    public AudioClip TitleSong;
    public AudioClip GameSong;
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

    public GameObject PlayerPrefab;

    void Start()
    {
        Score = 0;
        Lives = 5;
        eventCtrl = GlobalEventController.GetInstance();
        bgmSrc = GetComponent<AudioSource>();
        Board = new GameBoard();
        isTicking = false;
        bgmSrc.clip = TitleSong;
        bgmSrc.Play();
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
            eventCtrl.QueueListener(typeof(GameStartEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), GameResetCallback));
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
        Lives--;

        if (Lives <= 0)
        {
            eventCtrl.BroadcastEvent(typeof(StopBackgroundMusicEvt), new StopBackgroundMusicEvt());
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
        Debug.Log("READING FROM HIGH SCORES FILE");

        string path = Application.dataPath + "/Resources/Data/HighScores.txt";
        FileStream f = File.OpenRead(path);

        string lines = "";
        if (f != null && f.CanRead)
        {
            lines = File.ReadAllText(path);
        }

        List<data_HighScoreEntry> entryList = new List<data_HighScoreEntry>();

        string currentLine = "";

        //string[] linesDivided = lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        //print("FOUND : " + linesDivided.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == '\n')
            {
                if (currentLine.Contains(","))
                {
                    string[] fields = currentLine.Split(',');

                    if (fields.Length > 0)
                    {
                        data_HighScoreEntry entry = new data_HighScoreEntry();
                        print(fields + " - " + fields[0] + " - " + fields[1]);
                        entry.Name = fields[0];
                        entry.Score = Int32.Parse(fields[1]);

                        entryList.Add(entry);

                        currentLine = "";
                    }
                }

                continue;
            }
            else
            {
                currentLine += lines[i];
            }

            
        }

        

        f.Close();

        return entryList;
    }


    /// <summary>
    /// UTILITY FUNCTION
    /// </summary>
    /// <param name="entryList"></param>
    void WriteHighScoreList(List<data_HighScoreEntry> entryList)
    {
        Debug.Log("WRITING TO HIGH SCORES FILE " + entryList.Count);
        string path = Application.dataPath + "/Resources/Data/HighScores.txt";

        File.WriteAllText(path, string.Empty);

        List<string> lines = new List<string>();
        string fullText = "";
        foreach(data_HighScoreEntry en in entryList)
        {
            string line = en.Name + "," + en.Score + "\n";
            fullText += line;
            lines.Add(line);
        }
        print("Writing " + lines);
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(fullText);
        writer.Close();
    }

    /// <summary>
    /// CHECK HIGH SCORE
    /// </summary>
    /// <param name="e"></param>
    void CheckScoreCallback(GameEvent e)
    {
        print("Checking score");
        List<data_HighScoreEntry> entryList = ReadHighScoreList();
        print("Found " + entryList.Count);

        if (entryList.Count > 0)
        {
            bool foundHigher = false;
            int i = 0;
            for (; i < entryList.Count; i++)
            {
                data_HighScoreEntry entry = entryList[i];
                if (Score > entry.Score)
                {
                    foundHigher = true;
                    print("Current score " + Score + " is higher than " + entry.Score);
                    break;
                }
            }

            if(foundHigher)
            {
                eventCtrl.BroadcastEvent(typeof(ShowNameEntryEvt), new ShowNameEntryEvt(Score, i));
            }
            else
            {
                print("Current score " + Score + " is too low");
                GlobalEventController.GetInstance().BroadcastEvent(typeof(ShowHighScoreTable), new ShowHighScoreTable());
            }
        }
        else if (entryList.Count == 0 && Score > 0)
        {
            eventCtrl.BroadcastEvent(typeof(ShowNameEntryEvt), new ShowNameEntryEvt(Score, 0));
        }
        else
        {
            GlobalEventController.GetInstance().BroadcastEvent(typeof(ShowHighScoreTable), new ShowHighScoreTable());
        }
    }

    void RegisterNameCallback(GameEvent e)
    {
        print("REGISTERING NEW NAME");
        RegisterNewEntryEvt ev = (RegisterNewEntryEvt)e;

        List<data_HighScoreEntry> entryList = ReadHighScoreList();

        entryList.Insert(ev.entry.Index, ev.entry);

        if (entryList.Count > 7)
        {
            entryList.RemoveAt(entryList.Count - 1);
        }
        
        WriteHighScoreList(entryList);

        eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("displayTableTimer", 1, () => { GlobalEventController.GetInstance().BroadcastEvent(typeof(ShowHighScoreTable), new ShowHighScoreTable()); }));
    }

    void GameResetCallback(GameEvent e)
    {
        var items = Board.GetAllItems();
        foreach(var item in items)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        Score = 0;
        Lives = 5;

        Board = new GameBoard();

        Player = Instantiate(PlayerPrefab);
        bgmSrc.Play();

        eventCtrl.BroadcastEvent(typeof(StartTickEvt), new StartTickEvt());
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(StartTickEvt), StartTickCallback);
        eventCtrl.RemoveListener(typeof(StopTickEvt), StopTickCallback);
        eventCtrl.RemoveListener(typeof(RocketCollidedEvt), OnRocketCollided);
        eventCtrl.RemoveListener(typeof(AddScoreEvt), OnAddScore);
        eventCtrl.RemoveListener(typeof(StopBackgroundMusicEvt), StopBgmCallback);
        eventCtrl.RemoveListener(typeof(CheckHighScoreEvt), CheckScoreCallback);
        eventCtrl.RemoveListener(typeof(RegisterNewEntryEvt), RegisterNameCallback);
        eventCtrl.RemoveListener(typeof(GameStartEvt), GameResetCallback);
    }
}