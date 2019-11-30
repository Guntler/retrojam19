using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{

    private GameManager _gameManager;
    private GameBoard _gameBoard;
    public GameObject debrisPrefab;

    private GlobalEventController eventController;
    private bool evtReady = false;

    public void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void InstatiateAtRandomPos(GameEvent e)
    {
        int newX, newY;

        do
        {
            print("Cenas");
            newX = Random.Range(0, _gameBoard.GetWidth());
            newY = Random.Range(0, _gameBoard.GetHeight());
        }
        while (_gameBoard.GetBoard()[newX, newY].BoardItems.Count != 0);

        GameObject newDebris = Instantiate(debrisPrefab, _gameBoard.GetWorldPosition(new Vector2(newX, -newY)), Quaternion.identity);
        _gameBoard.AddItem(newDebris.GetComponent<DebrisBehavior>(), newX, newY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!evtReady)
        {
            _gameBoard = _gameManager.Board;
            eventController = GlobalEventController.GetInstance();
            eventController.QueueListener(typeof(Update1000Evt), new GlobalEventController.Listener(GetInstanceID(), InstatiateAtRandomPos));
            evtReady = true;
        }
    }
}
