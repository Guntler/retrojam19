using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewEntryScoreBehavior : MonoBehaviour
{
    public AudioSettings InputSfx;
    public GameObject ScoreObj;
    public GameObject PlayerNameInputField;
    public int Score;
    public float TypingDelay = 0.1f;

    Text scoreText;
    Text playerNameText;

    GlobalEventController eventCtrl;
    
    string currentName = "";

    bool canType = false;

    void Start()
    {
        scoreText = ScoreObj.GetComponent<Text>();
        playerNameText = PlayerNameInputField.GetComponent<Text>();
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("canType" + gameObject.GetInstanceID(), 1, () => { canType = true; }));
    }

    void Update()
    {
        if (Input.anyKey && canType)
        {
            if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Backspace))
            {
                if(Input.GetKey(KeyCode.KeypadEnter) && currentName.Length > 0)
                {
                    eventCtrl.BroadcastEvent(typeof(PlayBackgroundClip), new PlayBackgroundClip(InputSfx));
                    data_HighScoreEntry data = new data_HighScoreEntry();
                    data.Name = currentName;
                    data.Score = Score;
                    eventCtrl.BroadcastEvent(typeof(RegisterNewEntryEvt), new RegisterNewEntryEvt(data));
                    Destroy(gameObject);
                }
                else if (Input.GetKey(KeyCode.Backspace))
                {
                    if(currentName.Length > 0)
                    {
                        currentName.Remove(currentName.Length - 1);
                    }
                    
                }
                
            }
            else
            {
                eventCtrl.BroadcastEvent(typeof(PlayBackgroundClip), new PlayBackgroundClip(InputSfx));
                currentName += Input.inputString;
            }

            canType = false;
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("canType" + gameObject.GetInstanceID(), TypingDelay, () => { canType = true; }));
        }

        playerNameText.text = currentName;
    }

    public void UpdateScore(int score)
    {
        Score = score;
        scoreText.text = Score.ToString();
    }
}
