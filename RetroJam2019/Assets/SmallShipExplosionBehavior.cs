using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShipExplosionBehavior : MonoBehaviour
{
    public GameObject DebrisPrefab;

    void Start()
    {
        
    }

    public void SpawnDebris()
    {
        GameObject debris = Instantiate(DebrisPrefab, transform.position, new Quaternion(0, 0, 0, 0));

        Destroy(gameObject);
    }
}
