using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour

{//same as the key/power up script pretty much, just floaty

    [Header("Hover Animation")] // allows me to see in the inspector to change the values if i wanna
    //makes it go up and down so the player knows its interactable!!!
    public float floatAmplitude = 1f;   // how high it moves up/down
    public float floatFrequency = 2f;      // how fast it bobs

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // fancy way of making it float up and down
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, offsetY, 0);
    }

  
}


