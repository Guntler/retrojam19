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
    public int SpawnRate = 2;
    private int spawnTick = 0;

    public void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void InstatiateAtRandomPos(GameEvent e)
    {
        int newX, newY;
        spawnTick++;

        if (spawnTick % SpawnRate != 0)
        {
            return;
        }

        do
        {
            newX = Random.Range(3, _gameBoard.GetWidth() - 3);
            newY = Random.Range(2, _gameBoard.GetHeight() - 8);
        }
        while (_gameBoard.GetBoard()[newX, newY].BoardItems.Count != 0 || HasRocketInColumn(newX));

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

    private bool HasRocketInColumn(int x)
    {
        for (var y = 0; y < _gameManager.Board.GetHeight(); y++)
        {
            if (_gameManager.Board.GetCell(x,y).BoardItems.Exists(i => i is RocketBehavior))
            {
                return true;
            }
        }

        return false;
    }
}
