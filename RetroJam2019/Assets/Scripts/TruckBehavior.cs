using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions { UP, DOWN, LEFT, RIGHT }

public class TruckBehavior : BoardItemBehavior
{
    public GameObject CarPrefab;
    public Vector2 FacingDirection = new Vector2(1, 0); //x1 - Right, x-1 - Left, y1 - Up, y-1 - Down

    List<CarBehavior> cars = new List<CarBehavior>();

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        Vector2 prevPos = transform.position;
        bool hasMoved = false;

        if(Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, GameManager.TILE_Y);
            FacingDirection = new Vector2(-1, 0);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-GameManager.TILE_X, 0);
            FacingDirection = new Vector2(0, -1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -GameManager.TILE_Y);
            FacingDirection = new Vector2(0, 1);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(GameManager.TILE_X, 0);
            FacingDirection = new Vector2(1, 0);
            hasMoved = true;
        }
        else if (Input.GetKey(KeyCode.G))
        {
            GameObject car = Instantiate(CarPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            transform.position += new Vector3(GameManager.TILE_X * FacingDirection.x, GameManager.TILE_Y * FacingDirection.y);
            CarBehavior cB = car.GetComponent<CarBehavior>();
            cB.truckObj = this;
            cars.Add(cB);
        }

        if(hasMoved)
        {
            foreach(CarBehavior c in cars)
            {
                Vector2 newPos = c.transform.position;
                c.transform.position = prevPos;

                prevPos = newPos;
            }
        }
    }


}
