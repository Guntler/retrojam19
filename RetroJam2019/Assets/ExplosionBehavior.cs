using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{

    public AudioClip ExplosionSound;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(ExplosionSound, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
