using UnityEngine;

public class GreenShooter : MonoBehaviour
{
    //fancy word for putting something in the inspector.
    [SerializeField] float defaultSpeed = 12f;

    private float speed;
    private Vector3 direction;
    private bool initialized = false;

    // Call this right after Instantiate

    //targetWorldPos: where the bullet should fly toward (mouse click position).
    //speedOverride: lets you change the bullet’s speed(for power-ups).
    //scale: lets you change the bullet’s size(for power-ups).
    public void Initialize(Vector3 targetWorldPos, float speedOverride, float scale)
    {
        direction = (targetWorldPos - transform.position);
        direction.z = 0f;
        direction.Normalize();

        speed = speedOverride > 0f ? speedOverride : defaultSpeed;
        if (scale != 1f) transform.localScale *= scale;

        initialized = true;
    }

    void Start()
    {
        // Fallback if greenshooter forgets to Initialize
        if (!initialized)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            direction = (mousePos - transform.position).normalized;
            speed = defaultSpeed;
        }

        Destroy(gameObject, 2f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
