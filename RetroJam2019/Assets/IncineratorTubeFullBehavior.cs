using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncineratorTubeFullBehavior : MonoBehaviour
{
    public float MovementRate = 0.25f;
    public int UnitsToMove = 4;

    private float unitsMoved = 0;
    
    void Update()
    {
        unitsMoved += UnitsToMove;

        if(unitsMoved >= UnitsToMove)
        {
            Destroy(gameObject);
        }
    }
}
