using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : BoardItemBehavior
{
    int countDownTime = 10;
    int dropDebrisTime = 13;
    bool hasLaunched = false;
    bool droppedDebris = false;
    public Vector2 BottomBoardPosition;
    public List<float> frameTimes = new List<float>();
    Animator animComp;
    public GameObject debrisPrefab;

    protected override void Start()
    {
        base.Start();
        animComp = GetComponent<Animator>();
        animComp.Play("RocketAnim", 0, 0);
        dropDebrisTime = dropDebrisTime + Random.Range(-4, 4);
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
            dropDebrisTime--;
            LastBoardPosition = BoardPosition;

            transform.position += new Vector3(0, GameManager.TILE_Y);
            BoardPosition = gameCtrl.Board.MoveItemInDirection(this, new Vector2(0,-1));
            BottomBoardPosition = new Vector2(BoardPosition.x, BoardPosition.y + 1);

            if (dropDebrisTime <= 0 && !droppedDebris)
            {
                droppedDebris = true;
                GameObject newDebris = Instantiate(debrisPrefab, gameCtrl.Board.GetWorldPosition(new Vector2(BottomBoardPosition.x, -(BottomBoardPosition.y+1))), Quaternion.identity);
                gameCtrl.Board.AddItem(newDebris.GetComponent<DebrisBehavior>(), (int)BottomBoardPosition.x, (int)BottomBoardPosition.y+1);
                newDebris.GetComponent<Animator>().Play("DebrisAnim", 0, 0);
                newDebris.GetComponent<DebrisBehavior>().ForceAnim = true;

                animComp.Play("RocketAnim", 0, 0.5f);
            }

            CheckCollision();
        }
    }

    private void CheckCollision()
    {
        if (BoardPosition.x < 0 || BoardPosition.y < 0
            || BoardPosition.x >= gameCtrl.Board.GetWidth() || BoardPosition.y >= gameCtrl.Board.GetHeight())
        {
            eventCtrl.BroadcastEvent(typeof(AddScoreEvt), new AddScoreEvt(GameManager.SCORE_PER_ROCKET));
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