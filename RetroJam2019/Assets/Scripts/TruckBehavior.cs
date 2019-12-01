using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions { UP, DOWN, LEFT, RIGHT }

public class TruckBehavior : BoardItemBehavior
{
    public GameObject CarPrefab;
    public Vector2 FacingDirection = new Vector2(1, 0); //x1 - Right, x-1 - Left, y-1 - Up, y1 - Down

    /// <summary>
    /// Minimum amount of crates to earn a new multiplier level
    /// </summary>
    public int CarIntervalForMultiplier = 5;

    private int currentMultiplier = 1;
    private int carsDelivered = 0;

    public AudioClip PickupSound;
    
    AudioSource sfxSrc;
    public AudioSettings PlayerDeathSfx;
    public AudioClip ExplosionSound;
    public GameObject Explosion;

    List<CarBehavior> cars = new List<CarBehavior>();
    CarBehavior lastCar = null;
    Animator animComp;

    protected override void Start()
    {
        base.Start();
        animComp = GetComponent<Animator>();
        sfxSrc = GetComponent<AudioSource>();
    }

    void Update200EvtCallback(GameEvent e)
    {
        Vector2 prevPos = transform.position;

        LastBoardPosition = BoardPosition;

        transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y * -1);
        BoardPosition = gameCtrl.Board.MoveItemInDirection(this, FacingDirection);

        Vector2 prevCarBoardPos = BoardPosition;

        sfxSrc.Play();

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
                sfxSrc.PlayOneShot(PickupSound, 1);
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

                if(lastCar != null)
                {
                    lastCar.attachesTo = cB;
                }
                


                gameCtrl.Board.AddItem(cB, attachTo.LastBoardPosition);
                cars.Add(cB);
                lastCar = cB;
            }
        }
    }

    void DeliverCarEvtCallback(GameEvent e)
    {
        carsDelivered++;
        if(carsDelivered % CarIntervalForMultiplier == 0)
        {
            currentMultiplier++;
            eventCtrl.BroadcastEvent(typeof(CreateScoreMultiplierTextEvt), new CreateScoreMultiplierTextEvt(currentMultiplier));
        }
        eventCtrl.BroadcastEvent(typeof(AddScoreEvt), new AddScoreEvt(GameManager.SCORE_PER_DEBRIS * currentMultiplier));
        
    }

    void DeliveredDebrisEvtCallback(GameEvent e)
    {
        carsDelivered = 0;
        currentMultiplier = 1;
    }

    private void Update()
    {
        animComp.SetFloat("FacingDirectionX", FacingDirection.x);
        animComp.SetFloat("FacingDirectionY", FacingDirection.y);
    }

    void FixedUpdate()
    {
        if(!isEventReady)
        {
            isEventReady = true;

            gameCtrl.Board.AddItem(this, BoardPosition);
            eventCtrl.QueueListener(typeof(Update200Evt), new GlobalEventController.Listener(gameObject.GetInstanceID(), Update200EvtCallback));
            eventCtrl.QueueListener(typeof(DeliverCarEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), DeliverCarEvtCallback));
            eventCtrl.QueueListener(typeof(DeliveredDebrisEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), DeliveredDebrisEvtCallback));
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
        eventCtrl.BroadcastEvent(typeof(PlayBackgroundClip), new PlayBackgroundClip(PlayerDeathSfx));

        eventCtrl.RemoveListener(typeof(Update200Evt), Update200EvtCallback);
        eventCtrl.RemoveListener(typeof(DeliverCarEvt), DeliverCarEvtCallback);
        eventCtrl.RemoveListener(typeof(DeliveredDebrisEvt), DeliveredDebrisEvtCallback);
    }

    private bool CheckCollision()
    {
        if (BoardPosition.x < 0 || BoardPosition.y < 0
            || BoardPosition.x >= gameCtrl.Board.GetWidth() || BoardPosition.y >= gameCtrl.Board.GetHeight())
        {
            DestroyTruck();
            return true;
        }

        if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Exists(i => i is CarBehavior || i is RocketBehavior))
        {
            DestroyTruck();
            return true;
        }

        if (gameCtrl.Board.GetCell(BoardPosition).BoardItems.Exists(i => i is IncineratorBehavior))
        {
            if(cars.Count > 0)
            {
                eventCtrl.BroadcastEvent(typeof(StopTickEvt), new StopTickEvt());

                CarBehavior c = lastCar;
                gameCtrl.Board.RemoveItem(c);
                c.DeliverCar();
                cars.Clear();
            }
            
        }

        return false;
    }

    public void DestroyTruck()
    {
        eventCtrl.BroadcastEvent(typeof(StopBackgroundMusicEvt), new StopBackgroundMusicEvt());
        eventCtrl.BroadcastEvent(typeof(TruckCollidedEvt), new TruckCollidedEvt());
        eventCtrl.BroadcastEvent(typeof(StopTickEvt), new StopTickEvt());

        if(lastCar)
        {
            CarBehavior c = lastCar;
            gameCtrl.Board.RemoveItem(c);
            c.DestroyCar();
        }
        else
        {
            Destroy(gameObject);
        }
        cars.Clear();
        

        gameCtrl.RemoveLives();
        GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
    }
}
