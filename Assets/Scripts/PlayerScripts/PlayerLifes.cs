using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // used to reload the scene
using System.Collections;

public class PlayerLifes : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject HeartExplosion; // death effect

    [Header("Lives")]
    public int maxLives = 3; // starting cap

    [Header("Respawn / Restart")]
    public float respawnDelay = 1f; // wait before restart
    public string sceneToReload = ""; // leave empty to reload current

    [Header("Invincibility Frames")]
    public float invincibleDuration = 1.0f; // time you can't be hit again
    private bool invincible;

    [Header("Damage Flash")]
    public Color damageColor = Color.red; // flash color
    public float flashDuration = 0.2f; // how long the flash lasts
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Death Event")]
    public UnityEvent<int> onLivesChanged; // notify UI or others
    public UnityEvent onPlayerOutOfLives;  // fired when lives hit zero

    private int currentLives;
    private UIManager UI = null;

    void Awake()
    {
        // clamp starting lives
        currentLives = Mathf.Max(1, maxLives);

        // cache sprite and color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // keep explosion hidden until needed
        if (HeartExplosion) HeartExplosion.SetActive(false);

        // find UI and sync it
        UI = FindObjectOfType<UIManager>();
        SafeUpdateUI();

        // let listeners know starting lives
        onLivesChanged?.Invoke(currentLives);
    }

    public void GiveLife(int amount = 1)
    {
        // add life but don't pass max
        currentLives = Mathf.Min(maxLives, currentLives + amount);
        SafeUpdateUI();
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player gained life, lives: {currentLives}");
    }

    public void TakeHit(int amount = 1)
    {
        // ignore if in i-frames
        if (invincible) return;

        // reduce lives and update UI
        currentLives = Mathf.Max(0, currentLives - amount);
        SafeUpdateUI();
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player took damage, lives: {currentLives}");

        // quick damage flash
        StartCoroutine(FlashDamage());

        // handle death
        if (currentLives <= 0)
        {
            Debug.Log("Player out of lives!");
            onPlayerOutOfLives?.Invoke();

            // play death effect if set
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

            // restart scene after a short delay
            StartCoroutine(DelayedRestart());
            return;
        }

        // start i-frames after getting hit iframes r invisibility
        if (invincibleDuration > 0f)
        {
            StartCoroutine(StartIFrames());
        }
    }

    // wait a bit for the effect, then reload the scene
    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(respawnDelay);

        // pick target scene, default to active one
        string target = string.IsNullOrEmpty(sceneToReload)
            ? SceneManager.GetActiveScene().name
            : sceneToReload;

        SceneManager.LoadScene(target);
    }

    private IEnumerator StartIFrames()
    {
        // turn on i-frames
        invincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        invincible = false; // back to normal
    }

    private IEnumerator FlashDamage()
    {
        // swap to hit color, then back
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void SafeUpdateUI()
    {
        // update UI if it exists
        if (UI != null)
        {
            UI.UpdateLives(currentLives);
        }
    }
}
