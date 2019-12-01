﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySfxEvent : GameEvent
{
    public string sfxSettingName;
}

public class GlobalAudioController : MonoBehaviour
{
    public int GlobalMusicSrcs = 2;
    public int GlobalVoiceSrcs = 2;
    public int MaxGlobalSfxSrcs = 10;

    Queue<AudioSource> sfxSrcQueue;
    List<SfxInstance> sfxInstanceList;

    struct SfxInstance
    {
        public AudioSource sfxSrc;
        public AudioSettings sfxSetting;
        public bool isPaused;
    }

    GlobalEventController eventCtrl;
    AudioSource masterSrc;

    public bool IsEventReady = false;

    void Start()
    {
        GameObject masterSrcObj = new GameObject("MasterAudioSourceObject");
        masterSrcObj.transform.parent = transform;

        masterSrc = masterSrcObj.AddComponent<AudioSource>();
        

        eventCtrl = GlobalEventController.GetInstance();


        sfxInstanceList = new List<SfxInstance>();
        sfxSrcQueue = new Queue<AudioSource>();

        for (int i=0; i<MaxGlobalSfxSrcs; i++)
        {
            sfxSrcQueue.Enqueue(masterSrcObj.AddComponent<AudioSource>());
        }

        
    }

    void SetupEvents()
    {
        print("Setting up Audio Events with id " + GetInstanceID());

        IsEventReady = true;
        eventCtrl.QueueListener(typeof(FadeAudioEvent), new GlobalEventController.Listener(GetInstanceID(), FadeAudioVolumeCallback));
        eventCtrl.QueueListener(typeof(PlayOneshotClipEvent), new GlobalEventController.Listener(GetInstanceID(), PlayOneshotClipCallback));
        eventCtrl.QueueListener(typeof(PlayBackgroundClip), new GlobalEventController.Listener(GetInstanceID(), PlayBackgroundClipCallback));
    }

    void Update()
    {
        if (!IsEventReady) {
            SetupEvents();
            return;
        }
    }

    void FixedUpdate()
    {
        for(int i=0; i<sfxInstanceList.Count; i++)
        {
            SfxInstance inst = sfxInstanceList[i];
            if (!inst.sfxSrc.isPlaying && !inst.isPaused)
            {
                sfxInstanceList.Remove(inst);
                i--;

                inst.sfxSrc.clip = null;
                inst.sfxSrc.loop = false;
                inst.sfxSrc.volume = 1;
                sfxSrcQueue.Enqueue(inst.sfxSrc);
            }
        }
    }

    void PlaySfxCallback(GameEvent e)
    {

    }

    public void PlayOneshotClipCallback(GameEvent e)
    {
        PlayOneshotClipEvent ev = (PlayOneshotClipEvent)e;
        masterSrc.PlayOneShot(ev.AudioObject.Clip, ev.AudioObject.DefaultVolume);
    }

    public void PlayBackgroundClipCallback(GameEvent e)
    {
        
        PlayBackgroundClip ev = (PlayBackgroundClip)e;
        masterSrc.volume = ev.AudioObject.DefaultVolume;
        masterSrc.pitch = ev.AudioObject.DefaultPitch;
        masterSrc.clip = ev.AudioObject.Clip;

        eventCtrl.BroadcastEvent(typeof(FadeAudioEvent), new FadeAudioEvent(null, ev.AudioObject.DefaultVolume, ev.FadeRate, ev.FadeDelay));
        masterSrc.Play();
    }

    public void FadeAudioVolumeCallback(GameEvent e)
    {
        FadeAudioEvent ev = (FadeAudioEvent)e;
        StopCoroutine("FadeAudioVolume");
        StartCoroutine(FadeAudioVolume(ev.AudioSrc != null ? ev.AudioSrc : masterSrc, ev.TargetVolume, ev.FadeRate, ev.FadeDelay));
    }

    public IEnumerator FadeAudioVolume(AudioSource src, float target, float rate, float delay)
    {
        while (!Utilities.FloatApprox(src.volume, target, 0.1f)) {
            if (!src)
                break;
            src.volume = Mathf.Lerp(src.volume, target, rate * Time.deltaTime);
            yield return new WaitForSeconds(delay);
        }

        if (src) {
            src.volume = target;
        }

        eventCtrl.BroadcastEvent(typeof(FadeAudioCompleteEvent), new FadeAudioCompleteEvent());

        yield return null;
    }
}
