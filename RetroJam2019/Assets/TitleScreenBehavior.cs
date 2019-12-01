using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenBehavior : MonoBehaviour
{
    public bool Active = true;
    public GameObject pressAnyKey;
    public GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && Active)
        {
            var eventCtrl = GlobalEventController.GetInstance();
            eventCtrl.BroadcastEvent(typeof(StartTickEvt), new StartTickEvt());
            GetComponent<Image>().enabled = false;
            pressAnyKey.SetActive(false);
            manager.GetComponent<AudioSource>().clip = manager.GetComponent<GameManager>().GameSong;
            manager.GetComponent<AudioSource>().Play();
            Active = false;
        }
    }
}
