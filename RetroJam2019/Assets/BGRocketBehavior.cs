using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGRocketBehavior : MonoBehaviour
{
    public Vector2 TargetBoardPos;
    public GameObject DebrisPrefab;
    public GameObject ExplosionPrefab;
    private Vector2 TargetWorldPos;
    private Animator animComp;
    private GameManager gameCtrl;
    private bool arrivedAtPos = false;
    private GlobalEventController eventCtrl;

    // Start is called before the first frame update
    void Start()
    {
        gameCtrl = GameManager.GetInstance();
        animComp = GetComponent<Animator>();
        TargetWorldPos = gameCtrl.Board.GetWorldPosition(new Vector2(TargetBoardPos.x, -TargetBoardPos.y));
        transform.position = new Vector2(TargetWorldPos.x, -19);
        eventCtrl = GlobalEventController.GetInstance();
        eventCtrl.QueueListener(typeof(GameStartEvt), new GlobalEventController.Listener(gameObject.GetInstanceID(), OnGameEnd));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        eventCtrl.RemoveListener(typeof(GameStartEvt), OnGameEnd);
    }

    void OnGameEnd( GameEvent e)
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0, 1 * Time.fixedDeltaTime, 0);

        if (transform.position.y >= TargetWorldPos.y)
        {
            transform.position = TargetWorldPos;
            arrivedAtPos = true;
        }

        if (arrivedAtPos)
        {
            //GameObject newDebris = Instantiate(DebrisPrefab, TargetWorldPos, Quaternion.identity);
            GameObject explosion = Instantiate(ExplosionPrefab, TargetWorldPos, Quaternion.identity);
            explosion.GetComponent<SmallShipExplosionBehavior>().BoardPosition = TargetBoardPos;
            explosion.GetComponent<SmallShipExplosionBehavior>().DebrisPrefab = DebrisPrefab;
            //gameCtrl.Board.AddItem(newDebris.GetComponent<DebrisBehavior>(), (int)TargetBoardPos.x, (int)TargetBoardPos.y);
            Destroy(gameObject);
        }
    }
}
