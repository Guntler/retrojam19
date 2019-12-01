﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimerCallback();

public class Timer
{
    public TimerCallback Callback;
    public string Id = "";
    public float Time = 0;
    public float ElapsedTime = 0;
    public bool IsPaused = false;
    public bool IsDone = false;
    public bool DequeueAtEnd = false;
    public bool IsBroadcast = false;
    public bool IsProgramPaused = false;

    public Timer(string id, float time, bool dequeue, TimerCallback callback)
    {
        Id = id;
        Time = time;
        DequeueAtEnd = dequeue;

        Callback = callback;
    }

    public void Tick(float time)
    {
        if (IsProgramPaused)
        {
            return;
        }
            
        if(!IsDone && !IsPaused) {
            //Debug.Log(ElapsedTime + " -- " + Time + " -- " + Callback);
            ElapsedTime += time;
            IsDone = ElapsedTime >= Time;

            if (IsDone) {
                ElapsedTime = Time;
                if(Callback != null) {
                    //Debug.Log("CALLING CALLBACK");
                    Callback();
                }
            }
        }
        else {
        }
    }

    public void Restart()
    {
        ElapsedTime = 0;
        IsDone = false;
    }

    public void ToggleProgramPause(bool state)
    {
        IsProgramPaused = state;
    }

    public void Stop()
    {
        Callback = null;
        ElapsedTime = Time;
        IsDone = true;
    }
}

public class TimerManager : MonoBehaviour {
    GlobalEventController eventCtrl;

    /*private static TimerManager s_Instance;
    private TimerManager() { }*/

    public List<Timer> Timers = new List<Timer>();

    public bool IsEventReady = false;

	void Start () {
        eventCtrl = GlobalEventController.GetInstance();
    }

    void SetupEvents()
    {
        print("Setting up Timer Events with id " + GetInstanceID());

        IsEventReady = true;
        eventCtrl.QueueListener(typeof(StartTimerEvent), new GlobalEventController.Listener(GetInstanceID(), StartTimerCallback));
        eventCtrl.QueueListener(typeof(TimerOverEvent), new GlobalEventController.Listener(GetInstanceID(), TimerOverCallback));
        eventCtrl.QueueListener(typeof(PauseTimerEvent), new GlobalEventController.Listener(GetInstanceID(), PauseTimerCallback));
        eventCtrl.QueueListener(typeof(ResumeTimerEvent), new GlobalEventController.Listener(GetInstanceID(), ResumeTimerCallback));
        eventCtrl.QueueListener(typeof(StopTimerEvent), new GlobalEventController.Listener(GetInstanceID(), StopTimerCallback));
    }

    void Awake()
    {
        //s_Instance = this;
    }

    /*public static TimerManager GetInstance()
    {
        if (s_Instance)
            return s_Instance;
        else
            return null;
    }*/

    public void StartTimerCallback(GameEvent e)
    {
        StartTimerEvent ev = (StartTimerEvent)e;
        Timers.Add(new Timer(ev.TimerId, ev.Duration, true, ev.Callback));
    }

    public void PauseTimerCallback(GameEvent e)
    {
        PauseTimerEvent ev = (PauseTimerEvent)e;
        Timer t = Timers.Find(tT => tT.Id == ev.TimerId);
        if(t != null) {
            t.IsPaused = true;
        }
    }

    public void ResumeTimerCallback(GameEvent e)
    {
        ResumeTimerEvent ev = (ResumeTimerEvent)e;
        Timer t = Timers.Find(tT => tT.Id == ev.TimerId);
        if (t != null) {
            t.IsPaused = false;
        }
    }

    public void StopTimerCallback(GameEvent e)
    {
        StopTimerEvent ev = (StopTimerEvent)e;
        Timer t = Timers.Find(tT => tT.Id == ev.TimerId);
        if(t != null)
            t.IsDone = true;
        //eventCtrl.BroadcastEvent(typeof(TimerOverEvent), new TimerOverEvent(ev.TimerId));
    }

    public void TimerOverCallback(GameEvent e)
    {
        TimerOverEvent ev = (TimerOverEvent)e;
        Timer t = Timers.Find(tT => tT.Id == ev.TimerId);

        if(Timers.Remove(t)) {

        }
    }
    
    void Update () {
        if(!IsEventReady)
            SetupEvents();

        Timer[] tArr = Timers.ToArray();

        for (int i = 0; i < Timers.Count; i++) {
            Timer t = Timers[i];

            if(t.IsPaused)
            {
                continue;
            }

            //print("TICKING " + t.Id);
            t.Tick(Time.unscaledDeltaTime);

            if(!t.IsBroadcast && t.IsDone && t.DequeueAtEnd) {
                t.IsBroadcast = true;
                //eventCtrl.BroadcastEvent(typeof(TimerOverEvent), new TimerOverEvent(t.Id));
                Timers.RemoveAt(i);
                i--;
            }
        }
	}

    public void QueueTimer(string id, float time, TimerCallback callback)
    {
        Timers.Add(new Timer(id, time, true, callback));
    }

    public bool HasTimer(string id)
    {
        return Timers.Find(t => t.Id == id) != null;
    }

    public void RemoveTimer(string id)
    {
        Timer l_tToRem = Timers.Find(t => t.Id == id);
        if(l_tToRem != null)
            Timers.Remove(l_tToRem);
    }

    private void OnApplicationFocus(bool focus)
    {
        Timer[] tArr = Timers.ToArray();

        for (int i = 0; i < Timers.Count; i++)
        {
            Timer t = Timers[i];
            t.ToggleProgramPause(false);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Timer[] tArr = Timers.ToArray();

        for (int i = 0; i < Timers.Count; i++)
        {
            Timer t = Timers[i];
            t.ToggleProgramPause(true);
        }
    }

    private void OnApplicationQuit()
    {
        Timer[] tArr = Timers.ToArray();

        for (int i = 0; i < Timers.Count; i++)
        {
            Timer t = Timers[i];
            t.Stop();
        }

        Timers.Clear();
    }
}
