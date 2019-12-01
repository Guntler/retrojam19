using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntryBehavior : MonoBehaviour
{
    public GameObject ScoreNameObj;
    public GameObject ScoreTextObj;
    public string PlayerName;
    public int EntryIndex;
    public int ScoreAmt;

    Text playerNameText;
    Text scoreText;

    void Start()
    {
        playerNameText = ScoreNameObj.GetComponent<Text>();
        scoreText = ScoreTextObj.GetComponent<Text>();
        playerNameText.text = EntryIndex.ToString() + ". " + PlayerName;
        scoreText.text = ScoreAmt.ToString();
    }
}
