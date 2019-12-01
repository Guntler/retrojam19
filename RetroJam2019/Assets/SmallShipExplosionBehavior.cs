using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShipExplosionBehavior : MonoBehaviour
{
    public GameObject DebrisPrefab;
    public Vector2 BoardPosition;

    void Start()
    {
        
    }

    public void SpawnDebris()
    {
        GameObject debris = Instantiate(DebrisPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        DebrisBehavior debBev = debris.GetComponent<DebrisBehavior>();
        GameManager.GetInstance().Board.AddItem(debBev, BoardPosition);
        Destroy(gameObject);
    }
}
