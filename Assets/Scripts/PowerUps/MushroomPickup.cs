using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Require the touching thing to be the player
        if (!other.CompareTag("Player")) return;

        // Find the PlayerLifes component (works if collider is on a child)
        var player = other.GetComponentInParent<PlayerLifes>();
        if (player == null) return;

        // Give +1 life with built-in clamping to maxLives
        player.GiveLife(1);


        // Remove the mushroom
        Destroy(gameObject);
    }
}
