using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Hit/Death")]
    public int maxHits = 3;
    private int currentHits = 0;

    [Header("Visuals")]
    public SpriteRenderer sr;          // optional, auto-filled if left empty
    public Color hitColor = Color.red;
    private Color originalColor;

    [Header("Explosion")]
    // If this is a prefab, we will Instantiate it.
    // If this is a child object on the enemy, we will detach, play, then destroy it.
    public GameObject HeartExplosion;
    public float explosionLifetime = 0.6f; // seconds to keep the explosion alive

    // added
    private UIManager ui;
    private bool countedKill = false; // prevents double counting

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        if (HeartExplosion != null && HeartExplosion.scene.IsValid())
        {
            HeartExplosion.SetActive(false);
        }

        ui = FindObjectOfType<UIManager>(); // safe lookup
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Consider renaming your projectile tag to "PlayerProjectile"
        if (other.CompareTag("GreenShooter"))
        {
            TakeHit();
            Destroy(other.gameObject); // kill bullet on hit
        }
    }

    private void TakeHit()
    {
        currentHits++;

        if (sr != null) StartCoroutine(FlashRed());

        if (currentHits >= maxHits)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator FlashRed()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }

    private IEnumerator Die()
    {
        // count kill once
        if (!countedKill && ui != null)
        {
            countedKill = true;
            ui.AddKill(1);
        }

        // Play explosion either by instantiating a prefab or by using an inactive child
        GameObject fx = null;

        if (HeartExplosion != null)
        {
            if (HeartExplosion.scene.IsValid())
            {
                // HeartExplosion is a child already in the scene
                HeartExplosion.transform.SetParent(null, true);
                HeartExplosion.transform.position = transform.position;
                HeartExplosion.SetActive(true);

                var anim = HeartExplosion.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Rebind();
                    anim.Update(0f);
                    anim.Play(0, 0, 0f);
                }

                fx = HeartExplosion;
            }
            else
            {
                // HeartExplosion is a prefab
                fx = Instantiate(HeartExplosion, transform.position, Quaternion.identity);
                var anim = fx.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Rebind();
                    anim.Update(0f);
                    anim.Play(0, 0, 0f);
                }
            }
        }

        // remove the enemy now
        Destroy(gameObject);

        // let the explosion live for a moment, then clean up
        if (fx != null)
        {
            yield return new WaitForSeconds(explosionLifetime);
            Destroy(fx);
        }
    }
}
