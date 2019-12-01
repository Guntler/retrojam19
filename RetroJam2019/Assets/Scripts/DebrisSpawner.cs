using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{

    private GameManager _gameManager;
    private GameBoard _gameBoard;
    public GameObject debrisPrefab;
    public GameObject rocketPrefab;

    private GlobalEventController eventController;
    private bool evtReady = false;
    public int SpawnRate = 8;
    private int spawnTick = 0;
    private List<Vector2> reservedPositions = new List<Vector2>();

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

        if (reservedPositions.Count > 0)
        {
            reservedPositions.RemoveAt(0);
        }

        do
        {
            newX = Random.Range(3, _gameBoard.GetWidth() - 3);
            newY = Random.Range(2, _gameBoard.GetHeight() - 8);
        }
        while (_gameBoard.GetBoard()[newX, newY].BoardItems.Count != 0 || HasRocketInColumn(newX) || reservedPositions.Contains(new Vector2(newX, newY)));

        reservedPositions.Add(new Vector2(newX, newY));

        GameObject newDebrisRocket = Instantiate(rocketPrefab, _gameBoard.GetWorldPosition(new Vector2(newX, -_gameBoard.GetHeight())), Quaternion.identity);
        newDebrisRocket.GetComponent<BGRocketBehavior>().DebrisPrefab = debrisPrefab;
        newDebrisRocket.GetComponent<BGRocketBehavior>().TargetBoardPos = new Vector2(newX, newY);
        /*GameObject newDebris = Instantiate(debrisPrefab, _gameBoard.GetWorldPosition(new Vector2(newX, -newY)), Quaternion.identity);
        _gameBoard.AddItem(newDebris.GetComponent<DebrisBehavior>(), newX, newY);*/
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
            eventController.QueueListener(typeof(GameStartEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnGameEnd));
        }

        if (_gameManager.Score > 1000)
        {
            SpawnRate = 7;
        }
        else if (_gameManager.Score > 2000)
        {
            SpawnRate = 6;
        }
        else if (_gameManager.Score > 3000)
        {
            SpawnRate = 5;
        }
        else if (_gameManager.Score > 4000)
        {
            SpawnRate = 4;
        }
        else if (_gameManager.Score > 5000)
        {
            SpawnRate = 3;
        }
    }

    void OnGameEnd(GameEvent e)
    {
        reservedPositions.Clear();
        _gameBoard = _gameManager.Board;
        SpawnRate = 8;
        spawnTick = 0;
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
