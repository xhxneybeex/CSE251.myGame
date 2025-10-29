using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Hit/Death")]
    public int maxHits = 3;
    private int currentHits = 0;

    [Header("Visuals")]
    public SpriteRenderer sr;
    public Color hitColor = Color.red;
    private Color originalColor;

    [Header("Explosion")]
    public GameObject HeartExplosion;
    public float explosionLifetime = 0.6f;

    private UIManager ui;
    private bool countedKill = false;

    private void Awake()
    {
        // get sprite and remember the original color
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        // turn off explosion if it's already in the scene
        if (HeartExplosion != null && HeartExplosion.scene.IsValid())
            HeartExplosion.SetActive(false);

        // find UI
        ui = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // only react to player bullets
        if (other.CompareTag("GreenShooter"))
        {
            TakeHit();
            Destroy(other.gameObject);
        }
    }

    private void TakeHit()
    {
        currentHits++;

        // quick red flash when hit
        if (sr != null) StartCoroutine(FlashRed());

        // die if out of health
        if (currentHits >= maxHits)
            StartCoroutine(Die());
    }

    private IEnumerator FlashRed()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }

    private IEnumerator Die()
    {
        // add to kill count once
        if (!countedKill && ui != null)
        {
            countedKill = true;
            ui.AddKill(1);
        }

        GameObject fx = null;

        // handle explosion effect
        if (HeartExplosion != null)
        {
            if (HeartExplosion.scene.IsValid())
            {
                // if explosion is already part of the scene
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
                // if it's a prefab, spawn it
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

        // remove enemy
        Destroy(gameObject);

        // destroy explosion after a short delay
        if (fx != null)
        {
            yield return new WaitForSeconds(explosionLifetime);
            Destroy(fx);
        }
    }
}
