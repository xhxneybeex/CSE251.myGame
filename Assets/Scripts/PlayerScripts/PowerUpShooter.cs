using UnityEngine;

public class PowerUpShooter : MonoBehaviour
{
    [Header("Hover Animation")] // allows me to see in the inspector to change the values if i wanna
    //makes it go up and down so the player knows its interactable!!!
    public float floatAmplitude = 0.25f;   // how high it moves up/down
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the thing that collided with this power-up isn't the player, do nothing

        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)

        //if the player is not null, give them the big shot power-up :D
        {
            player.BigShotPowerUp();   // trigger your new big shot power-up
        }

        Destroy(gameObject);
    }
}

