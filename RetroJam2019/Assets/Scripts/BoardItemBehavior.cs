using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardItemBehavior : MonoBehaviour
{
    protected GlobalEventController eventCtrl;

    protected virtual void Start()
    {
        eventCtrl = GlobalEventController.GetInstance();
    }
}
