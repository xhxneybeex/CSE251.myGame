using UnityEngine;

public class Coin_delete : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f; //how fast coin falls    
    [SerializeField] private float destroyY = -7f; // Y position to destroy coin, so it doesnt fill up hierarchy and make memory leak.
    [SerializeField] private int coinValue = 1; // how many coins  pickup gives

    private UIManager uiManager;
    private bool collected = false;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not found in scene. Coins will not display.");
        }
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < destroyY) //y pos to destroy coin
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (uiManager != null)
            {
                uiManager.AddCoin(coinValue);
            }

            // disable visuals immediately to avoid double coin collection.
            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;

            Destroy(gameObject);
        }
    }
}
