using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions { UP, DOWN, LEFT, RIGHT }

public class TruckBehavior : BoardItemBehavior
{
    public GameObject CarPrefab;
    public Vector2 FacingDirection = new Vector2(1, 0); //x1 - Right, x-1 - Left, y-1 - Up, y1 - Down

    List<CarBehavior> cars = new List<CarBehavior>();
    CarBehavior lastCar = null;

    protected override void Start()
    {
        base.Start();
    }

    void Update200EvtCallback(GameEvent e)
    {
        Vector2 prevPos = transform.position;

        LastBoardPosition = BoardPosition;

        transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y * -1);
        BoardPosition = gameCtrl.Board.MoveItemInDirection(this, FacingDirection);

        Vector2 prevCarBoardPos = BoardPosition;

        foreach (CarBehavior c in cars)
        {
            c.MoveCar();
        }

        if (CheckCollision())
        {
            return;
        }

        if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Count > 0)
        {
            BoardItemBehavior bEv = gameCtrl.Board.GetCell(BoardPosition).BoardItems.Find(b => b is DebrisBehavior);
            if (bEv != null)
            {
                eventCtrl.BroadcastEvent(typeof(PickedUpDebrisEvt), new PickedUpDebrisEvt(bEv));

                BoardItemBehavior attachTo = null;
                if (lastCar != null)
                {
                    attachTo = lastCar;
                }
                else
                {
                    attachTo = this;
                }

                GameObject car = Instantiate(CarPrefab, gameCtrl.Board.GetWorldPosition(attachTo.LastBoardPosition), new Quaternion(0, 0, 0, 0));
                CarBehavior cB = car.GetComponent<CarBehavior>();
                cB.truckObj = this;
                cB.attachedTo = attachTo;


                gameCtrl.Board.AddItem(cB, attachTo.LastBoardPosition);
                cars.Add(cB);
                lastCar = cB;
            }
        }
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

        if(Input.GetKey(KeyCode.W) && FacingDirection.y != 1)
        {
            //transform.position += new Vector3(0, GameManager.TILE_Y);
            FacingDirection = new Vector2(0, -1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.A) && FacingDirection.x != 1)
        {
            //transform.position += new Vector3(-GameManager.TILE_X, 0);
            FacingDirection = new Vector2(-1, 0);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.S) && FacingDirection.y != -1)
        {
            //transform.position += new Vector3(0, -GameManager.TILE_Y);
            FacingDirection = new Vector2(0, 1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.D) && FacingDirection.x != -1)
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
        eventCtrl.RemoveListener(typeof(Update200Evt), Update200EvtCallback);
    }

    private bool CheckCollision()
    {
        if (BoardPosition.x < 0 || BoardPosition.y < 0
            || BoardPosition.x >= gameCtrl.Board.GetWidth() || BoardPosition.y >= gameCtrl.Board.GetHeight())
        {
            eventCtrl.BroadcastEvent(typeof(TruckCollidedEvt), new TruckCollidedEvt());
            Destroy(gameObject);
            return true;
        }

        if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Exists(i => i is CarBehavior || i is RocketBehavior))
        {
            eventCtrl.BroadcastEvent(typeof(TruckCollidedEvt), new TruckCollidedEvt());
            Destroy(gameObject);
            return true;
        }

        if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Exists(i => i is IncineratorBehavior))
        {
            eventCtrl.BroadcastEvent(typeof(DeliveredDebrisEvt), new DeliveredDebrisEvt());
            if(cars.Count > 0)
            {
                eventCtrl.BroadcastEvent(typeof(StopTickEvt), new StopTickEvt());

                CarBehavior c = cars[cars.Count-1];
                gameCtrl.Board.RemoveItem(c);
                c.DeliverCar();
                cars.Clear();
            }
            
        }

        return false;
    }
}
