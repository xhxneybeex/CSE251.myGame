using UnityEngine;

public class Coin_delete : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f; // how fast the coin falls
    [SerializeField] private float destroyY = -7f; // y position where it gets deleted

    void Update()
    {
        // move the coin straight down every frame
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // if it goes too low, destroy it
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collected the coin!");
            Destroy(gameObject);
        }
    }
}
