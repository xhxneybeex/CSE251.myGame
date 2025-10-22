using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour

{//damage the player 1.
    public int damage = 1;
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
        //if collide with player, take 1 of their lives basically 
    {
        if (other.CompareTag(playerTag))
        {
            var lives = other.GetComponent<PlayerLifes>();
            if (lives != null)
            {
                lives.TakeHit(damage);
                Debug.Log("Dealt damage to player");
            }
        }
    }
}
