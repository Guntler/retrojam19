using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions { UP, DOWN, LEFT, RIGHT }

public class TruckBehavior : BoardItemBehavior
{
    public GameObject CarPrefab;
    public Vector2 FacingDirection = new Vector2(1, 0); //x1 - Right, x-1 - Left, y-1 - Up, y1 - Down

    List<CarBehavior> cars = new List<CarBehavior>();

    protected override void Start()
    {
        base.Start();
    }

    void Update200EvtCallback(GameEvent e)
    {
        transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y * -1);
        BoardPosition = gameCtrl.Board.MoveItemInDirection(this, FacingDirection);

        Vector2 prevPos = transform.position;
        Vector2 prevBoardPos = BoardPosition;

        foreach (CarBehavior c in cars)
        {
            Vector2 newBoardPos = c.BoardPosition;
            Vector2 newPos = c.transform.position;
            c.transform.position = prevPos;
            prevPos = newPos;

            BoardPosition = gameCtrl.Board.MoveItemToPosition(c, prevBoardPos);
            prevPos = newBoardPos;
        }

        if(gameCtrl.Board.GetCell(BoardPosition).BoardItems.Count > 0)
        {
            BoardItemBehavior bEv = gameCtrl.Board.GetCell(BoardPosition).BoardItems.Find(b => b is DebrisBehavior);
            if(bEv != null)
            {
                eventCtrl.BroadcastEvent(typeof(PickedUpDebrisEvt), new PickedUpDebrisEvt(bEv));

                GameObject car = Instantiate(CarPrefab, transform.position, new Quaternion(0, 0, 0, 0));
                transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y * -1);
                CarBehavior cB = car.GetComponent<CarBehavior>();
                cB.truckObj = this;

                gameCtrl.Board.AddItem(cB, (int)BoardPosition.x, (int)BoardPosition.y);
                cars.Add(cB);
            }
        }
        print(gameCtrl.Board.GetCell(BoardPosition).BoardItems.Count);


    }

    void FixedUpdate()
    {
        if(!isEventReady)
        {
            isEventReady = true;

            gameCtrl.Board.AddItem(this, BoardPosition);
            eventCtrl.QueueListener(typeof(Update200Evt), new GlobalEventController.Listener(gameObject.GetInstanceID(), Update200EvtCallback));

        }
        
        bool hasMoved = false;

        if(Input.GetKey(KeyCode.W))
        {
            //transform.position += new Vector3(0, GameManager.TILE_Y);
            FacingDirection = new Vector2(0, -1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += new Vector3(-GameManager.TILE_X, 0);
            FacingDirection = new Vector2(-1, 0);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += new Vector3(0, -GameManager.TILE_Y);
            FacingDirection = new Vector2(0, 1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += new Vector3(GameManager.TILE_X, 0);
            FacingDirection = new Vector2(1, 0);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.G))
        {
            GameObject car = Instantiate(CarPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y * -1);
            CarBehavior cB = car.GetComponent<CarBehavior>();
            cB.truckObj = this;

            gameCtrl.Board.AddItem(cB, (int)BoardPosition.x, (int)BoardPosition.y);
            cars.Add(cB);
        }

        if(hasMoved)
        {
            
        }
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(Update100Evt), Update200EvtCallback);
    }
}
