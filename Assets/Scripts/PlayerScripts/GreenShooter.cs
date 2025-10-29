using UnityEngine;

public class GreenShooter : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 12f;
    [SerializeField] private float lifeTime = 2f;

    private Vector3 direction;
    private float speed;
    private bool initialized = false;

    // Must be called right after Instantiate.
    // targetWorldPos: world point to fly toward (e.g., mouse world position)
    // speedOverride: >0 to override default speed
    // scale: 1 = normal, otherwise scales the projectile
    
    public void Initialize(Vector3 targetWorldPos, float speedOverride = 0f, float scale = 1f)
    {
        if (PauseManager.IsPaused) return; // ignore init while paused

        Vector3 v = targetWorldPos - transform.position;
        v.z = 0f;

        // Fallback to right if target is essentially the same position
        if (v.sqrMagnitude < 0.0001f) v = Vector3.right;

        direction = v.normalized;
        speed = (speedOverride > 0f) ? speedOverride : defaultSpeed;

        if (!Mathf.Approximately(scale, 1f))
            transform.localScale *= scale;

        initialized = true;
    }

    private void Start()
    {
        // If spawned while paused or not initialized by the spawner, kill it immediately
        if (PauseManager.IsPaused || !initialized)
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
