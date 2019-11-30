using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        float unitsPerPixel = 800 / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        cam.orthographicSize = desiredHalfHeight;

        print(cam.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
