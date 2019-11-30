using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    private GameManager _gameManager;
    private GameBoard _gameBoard;
    public GameObject debrisPrefab;

    private GlobalEventController eventController;
    private bool evtReady = false;
    public int SpawnRate = 3;
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
        }
        while (_gameBoard.GetBoard()[newX, 18].BoardItems.Count != 0);

        GameObject newRocket = Instantiate(debrisPrefab, _gameBoard.GetWorldPosition(new Vector2(newX, -18)) + new Vector2(0,0.5f), Quaternion.identity);
        _gameBoard.AddItem(newRocket.GetComponent<RocketBehavior>(), newX, 18);

        var rocket = newRocket.GetComponent<RocketBehavior>();

        _gameManager.Board.AddItem(rocket, newX, 18);
        _gameManager.Board.AddItem(rocket, newX, 19);
        rocket.BoardPosition = new Vector2(newX, 18);
        rocket.BottomBoardPosition = new Vector2(newX, 19);
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
