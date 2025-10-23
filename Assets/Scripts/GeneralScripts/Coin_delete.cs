using UnityEngine;

public class Coin_delete : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float destroyY = -7f;
    [SerializeField] private int coinValue = 1; // how many coins this pickup gives

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

        if (transform.position.y < destroyY)
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

            // optional: disable visuals immediately to avoid double hits
            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;

            Destroy(gameObject);
        }
    }
}
