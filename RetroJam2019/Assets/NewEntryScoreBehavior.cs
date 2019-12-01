using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewEntryScoreBehavior : MonoBehaviour
{
    public GameObject ScoreObj;
    public GameObject PlayerNameInputField;

    Text scoreText;
    Text playerNameText;

    GlobalEventController eventCtrl;


    string currentName = "";

    bool canType = true;

    void Start()
    {
        scoreText = ScoreObj.GetComponent<Text>();
        playerNameText = PlayerNameInputField.GetComponent<Text>();
        eventCtrl = GlobalEventController.GetInstance();

    }

    void Update()
    {
        if(Input.anyKey)
        {
            if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Backspace))
            {
                //TODO: end
            }
            else
            {
                currentName += Input.inputString;
            }
        }

        playerNameText.text = currentName;
    }
}
