using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerLifes : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject HeartExplosion;

    [Header("Lives")]
    public int maxLives = 3;

    [Header("Respawn")]
    public Vector3 respawnPosition = new Vector3(-6f, -1f, 0f);
    public float respawnDelay = 1f;

    [Header("Invincibility Frames")]
    public float invincibleDuration = 1.0f;
    private bool invincible;

    [Header("Damage Flash")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.2f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Death Event")]
    public UnityEvent<int> onLivesChanged;
    public UnityEvent onPlayerOutOfLives;

    private int currentLives;
    private UIManager UI = null;

    void Awake()
    {
        currentLives = Mathf.Max(1, maxLives);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        if (HeartExplosion) HeartExplosion.SetActive(false);

        UI = FindObjectOfType<UIManager>(); // safer than GameObject.Find
        SafeUpdateUI();                      // show correct hearts immediately

        onLivesChanged?.Invoke(currentLives);
    }

    // REMOVE the per-frame UI write. It can mask your first hit change.
    // void Update() {}

    public void GiveLife(int amount = 1)
    {
        currentLives = Mathf.Min(maxLives, currentLives + amount);
        SafeUpdateUI();
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player gained life, lives: {currentLives}");
    }

    public void TakeHit(int amount = 1)
    {
        if (invincible) return;

        currentLives = Mathf.Max(0, currentLives - amount);
        SafeUpdateUI(); // update hearts immediately on FIRST hit
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player took damage, lives: {currentLives}");

        StartCoroutine(FlashDamage());

        if (currentLives <= 0)
        {
            Debug.Log("Player out of lives!");
            onPlayerOutOfLives?.Invoke();

            if (HeartExplosion != null)
            {
                HeartExplosion.transform.position = transform.position;
                HeartExplosion.SetActive(true);

                var anim = HeartExplosion.GetComponent<Animator>();
                if (anim)
                {
                    anim.Rebind();
                    anim.Update(0f);
                    anim.Play(0, 0, 0f);
                }
            }

            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
            var body = GetComponent<Rigidbody2D>();
            if (body)
            {
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
                body.simulated = false;
            }
            if (spriteRenderer) spriteRenderer.enabled = false;

            StartCoroutine(DelayedRespawn());
            return;
        }

        if (invincibleDuration > 0f)
        {
            StartCoroutine(StartIFrames());
        }
    }

    private IEnumerator DelayedRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
        currentLives = maxLives;
        SafeUpdateUI(); // show full hearts after respawn
        onLivesChanged?.Invoke(currentLives);

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;
        var body = GetComponent<Rigidbody2D>();
        if (body) body.simulated = true;
        if (spriteRenderer) spriteRenderer.enabled = true;

        if (HeartExplosion) HeartExplosion.SetActive(false);
    }

    private void Respawn()
    {
        var body = GetComponent<Rigidbody2D>();
        if (body) body.velocity = Vector2.zero;

        transform.position = respawnPosition;
        Debug.Log("Player respawned");
    }

    private IEnumerator StartIFrames()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        invincible = false;
    }

    private IEnumerator FlashDamage()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void SafeUpdateUI()
    {
        if (UI != null)
        {
            UI.UpdateLives(currentLives);
        }
    }

}
