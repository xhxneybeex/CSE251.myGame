using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // needed for reloading scenes
using System.Collections;

public class PlayerLifes : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject HeartExplosion;

    [Header("Lives")]
    public int maxLives = 3;

    [Header("Respawn / Restart")]
    public float respawnDelay = 1f;
    public string sceneToReload = "";

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

        UI = FindObjectOfType<UIManager>();
        SafeUpdateUI();

        onLivesChanged?.Invoke(currentLives);
    }

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
        SafeUpdateUI();
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

            // freeze player before restart
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

            StartCoroutine(DelayedRestart());
            return;
        }

        if (invincibleDuration > 0f)
        {
            StartCoroutine(StartIFrames());
        }
    }

    // waits a moment for the explosion, then reloads the whole scene
    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(respawnDelay);

        // if you left sceneToReload empty, reload whatever is active
        string target = string.IsNullOrEmpty(sceneToReload)
            ? SceneManager.GetActiveScene().name
            : sceneToReload;

        // if you hardcode, use exact case: "SampleScene"
        SceneManager.LoadScene(target);
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
