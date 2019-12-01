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

    bool isDone = false;
    bool canType = false;

    List<KeyCode> ignoredKeyCodes = new List<KeyCode>();

    void Start()
    {
        ignoredKeyCodes.Add(KeyCode.Escape);
        ignoredKeyCodes.Add(KeyCode.Escape);
        ignoredKeyCodes.Add(KeyCode.Return);
        ignoredKeyCodes.Add(KeyCode.Backspace);
        ignoredKeyCodes.Add(KeyCode.LeftControl);
        ignoredKeyCodes.Add(KeyCode.RightControl);
        ignoredKeyCodes.Add(KeyCode.LeftShift);
        ignoredKeyCodes.Add(KeyCode.RightShift);
        ignoredKeyCodes.Add(KeyCode.Tab);
        scoreText = ScoreObj.GetComponent<Text>();
        playerNameText = PlayerNameInputField.GetComponent<Text>();
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("canType" + gameObject.GetInstanceID(), 1, () => { canType = true; }));
    }

    void Update()
    {
        if(isDone)
        {
            return;
        }

        if (Input.anyKey && canType)
        {
            bool shouldIgnoreKey = false;
            foreach(KeyCode c in ignoredKeyCodes)
            {
                if(Input.GetKey(c))
                {
                    shouldIgnoreKey = true;
                    break;
                }
            }

            if (shouldIgnoreKey)
            {
                if((Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)) && currentName.Length > 0)
                {
                    isDone = true;
                    print("SUBMITTING NAME");
                    eventCtrl.BroadcastEvent(typeof(PlayOneshotClipEvent), new PlayOneshotClipEvent(InputSfx));
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
                        currentName = currentName.Remove(currentName.Length - 1);
                    }
                    
                }
                
            }
            else
            {
                eventCtrl.BroadcastEvent(typeof(PlayOneshotClipEvent), new PlayOneshotClipEvent(InputSfx));
                currentName += Input.inputString;
            }

            canType = false;
            eventCtrl.BroadcastEvent(typeof(StartTimerEvent), new StartTimerEvent("canType" + gameObject.GetInstanceID(), TypingDelay, () => { canType = true; }));
        }

        playerNameText.text = currentName;
    }

    public void UpdateScore(int score)
    {
        scoreText = ScoreObj.GetComponent<Text>();

        Score = score;
        scoreText.text = Score.ToString();
    }
}
