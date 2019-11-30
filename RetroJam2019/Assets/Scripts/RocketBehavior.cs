using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : BoardItemBehavior
{
    int countDownTime = 10;
    bool hasLaunched = false;
    public Vector2 BottomBoardPosition;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        if (!isEventReady)
        {
            eventCtrl.QueueListener(typeof(Update500Evt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnUpdate));
            isEventReady = true;
            BottomBoardPosition = new Vector2(BoardPosition.x, BoardPosition.y + 1);

            if (!gameCtrl.Board.HasItem(this))
            {
                gameCtrl.Board.AddItem(this, 11, 18);
                gameCtrl.Board.AddItem(this, 11, 19);
            }
        }
    }

    private void OnUpdate(GameEvent e)
    {
        if (!hasLaunched)
        {
            countDownTime--;

            if (countDownTime <= 0)
            {
                hasLaunched = true;
                eventCtrl.BroadcastEvent(typeof(LaunchedRocketEvt), new LaunchedRocketEvt());
            }
        }
        else
        {
            LastBoardPosition = BoardPosition;

            transform.position += new Vector3(0, GameManager.TILE_Y);
            BoardPosition = gameCtrl.Board.MoveItemInDirection(this, new Vector2(0,-1));
            BottomBoardPosition = new Vector2(BoardPosition.x, BoardPosition.y + 1);

            CheckCollision();
        }
    }

    private void CheckCollision()
    {
        if (BoardPosition.x < 0 || BoardPosition.y < 0
            || BoardPosition.x >= gameCtrl.Board.GetWidth() || BoardPosition.y >= gameCtrl.Board.GetHeight())
        {
            eventCtrl.BroadcastEvent(typeof(AddScoreEvt), new AddScoreEvt());
            Destroy(gameObject);
        }
        else if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Exists(i => i is DebrisBehavior || i is CarBehavior || i is TruckBehavior))
        {
            eventCtrl.BroadcastEvent(typeof(RocketCollidedEvt), new RocketCollidedEvt());
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        gameCtrl.Board.RemoveItem(this);
        eventCtrl.RemoveListener(typeof(Update500Evt), OnUpdate);
    }
}