using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerLifes : MonoBehaviour
{

    [Header("Explosion")]
    public GameObject HeartExplosion;   // child under the Player prefab, disabled by default

    //stuff to manage player lives, respawn, invincibility frames, and damage flash
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
    private UIManager UI = null; //can hold link to Canvas 

    void Awake()
    // Initialize lives and components
    {
        currentLives = Mathf.Max(1, maxLives);
        onLivesChanged?.Invoke(currentLives);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // ensure the child explosion starts OFF
        if (HeartExplosion) HeartExplosion.SetActive(false);
    }
    // Option to add lives with maxLives cap (3). Put into code later, not needed for now.

    void Start()
    {
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        //link to UI code
        UI.UpdateLives(currentLives);
    }

    void Update()
    {
     UI.UpdateLives(currentLives); //actual parameter aka value 
    }

    public void GiveLife(int amount = 1)
    {
        currentLives = Mathf.Min(maxLives, currentLives + amount);
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player gained life, lives: {currentLives}");
    }
    // Player takes damage, loses life, triggers invincibility frames and damage flash
    public void TakeHit(int amount = 1)
    {
        if (invincible) return;

        currentLives = Mathf.Max(0, currentLives - amount);
        onLivesChanged?.Invoke(currentLives);
        Debug.Log($"Player took damage, lives: {currentLives}");

        StartCoroutine(FlashDamage());

        // if lives are 0, spawn explosion and respawn
        if (currentLives <= 0)
        {
            Debug.Log("Player out of lives!");
            onPlayerOutOfLives?.Invoke();

            // turn ON the child explosion and restart its animation from frame 0
            if (HeartExplosion != null)
            {
                // if it’s a child, position is already correct; if not, sync position
                HeartExplosion.transform.position = transform.position;
                HeartExplosion.SetActive(true);

                var anim = HeartExplosion.GetComponent<Animator>();
                if (anim)
                {
                    anim.Rebind();      // reset all animated properties
                    anim.Update(0f);    // apply reset immediately
                    anim.Play(0, 0, 0f);// play entry state from time 0
                }
            }

            // Hide player visuals and collision temporarily
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
        onLivesChanged?.Invoke(currentLives);

        // re-enable player parts
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;
        var body = GetComponent<Rigidbody2D>();
        if (body) body.simulated = true;
        if (spriteRenderer) spriteRenderer.enabled = true;

        // turn OFF the child explosion after respawn
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
}
