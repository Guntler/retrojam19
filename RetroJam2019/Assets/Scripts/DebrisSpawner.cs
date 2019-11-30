using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    private GameManager _gameManager;
    private GameBoard _gameBoard;
    public GameObject debrisPrefab;

    private GlobalEventController eventController;

    public void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _gameBoard = _gameManager.Board;
        eventController = GlobalEventController.GetInstance();
        eventController.QueueListener(typeof(Update1000Evt), new GlobalEventController.Listener(GetInstanceID(), InstatiateAtRandomPos));
    }


    void InstatiateAtRandomPos(GameEvent e)
    {
        int newX, newY;
        
        do
        {
            newX = Random.Range(0, _gameBoard.GetWidth());
            newY = Random.Range(0, _gameBoard.GetHeight());
        }
        while (_gameBoard.GetBoard()[newX, newY].BoardItems.Count != 0);
        GameObject newDebris = Instantiate(debrisPrefab, new Vector2(newX, newY), Quaternion.identity);
        _gameBoard.AddItem(newDebris.GetComponent<DebrisBehavior>(), newX, newY);
    }

}
